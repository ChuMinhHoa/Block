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
            if (tiledType == TiledType.None)
                return;
            currentObjShow = objShows[((int)type - 1)];
            currentObjShow.SetActive(true);
            PointOnGrid.SetPoint(x, y);
        }

        public void InitUnit(TiledConfig tiledConfig)
        {
            var unitBase = currentObjShow.GetComponent<UnitBase>();
            unitBase.unitType = tiledConfig.unitType;
            unitBase.moveType = tiledConfig.moveType;
            unitBase.colorType = tiledConfig.colorType;
            unitBase.transform.eulerAngles = new Vector3(0, tiledConfig.rotateY, 0);
            unitBase.InitData();
        }

        public void InitGate(TiledConfig tiledConfig)
        {
            var gate = currentObjShow.GetComponent<Gate>();
            gate.gateType = tiledConfig.gateType;
            gate.colorType = tiledConfig.colorType;
            gate.checkType = tiledConfig.checkType;
            gate.transform.eulerAngles = new Vector3(0, tiledConfig.rotateY, 0);
            gate.InitData();
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
            //PointOnGrid.SetPoint((int)transform.position.x, (int)transform.position.y);

           
        }
    }
}
