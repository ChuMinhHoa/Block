using System.Collections.Generic;
using _BaseGame.Script.Unit;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace _BaseGame.Script.DataConfig
{
    [CreateAssetMenu(fileName = "LevelDataGlobalConfig", menuName = "GlobalConfig/LevelDataGlobalConfig")]
    [GlobalConfig("Resources/GlobalConfig/LevelDataGlobalConfig")]
    public class LevelDataGlobalConfig : GlobalConfig<LevelDataGlobalConfig>
    {
        public List<MapConfig> mapConfigs;
    }

    [System.Serializable]
    public class MapConfig
    {
        public int level;
        public List<TiledConfigRow> tiledConfigs = new();
    }
    
    [System.Serializable]
    public class TiledConfigRow
    {
        public List<TiledConfig> tiledConfigs = new();
    }

    [System.Serializable]
    public class TiledConfig
    {
        public TiledType tiledType;
        
        [ShowIf("tiledType", TiledType.Unit)]
        public UnitType unitType; 
        [ShowIf("tiledType", TiledType.Unit)]
        public ArrowType arrowType;
        [ShowIf("tiledType", TiledType.Unit)]
        public Vector3 arrowPosition;
        [ShowIf("tiledType", TiledType.Unit)]
        public MoveType moveType;
        [ShowIf("tiledType", TiledType.Unit)] 
        public BlockSpecialType blockSpecialType;
        
        [ShowIf("tiledType", TiledType.Gate)]
        public GateType gateType;
        [ShowIf("tiledType", TiledType.Gate)]
        public CheckType checkType;
        
        [ShowIf("@tiledType == TiledType.Unit || tiledType == TiledType.Gate")]
        public ColorType colorType;
        
        [ShowIf("tiledType", TiledType.Block)]
        public WallType wallType;
        [ShowIf("tiledType", TiledType.Block)]
        public Vector3 scale = Vector3.one;

        public float rotateY;
      
    }
    
    public enum BlockSpecialType
    {
        None,
        Chain,
        Star,
        Free
    }
    
    public enum TiledType{
        None,
        Block,
        Plane,
        Gate,
        Unit
    }
}
