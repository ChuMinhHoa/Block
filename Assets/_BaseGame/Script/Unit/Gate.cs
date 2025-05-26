using System;
using System.Collections.Generic;
using _BaseGame.Script.DataConfig;
using _BaseGame.Script.ETC;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _BaseGame.Script.Unit
{
    public enum ColorType
    {
        Red,
        Green,
        Blue,
        Yellow,
        Purple,
        Orange
    }
    public class Gate : InitDataMeshRender<GateDataConfig>
    {
        public ColorType colorType;
        public CheckType checkType;
        public List<Collider> colliders = new();
        public ParticleSystem effect;
        
        private void Start()
        {
            GameController.Instance.AddGate(this);
        }

        public bool IsCanPassGate(UnitBase unit)
        {
            if (unit.colorType != colorType) return false;
            if (!unit.CheckVector(transform.position, checkType, this)) return false;
            return true;
        }

        public bool ContainsCollider(Collider hitInfoCollider)
        {
            //Debug.Log(""hitInfoCollider.gameObject.name);
            return colliders.Contains(hitInfoCollider);
        }

        [Button]
        public void PlayEffect()
        {
            effect.Play();
        }


        [BoxGroup("Init Data")] public Transform mask;
        public override void InitData(GateDataConfig dataConfig)
        {
            base.InitData(dataConfig);
            MyMeshFilter.mesh = dataConfig.mesh;
            var colorData = BlockDataGlobalConfig.Instance.materialData.Find(x => x.colorType == colorType);
            MyMeshRenderer.material = colorData.material;
        }
    }
    
    
    public enum CheckType
    {
        Vertical,
        Horizontal,
    }
}
