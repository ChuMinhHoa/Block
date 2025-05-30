using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _BaseGame.Script.Unit
{
    public class Wall : MonoBehaviour
    {
        public WallType wallType = WallType.Wall1;
        public List<GameObject> objWalls = new();

        public void InitData(TiledConfig config)
        {
            wallType = config.wallType;
            transform.localScale = config.scale;
            transform.eulerAngles = new Vector3(0, config.rotateY, 0);
            for (var i = 0; i < objWalls.Count; i++)
            {
                objWalls[i].SetActive(false);
            }

            if (wallType == WallType.None)
            {
                objWalls[0].SetActive(true);
                return;
            }
            objWalls[(int)wallType-1].SetActive(true);
        }
        
        [Button]
        public void InitData(WallType wallTypeInit)
        {
            wallType = wallTypeInit;
            for (var i = 0; i < objWalls.Count; i++)
            {
                objWalls[i].SetActive(false);
            }

            objWalls[(int)wallType-1].SetActive(true);
        }
    }
    
    public enum WallType
    {
        None,
        Wall1,
        Wall2,
        Wall3,
        Wall4,
        Block,
        Conner
    }
}
