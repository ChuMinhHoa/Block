using System;
using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _BaseGame.Script.Unit
{
    public class Block : MonoBehaviour
    {
        public UnitBase unitBase;
        [FormerlySerializedAs("blockType")] public UnitType unitType;
        public ColorType colorType;
        
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public Collider colliderPref;
        public Transform parents;

        [Button]
        public void InitData()
        {
            var blockData = BlockDataGlobalConfig.Instance.blockDataConfig.Find(x => x.type == unitType);
            if (blockData != null)
            {
                meshFilter.mesh = blockData.mesh;
                meshFilter.transform.localPosition = blockData.vectorRightDefault;
                
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
            }
            else
            {
                Debug.LogError($"Block data not found for type: {unitType}");
            }

            var materialData = BlockDataGlobalConfig.Instance.materialData.Find(x => x.colorType == colorType);
            if (materialData != null)
            {
                meshRenderer.material = materialData.material;
            }
            else
            {
                Debug.LogError($"Material data not found for color type: {colorType}");
            }
        }
        
        [Button]
        private void SaveBackPosition()
        {
            var blockData = BlockDataGlobalConfig.Instance.blockDataConfig.Find(x => x.type == unitType);
            if (blockData != null)
            {
                blockData.vectorRightDefault = meshFilter.transform.localPosition;
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
            var newMaterial = new Material(meshRenderer.material);
            newMaterial.SetOverrideTag("RenderMode", "Transparent");
            newMaterial.SetOverrideTag("RenderType", "Transparent");
            newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            newMaterial.SetInt("_ZWrite", 0);
            newMaterial.DisableKeyword("_ALPHATEST_ON");
            newMaterial.EnableKeyword("_ALPHABLEND_ON");
            newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            newMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent+100;
            meshRenderer.material = newMaterial;
        }
    }
}
