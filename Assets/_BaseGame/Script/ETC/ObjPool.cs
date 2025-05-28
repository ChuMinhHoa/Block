using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _BaseGame.Script.ETC
{
    [System.Serializable]
    public class ObjPool<T> where T : Component
    {
        public Transform trsParents;
        public T prefab;

        [ShowInInspector]public Queue<T> tActive = new();
        [ShowInInspector]public Queue<T> tInactive = new();

        [ShowInInspector] public List<T> ListActive => new(tActive);

        [ShowInInspector]
        public List<T> ListInactive() => new(tInactive);

        public T Spawn()
        {
            if (tInactive.Count > 0)
            {
                var obj = tInactive.Dequeue();
                tActive.Enqueue(obj);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                var obj = Object.Instantiate(prefab, trsParents);
                tActive.Enqueue(obj);
                return obj;
            }
        }

        public void Despawn(T obj)
        {
            if (tActive.Contains(obj))
            {
                tActive.Dequeue();
                tInactive.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Object not found in active pool.");
            }
        }
        
        public void DespawnAll()
        {
            while (tActive.Count > 0)
            {
                var obj = tActive.Dequeue();
                tInactive.Enqueue(obj);
                obj.gameObject.SetActive(false);
            }
        }

        public T SpawnEditor()
        {
            if (tInactive.Count > 0)
            {
                var obj = tInactive.Dequeue();
                tActive.Enqueue(obj);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                var obj = PrefabUtility.InstantiatePrefab(prefab, trsParents) as T;
                tActive.Enqueue(obj);
                return obj;
            }
        }

        public void DespawnEditor(T obj)
        {
            if (tActive.Contains(obj))
            { 
                tActive.Dequeue();
                Object.DestroyImmediate(obj.gameObject);
            }
            else
            {
                Debug.LogWarning("Object not found in active pool.");
            }
        }

        public void Clear()
        {
            Debug.Log("Clear");
            Debug.Log(tActive.Count);
            // Deactivate or destroy all active objects
            while (tActive.Count > 0)
            {
                Debug.Log("Dequeue");
                tActive.Dequeue();
            }

            // Clear the inactive queue
            while (tInactive.Count > 0)
            {
                Debug.Log("Dequeue tInactive");
                tInactive.Dequeue();
            }
        }
        
        [Button] 
        private void DeQueueActive()
        {
            while (tActive.Count > 0)
            {
                Debug.Log("Dequeue");
                tActive.Dequeue();
            }
        }
    }
}
