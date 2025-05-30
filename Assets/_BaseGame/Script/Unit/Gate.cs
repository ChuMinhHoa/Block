using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using _BaseGame.Script.ETC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _BaseGame.Script.Unit
{
    public enum ColorType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        Orange
    }
    public class Gate : MonoBehaviour
    {
        public GateType gateType;
        public ColorType colorType;
        public CheckType checkType;
        public List<Collider> colliders = new();
        public ParticleSystem effect;
        public InitDataMeshRender<GateMeshConfig> initMesh;
        public Transform trsArrow;
        private void Start()
        {
            GameController.Instance.AddGate(this);
        }

        public bool IsCanPassGate(UnitBase unit)
        {
            if (unit.colorType != colorType) return false;
            if (!IsSameCheckType(unit.moveType)) return false;
            if (!unit.CheckVector(transform.position, checkType, this)) return false;
            return true;
        }
        
        bool IsSameCheckType(MoveType moveType)
        {
            switch (checkType)
            {
                case CheckType.Horizontal when moveType == MoveType.Horizontal:
                case CheckType.Vertical when moveType == MoveType.Vertical:
                    return true;
                default:
                    return false;
            }
        }

        public bool ContainsCollider(Collider hitInfoCollider)
        {
            //Debug.Log(""hitInfoCollider.gameObject.name);
            return colliders.Contains(hitInfoCollider);
        }

        [Button]
        public void PlayEffect()
        {
            effect.Play();
        }

        [BoxGroup("Init Data")]
        [Button]
        public void InitData(TiledConfig tiledConfig)
        {
            gateType = tiledConfig.gateType;
            checkType = tiledConfig.checkType;
            colorType = tiledConfig.colorType;
            var vectorEuler = new Vector3(0, tiledConfig.rotateY, 0);
            transform.eulerAngles = vectorEuler;
            
            var dataConfig = BlockDataGlobalConfig.Instance.gateDataConfigs.Find(x => x.type == gateType);
            initMesh.InitData(dataConfig);
            initMesh.myMeshFilter.mesh = dataConfig.mesh;
            var gateData = BlockDataGlobalConfig.Instance.gateMData.Find(x => x.colorType == colorType);
            initMesh.myMeshRenderer.material = gateData.material;
            for (var i = 0; i < colliders.Count; i++)
            {
                var sizeCollider = (colliders[i] as BoxCollider).size;
                sizeCollider.x = 1f * ((int)dataConfig.type + 1)- i * 0.5f;
                var center = (colliders[i] as BoxCollider).center;
                center.x = -0.5f * (int)dataConfig.type;
                (colliders[i] as BoxCollider).size = sizeCollider;
                (colliders[i] as BoxCollider).center = center;
            }
            var effectPosition = effect.transform.localPosition;
            effectPosition.x = -0.5f  * (int)dataConfig.type;
            effect.transform.localPosition = effectPosition;
            var arrowPosition= effectPosition;
            arrowPosition.y = trsArrow.localPosition.y;
            arrowPosition.z = 0;
            trsArrow.localPosition = arrowPosition;
            
            var shape = effect.shape;
            shape.radius = 0.5f * ((int)dataConfig.type + 1);
            var point = transform.localPosition;
            point += transform.forward * 0.25f;
            transform.localPosition = point;
            var colorData = BlockDataGlobalConfig.Instance.colorData.Find(x => x.colorType == colorType);
            effect.startColor = colorData.color;
        }
    }
    
    
    public enum CheckType
    {
        Vertical,
        Horizontal,
    }
}
