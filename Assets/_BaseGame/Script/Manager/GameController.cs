using System.Collections.Generic;
using _BaseGame.Script.Unit;
using UnityEngine;

namespace _BaseGame.Script
{
    public class GameController : Singleton<GameController>
    {
        public Camera mainCamera;
        public UnitBase currentUnitBase;
        public LayerMask mask;
        public LayerMask planeMask;
        public List<Gate> myGates = new();
        public List<UnitBase> unitBases = new();
        
        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 100,mask))
                {
                    var unitBase = GetUnitBase(hit.collider.transform.parent.gameObject);
                    if (unitBase)
                    {
                        if (Physics.Raycast(ray, out var hitPlane, 100,planeMask))
                            unitBase.SetPointDefault(hitPlane.point);
                        unitBase.PickUp();
                        SetUnitBase(unitBase);
                    }
                }
            }

            if (currentUnitBase)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    currentUnitBase.OnDrop();
                    SetUnitBase(null);
                }
            }
        }

        private void FixedUpdate()
        {
            if (currentUnitBase) 
            {
                if (Input.GetMouseButton(0))
                {
                    var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out var hit, 100,planeMask))
                    {
                        currentUnitBase.MovePosition(hit.point);
                    }
                }
            }
        }

        private void SetUnitBase(UnitBase unitBase)
        {
            currentUnitBase = unitBase;
        }

        public Gate GetGate(GameObject obj)
        {
            for (var i = 0; i < myGates.Count; i++)
            {
                if (myGates[i].gameObject == obj)
                    return myGates[i];
            }

            return null;
        }

        public void AddGate(Gate gate)
        {
            if (!myGates.Contains(gate))
            {
                myGates.Add(gate);
            }
        }
        
        public void ClearAllGates()
        {
            myGates.Clear();
        }
        
        public void AddUnitBase(UnitBase unit)
        {
            if (!unitBases.Contains(unit))
            {
                unitBases.Add(unit);
            }
        }
        
        public void ClearAllUnitBase()
        {
            unitBases.Clear();
        }

        private UnitBase GetUnitBase(GameObject obj)
        {
            for (var i = 0; i < unitBases.Count; i++)
            {
                if (unitBases[i].gameObject == obj)
                    return unitBases[i];
            }

            return null;
        }
    }
}
