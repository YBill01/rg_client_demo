using Legacy.Database;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client
{
    [Serializable]
    public struct KVPair
    {
        public ushort Key;
        public int Value;
    }
    public class ObjectPooler : MonoBehaviour
    {

        public static ObjectPooler instance;

        [SerializeField]
        private byte MinionsPoolMultiplier;

        [SerializeField]
        public KVPair[] exceptionMinions;

        [SerializeField]
        public ObjectsPool Minions;

        [SerializeField]
        private ObjectsPool Effects;

        [SerializeField]
        private List<ObjectPoolItem> effectsPoolObjects;

        void Start()
        {
            instance = this;
            Effects.Init(effectsPoolObjects);
        }

        internal void InitMinion(ushort index, Action callback = null)
        {
            if (Entities.Instance.Get(index, out BinaryEntity entity))
            {
                var pool = new List<GameObject>();
                if (Minions.pooledObjects.TryGetValue(entity.prefab, out List<GameObject> currentPool))
                {
                    pool = currentPool;
                }
                else
                {
                    Minions.pooledObjects.Add(entity.prefab, pool);
                }

                int count = MinionsPoolMultiplier;
                foreach(var e in exceptionMinions)
                {
                    if (e.Key != index) continue;
                    count = e.Value;
                    break;
                }

                for (byte k = 0; k < count; k++)
                {
                    if (callback == null || k < count - 1)
                        LoadMinion(entity, pool);
                    else
                        LoadMinion(entity, pool, callback: (GameObject g) => callback.Invoke());
                }
            }
        }

        private void GetAdditionalUnit(BinaryEntity entity, Action<GameObject> callback)
        {
            Debug.Log("Minion in pool not found. Creating new minion GameObject: " + entity.prefab);

            var obj = new GameObject();
            var pool = new List<GameObject>();
            if (Minions.pooledObjects.TryGetValue(entity.prefab, out List<GameObject> currentPool))
            {
                pool = currentPool;
            }
            else
            {
                Minions.pooledObjects.Add(entity.prefab, pool);
            }
            LoadMinion(entity, pool, obj, callback);
        }

        /*private void LoadMinion(string prefab, List<GameObject> pool, GameObject parent, Action<GameObject> callback)
        {
            var loaded = Addressables.InstantiateAsync(MinionsPath + "/" + prefab + ".prefab", parent.transform);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                GameObject obj = async.Result;
                obj.name = prefab;
                if (AppInitSettings.Instance.EnableColliders)
                {
                    obj.GetComponent<UnderUnitCircle>().ScaleDebugCollider(entity.collider);
                }
                pool.Add(obj);
                callback(obj);
            };
        }*/

        private string MinionsPath = "Minions";
        private void LoadMinion(BinaryEntity entity, List<GameObject> pool, GameObject parent = null, Action<GameObject> callback = null)
        {
            //var loaded = Addressables.InstantiateAsync("NewMinions/" + entity.prefab + ".prefab", Minions.transform);
            var loaded = Addressables.InstantiateAsync(MinionsPath + "/" + entity.prefab + ".prefab", parent == null ? Minions.transform : parent.transform);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                GameObject obj = async.Result;
                obj.SetActive(false);
                if (AppInitSettings.Instance.EnableColliders)
                {
                    var underUnit = obj.GetComponent<UnderUnitCircle>();
                    underUnit.ScaleDebugCollider(entity.collider / obj.transform.localScale.x);
                    var components = Components.Instance.Get<MinionOffence>();
                    if(components.TryGetValue(entity.index, out MinionOffence offence))
                    {
                        underUnit.ScaleDebugAggro(offence.aggro / obj.transform.localScale.x);
                    }
                }
                obj.name = entity.prefab;
                pool.Add(obj);
                callback?.Invoke(obj);
            };
        }

        [Serializable]
        public struct ObjectPoolItem
        {
            public int amountToPool;
            public GameObject prefab;
        }

        /// <summary>
        /// Возвращает gameObject миньона если миньон готов к использованию, и null если нужно дождаться его создания
        /// </summary>
        internal GameObject GetMinion(BinaryEntity entity, Action<GameObject> callback)
        {
            Debug.Log("RequestedFromMinionsPool: " + entity.prefab);
            var obj = Minions.GetObject(entity.prefab);
            if (obj == null)
            {
                GetAdditionalUnit(entity, callback);
                return null;
            }
            else
            {
                if (AppInitSettings.Instance.EnableColliders)
                {
                    var underUnit = obj.GetComponent<UnderUnitCircle>();
                    underUnit.ScaleDebugCollider(entity.collider / obj.transform.localScale.x);
                    var components = Components.Instance.Get<MinionOffence>();
                    if (components.TryGetValue(entity.index, out MinionOffence offence))
                    {
                        underUnit.ScaleDebugAggro(offence.aggro / obj.transform.localScale.x);
                    }
                }
                callback(obj);
                return obj;
            }
        }

        internal GameObject GetEffect(string name)
        {
            Debug.Log("RequestedFromEffectsPool: " + name);
            return Effects.GetObject(name);
        }

        internal void InitDeck(BattlePlayerDeck player_deck, Action lastCallback = null)
        {
            for (byte k = 0; k < player_deck.real_length; ++k)
            {
                var _card = player_deck._get(k);
                if (Cards.Instance.Get(_card.index, out BinaryCard card))
                {
                    for (var j = 0; j < card.entities.Count; j++)
                    {
                        // Если есть колбек для последнего юнита. Для всех не последних Юнитов обычная логика
                        if (lastCallback == null || (k < player_deck.real_length - 1 || j < card.entities.Count - 1))
                            InitMinion(card.entities[j]);
                        else //для последнего передаем колбек
                            InitMinion(card.entities[j], lastCallback);
                    }
                }
            }
        }

        internal void InitHero(ushort hero)
        {
            if (Heroes.Instance.Get(hero, out BinaryHero binary))
            {
                InitMinion(binary.minion);
            }
        }

        internal void MinionBack(GameObject gameObject)
        {
            gameObject.transform.parent = Minions.gameObject.transform;
        }

        public List<string> EffectsNamesList
        {
            get
            {
                List<string> currentList = new List<string>();
                foreach (var e in effectsPoolObjects)
                    currentList.Add(e.prefab.name);
                return currentList;
            }
        }

        public void AddEffect(GameObject prefab)
        {
            AddEffect(2, prefab);
        }
        public void AddEffect(int count, GameObject prefab)
        {
            effectsPoolObjects.Add(new ObjectPoolItem
            {
                amountToPool = count,
                prefab = prefab
            });
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    public class Startup
    {
        /*
        static Startup()
        {
            string assetPath = "Assets/GameResources/Prefabs/Battle/Battle.prefab";
            var dp = Application.dataPath;
            string fullAssetPath = assetPath;
            GameObject contentsRoot = PrefabUtility.LoadPrefabContents(fullAssetPath);
            if (contentsRoot == null) return;

            string read_folder_path = "Assets/GameResources/Prefabs/Effects/Spells";
            var info = new DirectoryInfo(read_folder_path);
            var fileInfo = info.GetFiles();
            var prefab = contentsRoot.GetComponentInChildren<ObjectPooler>(true);

            List<string> currentNames = prefab.EffectsNamesList;

            List<string> found = new List<string>();
            bool haschanges = false;
            foreach (var file in fileInfo)
            {
                if (file.Extension != ".prefab") continue;
                var shortName = file.Name.Split('.')[0];
                if (currentNames.Contains(shortName)) continue;
                haschanges = true;
                var splitList = file.FullName.Split(new string[] { "Assets\\" }, StringSplitOptions.None);
                var fpath = splitList[1];

                StringSplitOptions options = StringSplitOptions.None;
                string[] separator = new string[] { "\\" };
                fpath = String.Join("/", fpath.Split(separator, options));

                var effectPrefabToAdd = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/" + fpath);
                prefab.AddEffect(effectPrefabToAdd);
            }

            if (haschanges)
            {
                PrefabUtility.SaveAsPrefabAsset(contentsRoot, assetPath);
            }
            PrefabUtility.UnloadPrefabContents(contentsRoot);
        }
        */
    }
#endif
}