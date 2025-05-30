using System;
using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using EPOOutline;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _BaseGame.Script.Unit
{
    public class UnitBase : MonoBehaviour
    {
        [FormerlySerializedAs("block")] [BoxGroup("Unit Settings")]
        public BlockInit blockInit;
        [BoxGroup("Unit Settings")]
        public ColorType colorType;
        [BoxGroup("Unit Settings")]
        public UnitType unitType;
        [BoxGroup("Unit Settings")]
        public MoveType moveType;
        
        [BoxGroup("Unit Settings")]
        public ArrowType arrowType;
        [BoxGroup("Unit Settings")] public Vector3 arrowPosition;
        
        [BoxGroup("Unit Settings")] public ChainInit chainInit;

        [BoxGroup("Unit Settings")]
        [Button]
        private void InitData()
        {
            blockInit.Setting(unitType, moveType, colorType, arrowType, arrowPosition);
            blockInit.InitData();
            chainInit.InitData(unitType, true);
        }

        public void InitData(TiledConfig tiledConfig)
        {
            unitType = tiledConfig.unitType;
            moveType = tiledConfig.moveType;
            colorType = tiledConfig.colorType;
            transform.eulerAngles = new Vector3(0, tiledConfig.rotateY, 0);
            arrowType = tiledConfig.arrowType;
            arrowPosition = tiledConfig.arrowPosition;
            
            chainInit.InitData(unitType, tiledConfig.blockSpecialType == BlockSpecialType.Chain);
            
            blockInit.Setting(unitType, moveType, colorType, arrowType, arrowPosition);
            blockInit.InitData();
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotation;
            rb.constraints = rb.constraints | RigidbodyConstraints.FreezePositionY;
            switch (moveType)
            {
                case MoveType.Horizontal:
                    rb.constraints = rb.constraints | RigidbodyConstraints.FreezePositionZ;
                    break;
                case MoveType.Vertical:
                    rb.constraints = rb.constraints | RigidbodyConstraints.FreezePositionX;
                    break;
                case MoveType.None:
                default:
                    break;
            }
        }

        public void ResetUnit()
        {
            if (motionHandle.IsPlaying())
                motionHandle.TryCancel();
            status = Status.None;
            outline.FrontParameters.DilateShift = 0;
            rb.isKinematic = true;
        }


        
        public Outlinable outline;
        private Camera mainCamera;
        public Rigidbody rb;
        public float speed = 5f;
        public float distanceMin;
       

        private Vector3 lastPoint;
        public Status status;

        public List<Collider> myColliders = new();
        public List<Vector3> pointBlocks = new();
        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            status = Status.Drop;
            GameController.Instance.AddUnitBase(this);
        }
        
        private bool IsCanMove()
        {
            if (chainInit.isChain) return false;
            return true;
        }

        public void MovePosition(Vector3 pointCast)
        {
            if (!IsCanMove())
                return;
            var targetPosition = pointCast + vectorDir;
            targetPosition.y = transform.position.y;
            
            var distance = Vector3.Distance(transform.position, targetPosition);
            if (distance > distanceMin)
            {
                var direction = (targetPosition - transform.position);
                var targetVelocity = direction * speed;
                switch (moveType)
                {
                    case MoveType.Horizontal:
                        targetVelocity.z = 0;
                        break;
                    case MoveType.Vertical:
                        targetVelocity.x = 0;
                        break;
                    case MoveType.None:
                    default:
                        break;
                }
                //rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * timeSmooth);
                rb.velocity = targetVelocity;
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }

        private MotionHandle motionHandle;
        public LayerMask mask;
        public LayerMask gateMask;
        
        public float lengthCast;

        public Vector3 vectorDir;
        public void OnDrop()
        {
            outline.FrontParameters.DilateShift = 0;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            //Debug.DrawLine(transform.position, transform.position - transform.up * lengthCast, Color.red);
            if (!Physics.Linecast(transform.position, transform.position - transform.up * lengthCast, out var hit,
                    mask)) return;
            var target = hit.transform.position;
            target.y = transform.position.y;
            if (motionHandle.IsPlaying())
                motionHandle.TryCancel();
            motionHandle = 
                LMotion.Create(transform.position, target, 0.1f)
                    .WithOnComplete(() =>
                    {
                        status = Status.Drop;
                        CheckOnGate();
                    })
                    .Bind(x => transform.position = x)
                    .AddTo(this);

        }

        private void CheckOnGate()
        {
            for (var i = 0; i < myColliders.Count; i++)
            {
                // Center of the sphere
                var results = new Collider[10];
                var size = Physics.OverlapSphereNonAlloc(myColliders[i].bounds.center, 0.5f, results, gateMask);
                if (size <= 0) continue;
                foreach (var colliderCast in results)
                {
                    if (!colliderCast) continue;
                    if (colliderCast.gameObject.layer != 7) continue;
                    var gate = GameController.Instance.GetGate(colliderCast.gameObject);
                    if (gate.IsCanPassGate(this))
                    {
                        //LMotion.Create(1f,0f,.25f).Bind(x => transform.localScale = Vector3.one * x).AddTo(this);
                        ResolvedMode(gate);
                    }
                }
            }
          
        }

        private void ResolvedMode(Gate gate)
        {
            for (var i = 0; i < myColliders.Count; i++)
            {
                myColliders[i].enabled = false;
            }

            blockInit.ResolvedMode();
            var pointGate = gate.transform.position;
            var dir = pointGate - transform.position;
            switch (gate.checkType)
            {
                case CheckType.Vertical:
                    dir.x = 0;
                    break;
                case CheckType.Horizontal:
                    dir.z = 0;
                    break;
                default:
                    break;
            }
            motionHandle = LMotion.Create(transform.position, transform.position + dir * 4f, 0.5f).Bind(x => transform.position = x)
                .AddTo(this);
            gate.PlayEffect();
        }

        public void PickUp()
        {
            rb.isKinematic = false;
            outline.FrontParameters.DilateShift = 1;
            status = Status.PickUp;
        }


        public bool CheckVector(Vector3 vectorEnd, CheckType checkType, Gate gateCheck)
        {
            CreatePointBlocks();
            for (var i = 0; i < pointBlocks.Count; i++)
            {
                var dir = vectorEnd;
                dir.y = pointBlocks[i].y;
                dir.x = checkType == CheckType.Vertical ? pointBlocks[i].x : dir.x;
                dir.z = checkType == CheckType.Horizontal ? pointBlocks[i].z : dir.z;
                
                var point = pointBlocks[i];
                var pointEnd = dir;
                Debug.DrawLine(point, pointEnd, Color.red, 10f);
                if (Physics.Linecast(point, pointEnd, out var hitInfo))
                {
                    if (!myColliders.Contains(hitInfo.collider) && !gateCheck.ContainsCollider(hitInfo.collider))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        private void CreatePointBlocks()
        {
            pointBlocks.Clear();
            for (var i = 0; i < myColliders.Count; i++)
            {
                var point = myColliders[i].bounds.center;
                pointBlocks.Add(point);
            }
        }

        public void SetPointDefault(Vector3 hitPlanePoint)
        {
            vectorDir = transform.position - hitPlanePoint;
        }

        public void Settings(Vector3 unitPosition, UnitType unitType1, MoveType moveType1, ArrowType arrowType1,
            ColorType colorUnitType, Vector3 vectorUnitEuler, Vector3 vectorArrow)
        {
            transform.localPosition = unitPosition;
            unitType = unitType1;
            moveType = moveType1;
            arrowType = arrowType1;
            colorType = colorUnitType;
            transform.eulerAngles = vectorUnitEuler;
            arrowPosition = vectorArrow;
        }

        public Vector3 GetArrowPosition()
        {
            return blockInit.GetArrowPosition();
        }
    }
    

    public enum Status
    {
        None,
        PickUp,
        Drop
    }

    public enum MoveType
    {
        None,
        Vertical,
        Horizontal,
    }

    [System.Serializable]
    public class PositionCube
    {
        public float pointX;
        public float pointY;
    }
}
