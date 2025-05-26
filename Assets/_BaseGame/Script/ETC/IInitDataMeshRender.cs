using Sirenix.OdinInspector;
using UnityEngine;

namespace _BaseGame.Script.ETC
{
    public class InitDataMeshRender<T> : MonoBehaviour
    {
        [BoxGroup("Init Data")]
        [ShowInInspector]public T Data { get; set; }
        [BoxGroup("Init Data")]
        [ShowInInspector]public MeshRenderer MyMeshRenderer { get; set; }
        [BoxGroup("Init Data")]
        [ShowInInspector]public MeshFilter MyMeshFilter { get; set; }
        [BoxGroup("Init Data"), Button]
        public virtual void InitData(T dataConfig)
        {
            Data = dataConfig;
        }
    }
}
