using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

namespace Legacy.Client
{
    public class HeroesWindowBehaviour : WindowBehaviour
    {
        [SerializeField] public HeroesScrollBehaviour scrollPanel;

        [SerializeField] private GameObject HeroButtonPrefab;
        [SerializeField] private GameObject SmallHeroButtonPrefab;
        [SerializeField] private GameObject SmallHeroSeparatorPrefab;

        private List<HeroPanelBehaviour> panels = new List<HeroPanelBehaviour>();
        private List<SmallHeroButtonBehaviour> smallPanels = new List<SmallHeroButtonBehaviour>();

        [SerializeField] private RectTransform Content;
        [SerializeField] private RectTransform SmallContent;

        public override void Init(Action callback)
        {
            InitData();
            gameObject.SetActive(true);
            gameObject.SetActive(false);
            callback();
            ClientWorld.Instance.Profile.PlayerProfileUpdated.AddListener(UpdateAll);
            UpdateAll();
        }

        protected override void SelfClose()
        {
            gameObject.SetActive(false);
        }

        protected override void SelfOpen()
        {
            scrollPanel.BuildScrollSnaping();
            UpdateSelectedPanel();
            gameObject.SetActive(true);
        }

        private void UpdateSelectedPanel()
        {
            foreach (var panel in panels)
            {
                panel.UpdateSelected();
            }

            foreach (var smallPanel in smallPanels)
            {
                smallPanel.UpdateSelected();
            }
        }
        private void UpdateAll()
        {
            scrollPanel.ClearPanelList();
            var i = 0;
            HeroPanelBehaviour.SiblingExists = 0;
            HeroPanelBehaviour.SiblingOpened = 0;
            HeroPanelBehaviour.SiblingClosed = 0;
            foreach (var panel in panels)
            {
                if (Heroes.Instance.Get(panel.GetHeroOfPanel().index, out BinaryHero binaryHero))
                {
                    BinaryHero binary_hero = binaryHero;
                    if (binary_hero.type != BinaryHeroType.Player) continue;
                    UpdHero(binary_hero, i);
                    i++;
                }
            }
            UpdateSelectedPanel();
            scrollPanel.BuildScrollSnaping();
        }

        internal ushort GetClickedHero()
        {
            return ClickedHero;
        }

        public ushort ClickedHero;

        public void OpenHero(ushort index)
        {
            ClickedHero = index;
            WindowManager.Instance.OpenWindow(childs_windows[0]);
        }

        internal void InitData()
        {
            foreach (var hero_index in Heroes.Instance.SortedByArenaList)
            {
                if (Heroes.Instance.Get(hero_index, out BinaryHero binaryHero))
                {
                    BinaryHero binary_hero = binaryHero;
                    if (binary_hero.type != BinaryHeroType.Player) continue;
                    CreateHero(binary_hero);
                }
            }

            DestroyImmediate(SmallContent.GetChild(SmallContent.childCount - 1).gameObject);
        }

        void CreateHero(BinaryHero binary_hero, bool sep = false)
        {
            var panel = Instantiate(HeroButtonPrefab, Content).GetComponent<HeroPanelBehaviour>();
            var smallPanel = Instantiate(SmallHeroButtonPrefab, SmallContent).GetComponent<SmallHeroButtonBehaviour>();
            smallPanel.separator = Instantiate(SmallHeroSeparatorPrefab, SmallContent);

            panel.Init(binary_hero, this);
            panels.Add(panel);
            smallPanel.Init(binary_hero, panel);
            smallPanels.Add(smallPanel);
            scrollPanel.AddPanelToScrollList(panel, smallPanel, binary_hero.index);//
        }

        void UpdHero(BinaryHero binary_hero, int i)
        {
            var panel = panels[i];
            var smallPanel = smallPanels[i];

            panel.Init(binary_hero, this);
            smallPanel.Init(binary_hero, panel);
            scrollPanel.AddPanelToScrollList(panel, smallPanel, binary_hero.index);//
        }
    }
}
