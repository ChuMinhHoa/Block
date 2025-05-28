using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using _BaseGame.Script.ETC;
using _BaseGame.Script.Grid;
using _BaseGame.Script.Unit;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _BaseGame.Script.Manager
{
    public class MapManager : Singleton<MapManager>
    {
        public bool drawMapMode = true;
        public GridSpawn gridSpawn;

        private int GridSizeX => gridSpawn.gridSizeX;
        private int GridSizeY => gridSpawn.gridSizeY;
        private float Spacing => gridSpawn.spacing;
        private MapConfig MapConfig => gridSpawn.mapConfig;
        
        
        [BoxGroup("Draw Map")] public ObjPool<ObjDrawMap> objDrawMapPool = new();
        [BoxGroup("Draw Map")]
        [Button(50)]
        private void DrawMap()
        {
            ClearPool(objDrawMapPool);
            objDrawMaps.Clear();



            if (MapConfig == null || MapConfig.tiledConfigs.Count == 0)
            {
                SpawnNewMap();
            }
            else
            {
                SpawnExistMap();
            }

           
        }

        private void SpawnExistMap()
        {
            var positionZ = GridSizeX % 2 == 0 ? (int)(GridSizeX / 2f) - 0.5f : (int)(GridSizeX / 2f);
            var positionX = GridSizeY % 2 == 0 ? (int)(GridSizeY / 2f) - 0.5f : (int)(GridSizeY / 2f);
            for (var i = 0; i < GridSizeX; i++)
            {
                for (var j = 0; j < GridSizeY; j++)
                {
                    var position = new Vector3(-positionX + j * Spacing, 0, -positionZ + i * Spacing);
                    var objDrawMap = objDrawMapPool.SpawnEditor();
                    objDrawMap.transform.position = position;
                    var tiledConfig = MapConfig.tiledConfigs[i].tiledConfigs[j];
                    if (tiledConfig == null)
                    {
                        tiledConfig = new TiledConfig
                        {
                            tiledType = TiledType.Plane
                        };
                    }

                    objDrawMap.InitData(tiledConfig.tiledType, i, j);
                    objDrawMaps.Add(objDrawMap);
                    switch (tiledConfig.tiledType)
                    {
                        case TiledType.Gate:
                            objDrawMap.InitGate(tiledConfig);
                            break;
                        case TiledType.Unit:
                            objDrawMap.InitUnit(tiledConfig);
                            break;    
                    }
                }
            }
        }

        private void SpawnNewMap()
        {
            var positionZ = GridSizeX % 2 == 0 ? (int)(GridSizeX / 2f) - 0.5f : (int)(GridSizeX / 2f);
            var positionX = GridSizeY % 2 == 0 ? (int)(GridSizeY / 2f) - 0.5f : (int)(GridSizeY / 2f);
            for (var i = 0; i < GridSizeX; i++)
            {
                for (var j = 0; j < GridSizeY; j++)
                {
                    var position = new Vector3(-positionX + j * Spacing, 0, -positionZ + i * Spacing);
                    var objDrawMap = objDrawMapPool.SpawnEditor();
                    objDrawMap.transform.position = position;
                    objDrawMap.InitData(TiledType.Plane, i, j);
                    objDrawMaps.Add(objDrawMap);
                }
            }
        }

        private void ClearPool<T>(ObjPool<T> objPool) where T : Component
        {
            objPool.Clear();
            for (var i = objPool.trsParents.childCount-1; i >= 0; i--)
            {
                var child = objPool.trsParents.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }
        [BoxGroup("Draw Map")]
        public List<ObjDrawMap> objDrawMaps = new();
        [BoxGroup("Draw Map")]
        [Button(100)]
        private void SaveDataToConfig()
        {
            var mapConfig = LevelDataGlobalConfig.Instance.mapConfigs.Find(x => x.level == gridSpawn.level);
            if (mapConfig == null)
            {
                mapConfig = new MapConfig();
                mapConfig.level = gridSpawn.level;
            }

            LevelDataGlobalConfig.Instance.mapConfigs.Add(mapConfig);
            mapConfig.tiledConfigs.Clear();
            for (int i = 0; i < gridSpawn.gridSizeX; i++)
            {
                var tiledConfigRow = new TiledConfigRow();
                for (int j = 0; j < gridSpawn.gridSizeY; j++)
                {
                    var tiledConfig = new TiledConfig();
                    var objDrawMap = objDrawMaps.Find(x => x.PointOnGrid.x == i && x.PointOnGrid.y == j);
                    Debug.Log(objDrawMap);
                    tiledConfig.tiledType = objDrawMap.tiledType;
                    switch (objDrawMap.tiledType)
                    {
                        case TiledType.None:
                            tiledConfig.tiledType = TiledType.None;
                            break;
                        case TiledType.Plane:
                            tiledConfig.tiledType = TiledType.Plane;
                            break;    
                        case TiledType.Block:
                            break;
                        case TiledType.Gate:
                            var gate = objDrawMap.currentObjShow.GetComponent<Gate>();
                            tiledConfig.gateType = gate.gateType;
                            tiledConfig.colorType = gate.colorType;
                            tiledConfig.rotateY = gate.transform.eulerAngles.y;
                            tiledConfig.checkType = gate.checkType;
                            break;
                        case TiledType.Unit:
                            var unit = objDrawMap.currentObjShow.GetComponent<UnitBase>();
                            tiledConfig.unitType = unit.unitType;
                            tiledConfig.colorType = unit.colorType;
                            tiledConfig.rotateY = unit.transform.eulerAngles.y;
                            tiledConfig.moveType = unit.moveType;
                            break;
                    }
                    tiledConfigRow.tiledConfigs.Add(tiledConfig);
                }
                mapConfig.tiledConfigs.Add(tiledConfigRow);
            }
            EditorUtility.SetDirty(LevelDataGlobalConfig.Instance);
            AssetDatabase.SaveAssets();
        }
    }
   
}
