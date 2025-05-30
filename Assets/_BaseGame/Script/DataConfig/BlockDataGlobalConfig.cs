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
        public List<BlockMaterialConfig> materialData = new();
        public List<BlockMaterialConfig> gateMData = new();
        public List<BlockDataConfig> blockDataConfig = new();
        public List<GateMeshConfig> gateDataConfigs = new();
        public List<ArrowMeshConfig> moveObjData = new();
        public List<ColorConfig> colorData = new();
        public List<ChainConfig> chainConfigs = new();
    }
    
    [System.Serializable]
    public class ChainConfig
    {
        public UnitType unitType;
        public Mesh mesh;
        public Vector3 rotate;
        public Vector3 position;
    }

    [System.Serializable]
    public class BlockMaterialConfig
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
    public class GateMeshConfig
    {
        public GateType type;
        public Mesh mesh;
    }
    
    [System.Serializable]
    public class ArrowMeshConfig
    {
        public ArrowType type;
        public Mesh mesh;
    }

    public enum GateType
    {
        Gate1,
        Gate2,
        Gate3,
    }
    
    public enum ArrowType
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
