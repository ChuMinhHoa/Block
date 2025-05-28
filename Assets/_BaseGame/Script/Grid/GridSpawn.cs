using System;
using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using _BaseGame.Script.ETC;
using _BaseGame.Script.Unit;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _BaseGame.Script.Grid
{
    [System.Serializable]
    public class GridSpawn
    {
        public int level = 1;
        public MapConfig mapConfig;
        [SerializeField] public int gridSizeX = 5;
        [SerializeField] public int gridSizeY = 5;
        [SerializeField] public float spacing = 1.0f;
        [SerializeField] private Vector3 vectorFirst;
        public ObjPool<Transform> planePool = new();
        public ObjPool<Transform> blockPool = new();
        public ObjPool<UnitBase> unitPool = new();
        public ObjPool<Gate> gatePool = new();
        public List<Transform> maskDeep = new();
        public bool isEditorMode = true;
        
        [Button]
        public void GetMapConfig()
        {
            mapConfig = LevelDataGlobalConfig.Instance.mapConfigs.Find(x => x.level == level);
            if (mapConfig == null)
            {
                Debug.LogError($"Map config for level {level} not found.");
                return;
            }
            gridSizeX = mapConfig.tiledConfigs.Count;
            gridSizeY = mapConfig.tiledConfigs[0].tiledConfigs.Count;
        }
        
        [Button]
        private void SpawnGrid()
        {
            ClearPool(planePool);
            ClearPool(unitPool);
            ClearPool(gatePool);
            ClearPool(blockPool);
            var pointTemp = maskDeep[0].localPosition;
            pointTemp.z = (-gridSizeX+2) / 2f;
            maskDeep[0].localPosition = pointTemp;
            pointTemp.z *= -1;
            maskDeep[1].localPosition = pointTemp;
            maskDeep[0].localScale = new Vector3(gridSizeY, 1, 20);
            maskDeep[1].localScale = new Vector3(gridSizeY, 1, 20);
            
            pointTemp = maskDeep[2].localPosition;
            pointTemp.x = (-gridSizeY+2) / 2f;
            maskDeep[2].localPosition = pointTemp;
            pointTemp.x *= -1;
            maskDeep[3].localPosition = pointTemp;
            maskDeep[2].localScale = new Vector3(gridSizeX, 1, 20);
            maskDeep[3].localScale = new Vector3(gridSizeX, 1, 20);
            
            var positionZ = gridSizeX % 2 == 0 ? (int)(gridSizeX / 2f) - 0.5f : (int)(gridSizeX / 2f);
            var positionX = gridSizeY % 2 == 0 ? (int)(gridSizeY / 2f) - 0.5f : (int)(gridSizeY / 2f);
            
            for (var x = 0; x < gridSizeX; x++)
            {
                for (var y = 0; y < gridSizeY; y++)
                {
                    var position = new Vector3(-positionX + y * spacing, 0, -positionZ + x * spacing);
                    switch (mapConfig.tiledConfigs[x].tiledConfigs[y].tiledType)
                    {
                        case TiledType.Block:
                            var block = blockPool.Spawn();
                            block.transform.localPosition = position;
                            break;
                        case TiledType.Plane:
                            SpawnPlane(position.x, position.z);
                            break;
                        case TiledType.Gate:
                            var gate = gatePool.Spawn();
                            gate.transform.localPosition = position;
                            gate.gateType = mapConfig.tiledConfigs[x].tiledConfigs[y].gateType;
                            gate.colorType = mapConfig.tiledConfigs[x].tiledConfigs[y].colorType;
                            gate.checkType = mapConfig.tiledConfigs[x].tiledConfigs[y].checkType;
                            gate.transform.eulerAngles = new Vector3(0, mapConfig.tiledConfigs[x].tiledConfigs[y].rotateY, 0);
                            gate.InitData();
                            break;
                        case TiledType.Unit:
                            SpawnPlane(position.x, position.z);
                            var unit = unitPool.Spawn();
                            unit.ResetUnit();
                            unit.transform.localPosition = position;
                            unit.unitType = mapConfig.tiledConfigs[x].tiledConfigs[y].unitType;
                            unit.moveType = mapConfig.tiledConfigs[x].tiledConfigs[y].moveType;
                            unit.colorType = mapConfig.tiledConfigs[x].tiledConfigs[y].colorType;
                            unit.transform.eulerAngles = new Vector3(0, mapConfig.tiledConfigs[x].tiledConfigs[y].rotateY, 0);
                            unit.InitData();
                            break;
                        default:
                            // Handle other types if necessary
                            break;
                    }
                    // var trs = planePool.SpawnEditor();
                    // trs.position = position;
                }
            }
        }
        
        [Button]
        private void DestroyMap()
        {
            ClearPool(planePool);
            ClearPool(unitPool);
            ClearPool(gatePool);
            ClearPool(blockPool);
        }

        private void SpawnPlane(float x, float z)
        {
            var position = new Vector3(x, 0, z);
            var plane = planePool.Spawn();
            plane.localPosition = position;
        }

        private void ClearPool<T>(ObjPool<T> objPool) where T : Component
        {
            if (isEditorMode)
            {
                objPool.Clear();
                for (var i = objPool.trsParents.childCount-1; i >= 0; i--)
                {
                    var child = objPool.trsParents.GetChild(i);
                    Object.DestroyImmediate(child.gameObject);
                }
                //unitPool.Clear();
            }
            else
            {
                    objPool.DespawnAll();
            }
        }

        

    }

    [System.Serializable]
    public class TiledOnMatrix
    {
        public TiledConfig tiledConfig;
        [PreviewField]public GameObject objShow;
    }
}
