using System.Collections.Generic;
using _BaseGame.Script.Unit;
using Sirenix.Utilities;
using UnityEngine;

namespace _BaseGame.Script.DataConfig
{
    [CreateAssetMenu(fileName = "BlockDataGlobalConfig", menuName = "GlobalConfig/BlockDataGlobalConfig")]
    [GlobalConfig("Resources/GlobalConfig/BlockDataGlobalConfig")]
    public class BlockDataGlobalConfig : GlobalConfig<BlockDataGlobalConfig>
    {
        public List<MaterialData> materialData = new();
        public List<MaterialData> gateMData = new();
        public List<BlockDataConfig> blockDataConfig = new();
        public List<GateDataConfig> gateDataConfigs = new();
        public List<MoveObjData> moveObjData = new();
        public List<ColorConfig> colorData = new();
    }

    [System.Serializable]
    public class MaterialData
    {
        public ColorType colorType;    
        public Material material;
    }
    
    [System.Serializable]
    public class ColorConfig
    {
        public ColorType colorType;    
        public Color color;
    }
    
    [System.Serializable]
    public class BlockDataConfig
    {
        public Vector3 vectorRightDefault;
        public List<Vector2> pointColliders = new();
        public UnitType type;
        public Mesh mesh;
    }
    
    [System.Serializable]
    public class GateDataConfig
    {
        public GateType type;
        public Mesh mesh;
    }
    
    [System.Serializable]
    public class MoveObjData
    {
        public MoveTypeObj type;
        public Mesh mesh;
    }

    public enum GateType
    {
        Gate1,
        Gate2,
        Gate3,
    }
    
    public enum MoveTypeObj
    {
        M1,
        M2,
        M3
    }

    public enum UnitType
    {
        Block1,
        Block2,
        Block3,
        BlockL3,
        BlockL4,
        BlockPlus,
        BlockSquare,
        BlockT1
    }
}
