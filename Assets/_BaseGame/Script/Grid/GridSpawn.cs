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
        [SerializeField] private int gridSizeX = 5;
        [SerializeField] private int gridSizeY = 5;
        [SerializeField] private float spacing = 1.0f;
        [SerializeField] private Vector3 vectorFirst;
        public ObjPool<Transform> planePool = new();
        public ObjPool<Transform> blockPool = new();
        public ObjPool<UnitBase> unitPool = new();
        public ObjPool<Gate> gatePool = new();
        
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

            var positionZ = gridSizeX % 2 == 0 ? (int)(gridSizeX / 2f) - 0.5f : (int)(gridSizeX / 2f);
            var positionX = gridSizeY % 2 == 0 ? (int)(gridSizeY / 2f) - 0.5f : (int)(gridSizeY / 2f);
            
            for (var x = 0; x < gridSizeX; x++)
            {
                for (var y = 0; y < gridSizeY; y++)
                {
                    var position = new Vector3(-positionX + y * spacing, 0, -positionZ + x * spacing);
                    var plane = planePool.Spawn();
                    plane.position = position;
                    switch (mapConfig.tiledConfigs[x].tiledConfigs[y].tiledType)
                    {
                        case TiledType.Block:
                            var block = blockPool.Spawn();
                            block.transform.position = position;
                            break;
                        case TiledType.Plane:
                            break;
                        case TiledType.Gate:
                            var gate = gatePool.Spawn();
                            gate.transform.position = position;
                            break;
                        case TiledType.Unit:
                            var unit = unitPool.Spawn();
                            unit.transform.position = position;
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
        
        private void ClearPool<T>(ObjPool<T> objPool) where T : Component
        {
            if (isEditorMode)
            {
                unitPool.Clear();
                for (var i = objPool.trsParents.childCount-1; i >= 0; i--)
                {
                    var child = objPool.trsParents.GetChild(i);
                    Object.DestroyImmediate(child.gameObject);
                }
                //unitPool.Clear();
            }
            else
            {
                for (var i = objPool.ListActive.Count; i >=0; i--)
                {
                    objPool.Despawn(objPool.ListActive[i]);
                }
            }
        }

        [Button]
        private void SaveBackMapData()
        {
            EditorUtility.SetDirty(LevelDataGlobalConfig.Instance);
            AssetDatabase.SaveAssets();
        }
        
        [BoxGroup("Matrix")]
        [TableMatrix(HorizontalTitle = "Grid Elements", SquareCells = true, DrawElementMethod = "DrawColoredEnumElement")]
        [ShowInInspector]public TiledOnMatrix[,] GridElements;

        [ShowInInspector] public List<GameObject> objShows;

        private TiledOnMatrix DrawColoredEnumElement(Rect rect, TiledOnMatrix value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                int valueIndex = (int)value.tiledConfig.tiledType;
                valueIndex++;
                if (valueIndex>= Enum.GetValues(typeof(TiledType)).Length)
                {
                    valueIndex = 0;
                }
                
                value.objShow = objShows[valueIndex];
                value.tiledConfig.tiledType = (TiledType)valueIndex;
                GUI.changed = true;
                Event.current.Use();
            } 
        
            //Draw the preview of the objShow GameObject
            if (value.objShow != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(value.objShow);
                if (previewTexture != null)
                {
                    GUI.DrawTexture(rect, previewTexture, ScaleMode.ScaleToFit);
                }
            }
            else
            {
                // Draw a placeholder if objShow is null
                EditorGUI.DrawRect(rect, Color.gray);
            }
            return value;
        }
        
        [BoxGroup("Matrix")]
        [Button]
        public void DrawMatrix()
        {
            GridElements = new TiledOnMatrix[gridSizeY, gridSizeX];
            for (var i = 0; i < gridSizeX; i++)
            {
                for (var j = 0; j < gridSizeY; j++)
                {
                    Debug.Log($"{i},{j}");
                    GridElements[j, i] = new TiledOnMatrix();
                    GridElements[j, i].tiledConfig = new TiledConfig();
                    GridElements[j, i].tiledConfig.tiledType = TiledType.Plane;
                    var objIndex= (int)mapConfig.tiledConfigs[i].tiledConfigs[j].tiledType;
                    GridElements[j, i].objShow = objShows[objIndex];
                }
            }
            ReverseMatrixHorizontally();
            ReverseMatrixVertically();
        }
        
        public void ReverseMatrixHorizontally()
        {
            for (int i = 0; i < gridSizeY; i++)
            {
                for (int j = 0; j < gridSizeX / 2; j++)
                {
                    (GridElements[i, j], GridElements[i, gridSizeX - 1 - j]) = (GridElements[i, gridSizeX - 1 - j], GridElements[i, j]);
                }
            }
        }
        
        public void ReverseMatrixVertically()
        {
            for (int i = 0; i < gridSizeY / 2; i++)
            {
                for (int j = 0; j < gridSizeX; j++)
                {
                    (GridElements[i, j], GridElements[gridSizeY - 1 - i, j]) = (GridElements[gridSizeY - 1 - i, j], GridElements[i, j]);
                }
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
