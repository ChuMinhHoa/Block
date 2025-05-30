using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using _BaseGame.Script.ETC;
using _BaseGame.Script.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnitBase = _BaseGame.Script.Unit.UnitBase;

namespace _BaseGame.Script.Grid
{
    public class ObjDrawMap : MonoBehaviour
    {
        public TiledType tiledType;
        
        public List<GameObject> objShows = new();
        
        public PointOnGrid PointOnGrid = new();
        
        public GameObject currentObjShow;

        public void InitData(TiledType type, int x, int y)
        {
            for (var i = 0; i < objShows.Count; i++)
            {
                objShows[i].SetActive(false);
            }
            tiledType = type;
            PointOnGrid.SetPoint(x, y);
            if (tiledType == TiledType.None)
                return;
            currentObjShow = objShows[((int)type - 1)];
            currentObjShow.SetActive(true);
        }

        public void InitUnit(TiledConfig tiledConfig)
        {
            var unitBase = currentObjShow.GetComponent<UnitBase>();
          
            unitBase.InitData(tiledConfig);
        }

        public void InitGate(TiledConfig tiledConfig)
        {
            var gate = currentObjShow.GetComponent<Gate>();
            gate.InitData(tiledConfig);
        }

        [Button]
        public void PreInit()
        {
            for (var i = 0; i < objShows.Count; i++)
            {
                objShows[i].SetActive(false);
            }

            if (tiledType == TiledType.None)
                return;
            currentObjShow = objShows[((int)tiledType - 1)];
            currentObjShow.SetActive(true);
            
            switch (tiledType)  
            {
                case TiledType.Unit:
                    var unitBase = currentObjShow.GetComponent<UnitBase>();
                    unitBase.ResetUnit();
                    unitBase.InitData(config);
                    break;
                case TiledType.Gate:
                    var gate = currentObjShow.GetComponent<Gate>();
                    gate.InitData(config);
                    break;
                case TiledType.Block:
                    Debug.Log("Block Init");
                    var wall = currentObjShow.GetComponent<Wall>();
                    wall.InitData(config);
                    break;
            }
        }

        public TiledConfig config;

        public void InitBlock(TiledConfig tiledConfig)
        {
            var wall = currentObjShow.GetComponent<Wall>();
            wall.InitData(tiledConfig);
        }
    }
}
