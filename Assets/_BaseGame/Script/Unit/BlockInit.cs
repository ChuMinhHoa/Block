using _BaseGame.Script.DataConfig;
using _BaseGame.Script.ETC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _BaseGame.Script.Unit
{
    public class BlockInit : MonoBehaviour
    {
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        public UnitBase unitBase;
        public UnitType unitType;
        public ColorType colorType;
        public MoveType moveType;
        public ArrowType arrowType;

        public InitDataMeshRender<BlockDataConfig> initDataBlock;
        public InitDataMeshRender<ArrowMeshConfig> initDataMoveType;

        public Collider colliderPref;
        public Transform parents;
        public Transform trsMoveType;
        public Vector3 arrowPosition;

        [Button]
        public void InitData()
        {
            var blockData = BlockDataGlobalConfig.Instance.blockDataConfig.Find(x => x.type == unitType);
            var materialData = BlockDataGlobalConfig.Instance.materialData.Find(x => x.colorType == colorType);
            if (blockData != null)
            {
                initDataBlock.myMeshFilter.mesh = blockData.mesh;
                initDataBlock.myMeshFilter.transform.localPosition = blockData.vectorRightDefault;
                
                for (var i = 0; i < unitBase.myColliders.Count; i++)
                {
#if UNITY_EDITOR
                    DestroyImmediate(unitBase.myColliders[i].gameObject);
#endif
                    //Destroy(unitBase.myColliders[i].gameObject);
                }
                
                unitBase.myColliders.Clear();

                for (var i = 0; i < blockData.pointColliders.Count; i++)
                {
                    var colliderTemp = Instantiate(colliderPref, parents);
                    colliderTemp.transform.localPosition = new Vector3(blockData.pointColliders[i].x, 0, blockData.pointColliders[i].y);
                    unitBase.myColliders.Add(colliderTemp);
                }
                var moveData = BlockDataGlobalConfig.Instance.moveObjData.Find(x => x.type == arrowType);
                InitMoveData(moveData);
            }
            else
            {
                Debug.LogError($"Block data not found for type: {unitType}");
            }

           
            if (materialData != null)
            {
                initDataBlock.myMeshRenderer.material = materialData.material;
            }
            else
            {
                Debug.LogError($"Material data not found for color type: {colorType}");
            }
            
           
        }

        private void InitMoveData(ArrowMeshConfig arrowConfig)
        {
            initDataMoveType.InitData(arrowConfig);
            initDataMoveType.myMeshFilter.mesh = arrowConfig.mesh;
            if(moveType == MoveType.Horizontal)
                trsMoveType.eulerAngles = new Vector3(0, 90, 0);
            else
            {
                trsMoveType.eulerAngles = new Vector3(0, 0, 0);
            }

            trsMoveType.transform.localPosition = arrowPosition;
        }

        [Button]
        private void SaveBackPosition()
        {
            var blockData = BlockDataGlobalConfig.Instance.blockDataConfig.Find(x => x.type == unitType);
            if (blockData != null)
            {
                blockData.vectorRightDefault = initDataBlock.myMeshFilter.transform.localPosition;
                blockData.pointColliders.Clear();
                for (var i = 0; i < unitBase.myColliders.Count; i++)
                {
                    var vector = unitBase.myColliders[i].transform.localPosition;
                    var vector2D = new Vector2(vector.x, vector.z);
                    blockData.pointColliders.Add(vector2D);
                }
                #if UNITY_EDITOR
                // Mark the config as dirty to ensure changes are saved in the editor
                UnityEditor.EditorUtility.SetDirty(BlockDataGlobalConfig.Instance);
                UnityEditor.AssetDatabase.SaveAssets();
                #endif
            }
            else
            {
                Debug.LogError($"Block data not found for type: {unitType}");
            }
        }
        [Button]
        public void ResolvedMode()
        {
            var newMaterial = new Material(initDataBlock.myMeshRenderer.material);
            newMaterial.SetOverrideTag("RenderMode", "Transparent");
            newMaterial.SetOverrideTag("RenderType", "Transparent");
            newMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            newMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            newMaterial.SetInt(ZWrite, 0);
            newMaterial.DisableKeyword("_ALPHATEST_ON");
            newMaterial.EnableKeyword("_ALPHABLEND_ON");
            newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            newMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+100;
            initDataBlock.myMeshRenderer.material = newMaterial;
            
            var newMaterialArrow = new Material(initDataMoveType.myMeshRenderer.material);
            newMaterialArrow.SetOverrideTag("RenderMode", "Transparent");
            newMaterialArrow.SetOverrideTag("RenderType", "Transparent");
            newMaterialArrow.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            newMaterialArrow.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            newMaterialArrow.SetInt(ZWrite, 0);
            newMaterialArrow.DisableKeyword("_ALPHATEST_ON");
            newMaterialArrow.EnableKeyword("_ALPHABLEND_ON");
            newMaterialArrow.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            newMaterialArrow.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+100;
            initDataMoveType.myMeshRenderer.material = newMaterialArrow;
        }

        public void Setting(UnitType unitType1,MoveType moveType1, ColorType colorType1, ArrowType arrowTypeChange, Vector3 arrowPositionChange)
        {
            unitType = unitType1;
            moveType = moveType1;
            colorType = colorType1;
            arrowType = arrowTypeChange;
            arrowPosition = arrowPositionChange;
        }

        public Vector3 GetArrowPosition()
        {
            return trsMoveType.localPosition;
        }
    }
}
