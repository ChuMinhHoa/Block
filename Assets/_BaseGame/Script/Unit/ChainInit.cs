using _BaseGame.Script.DataConfig;
using _BaseGame.Script.ETC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _BaseGame.Script.Unit
{
   public class ChainInit : MonoBehaviour
   {
      public bool isChain = false;
      public int keyID;
      public InitDataMeshRender<ChainConfig> initChain;

      public void InitData(ChainConfig chainConfig)
      {
         initChain.InitData(chainConfig);
         initChain.myMeshFilter.mesh = chainConfig.mesh;
         transform.localEulerAngles = chainConfig.rotate;
         transform.localPosition = chainConfig.position;
      }

      [Button]
      public void InitData(UnitType unitType, bool setIsChain = false)
      {
         isChain = setIsChain;
         if (!isChain)
         {
            gameObject.SetActive(false);
            return;
         }

         gameObject.SetActive(true);
         var chainConfig = BlockDataGlobalConfig.Instance.chainConfigs.Find(x => x.unitType == unitType);
         if (chainConfig != null)
         {
            InitData(chainConfig);
         }
         else
         {
            Debug.LogError($"ChainConfig not found for UnitType: {unitType}");
         }
      }

      [Button]
      public void SaveBackData()
      {
         var chainConfig = BlockDataGlobalConfig.Instance.chainConfigs.Find(x => x.unitType == initChain.data.unitType);
         if (chainConfig != null)
         {
            chainConfig.mesh = initChain.myMeshFilter.sharedMesh;
            chainConfig.rotate = transform.localEulerAngles;
            chainConfig.position = transform.localPosition;
         }
         else
         {
            chainConfig = new ChainConfig
            {
               unitType = initChain.data.unitType,
               mesh = initChain.myMeshFilter.sharedMesh,
               rotate = transform.localEulerAngles,
               position = transform.localPosition
            };
            BlockDataGlobalConfig.Instance.chainConfigs.Add(chainConfig);
         }

         UnityEditor.EditorUtility.SetDirty(BlockDataGlobalConfig.Instance);
         UnityEditor.AssetDatabase.SaveAssets();
      }

      public bool IsUnlock()
      {
         return isChain;
      }

      public void UnlockKey(int key)
      {
         if (keyID == key)
         {
            isChain = false;
            gameObject.SetActive(false);
         }
      }
   }
}
