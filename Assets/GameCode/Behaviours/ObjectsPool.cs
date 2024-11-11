using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static Legacy.Client.ObjectPooler;

namespace Legacy.Client
{
    public class ObjectsPool : MonoBehaviour
    {

        public Dictionary<string, List<GameObject>> pooledObjects = new Dictionary<string, List<GameObject>>();
        private List<ObjectPoolItem> itemsToPool;

        internal void Init(List<ObjectPoolItem> itemsToPool)
        {
            this.itemsToPool = itemsToPool;
            pooledObjects = new Dictionary<string, List<GameObject>>();
            foreach (ObjectPoolItem item in itemsToPool)
            {
                if (item.amountToPool > 0)
                {
                    var currentPool = new List<GameObject>();

                    if (pooledObjects.TryGetValue(item.prefab.name, out List<GameObject> pool))
                    {
                        currentPool = pool;
                    }
                    else
                    {
                        pooledObjects.Add(item.prefab.name, currentPool);
                    }
                    for (int i = 0; i < item.amountToPool; i++)
                    {
                        currentPool.Add(InitObject(item.prefab));
                    }
                }
            }
        }


        private GameObject InitObject(GameObject prefab)
        {
            GameObject obj = GameObject.Instantiate(prefab, transform);
            var back = obj.AddComponent<BackToPool>();
            back.parent = transform;
            obj.name = prefab.name;
            obj.SetActive(false);

            return obj;
        }

        public GameObject GetObject(string name)
        {
            if (pooledObjects.TryGetValue(name, out List<GameObject> currentPool))
            {
                for (int i = 0; i < currentPool.Count; i++)
                {
                    if (!currentPool[i].activeSelf && currentPool[i].name == name)
                    {
                        return currentPool[i];
                    }
                }
                if (itemsToPool != null)
                {
                    foreach (ObjectPoolItem item in itemsToPool)
                    {
                        if (item.prefab.name == name)
                        {
                            var obj = InitObject(item.prefab);
                            currentPool.Add(obj);
                            return obj;
                        }
                    }
                }
            }
            Debug.Log("No " + name + " in objects pool.");
            return null;
        }
    }
}
