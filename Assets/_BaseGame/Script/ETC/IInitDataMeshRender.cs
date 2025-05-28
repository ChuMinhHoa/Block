using UnityEngine;

namespace _BaseGame.Script.ETC
{
   [System.Serializable]
    public class InitDataMeshRender<T>
    {
        public T data;
        public MeshRenderer myMeshRenderer;
        public MeshFilter myMeshFilter;
        
        public void InitData(T dataConfig)
        {
            data = dataConfig;
        }
    }
}
