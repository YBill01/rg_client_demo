using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class MenuArenasBehaviour : MonoBehaviour
    {
        public static MenuArenasBehaviour Instance { get; internal set; }

        public const ushort TUTORIAL_ARENA_ID = 6;

        private Dictionary<ushort, MenuArenaBehaviour> InstantiatedArenas = new Dictionary<ushort, MenuArenaBehaviour>();
        private MenuArenaBehaviour activeArena;
        private BattlefieldInfo activeArenaInfo;

        [SerializeField]
        private RectTransform ArenaContainer;



        void Start()
        {
            Instance = this;
        }

        public void Enable()
        {
            var profile = ClientWorld.Instance.Profile;

            if (profile.IsBattleTutorial)
            {
                ShowArena(TUTORIAL_ARENA_ID);
            }
            else if (Settings.Instance.Get<ArenaSettings>().Index(
				ClientWorld.Instance.Profile.CurrentArena.number,
                out BattlefieldInfo info
            ))
            {
                activeArenaInfo = info;
                ShowArena(activeArenaInfo.binary.index);
            }
        }

        internal void ShowArena(ushort index)
        {
            MenuArenaBehaviour arenaModel;
            if (InstantiatedArenas.TryGetValue(index, out MenuArenaBehaviour arena))
            {
                arenaModel = arena;
            }
            else
            {
                arenaModel = CreateArenaModel(index);
            }
            if (arenaModel == null || activeArena == arenaModel) return;
            if (activeArena != null)
            {
                activeArena.Enable(false);
            }
            activeArena = arenaModel;
            arenaModel.Enable(true);
        }

        GameObject GetArenaPrefab(ushort index)
        {
            return VisualContent.Instance.GetArenaVisualData(index).MenuPrefab;
        }

        public void HideArena()
        {
            if (activeArena != null)
            {
                activeArena.Enable(false);
                activeArena = null;
            }
        }

        public MenuArenaBehaviour GetActiveArena()
        {
            return activeArena;
        }

        public BattlefieldInfo GetActiveArenaInfo()
        {
            return activeArenaInfo;
        }

        public MenuArenaBehaviour CreateArenaModel(ushort index, bool addedToList = true)
        {
            var model = Instantiate(GetArenaPrefab(index), ArenaContainer).GetComponent<MenuArenaBehaviour>();
            if (model == null)
            {
                Debug.LogError("No MenuArenaBehaviour in ArenaMiniaturePrefab! Arena Index: " + index.ToString());
            }

            if (addedToList)
			{
                InstantiatedArenas.Add(index, model);
            }

            return model;
        }
    }
}
