using System.Collections.Generic;
using System.Linq;
using Legacy.Database;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaNewHeroesViewBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform FrontPanel;
        [SerializeField]
        private RectTransform FrontPanelRect;
        [SerializeField]
        private HorizontalLayoutGroup FrontPanelLayout;
        [SerializeField]
        private Transform BackPanel;
        [SerializeField]
        private HorizontalLayoutGroup BackPanelLayout;
        [SerializeField]
        private HorizontalLayoutGroup MainLayout;
        [SerializeField]
        private RectTransform MainRect;

        private List<ArenaHeroBehaviour> frontHeroes;
        private List<ArenaHeroBehaviour> backHeroes;
        private int newHeroesCount;

        public void Init(List<ushort> binaryData)
        {

            switch (binaryData.Count)
            {
                case 0:
                    return;
                case 1:
                    InstantiateInRange(0, binaryData.Count, BackPanel, binaryData);
                    break;
                default:
                    {
                        var countInHalf = binaryData.Count / 2;
                        var backRowCount = Mathf.FloorToInt(countInHalf);
                        InstantiateInRange(0, backRowCount, BackPanel, binaryData);
                        InstantiateInRange(backRowCount, binaryData.Count, FrontPanel, binaryData);
                        break;
                    }
            }
           var indexes = GetHeroesIndexesOfCurrentArena(binaryData);
            SetLists();

            gameObject.SetActive(true);

            SetNewHeroes(indexes);

            UpdateLayouts();
        }

        private  List<int> GetHeroesIndexesOfCurrentArena(List<ushort> binaryData)
        {
            List<int> indexex =new List<int>();
            if (Settings.Instance.Get<ArenaSettings>().RatingBattlefield((ushort)ClientWorld.Instance.Profile.Rating.current, out BinaryBattlefields binaryArena))
            {
                for (int i = 0; i < binaryData.Count; i++)
                {
                    if (binaryArena.heroes.Contains(binaryData[i]))
                        indexex.Add(binaryData[i]);

                }
            }

            return indexex;
        }

        public void SetNewHeroes(List<int> indexes)
        {
            var allHeroes = frontHeroes.Union(backHeroes);
            var newHeroes = allHeroes.Where(x => x.IsNewHero(indexes)).ToList();

            newHeroes.ForEach((x) => x.SetIndexer((ushort)newHeroes.IndexOf(x)));
        }

        private void InstantiateInRange(int start, int end, Transform parent, List<ushort> heroes)
        {
            for (int i = start; i < end; i++)
            {
                var hero = Instantiate(GetPrefab(heroes[i]), parent);
                hero.SetName(GetName(heroes[i]));
                hero.SetColor(GetColor(heroes[i]));
                hero.SetDataToPlayEffect(heroes[i]);
            }
        }

        private ArenaHeroBehaviour GetPrefab(ushort index)
        {
            return VisualContent.Instance.GetHeroVisualData(index).ArenaPrefab;
        }

        private string GetName(ushort index)
        {
            if (Heroes.Instance.Get(index, out BinaryHero hero))
            {
                return Locales.Get(hero.title);
            }

            return "";
        }

        private Color32 GetColor(ushort index)
        {
            if (Heroes.Instance.Get(index, out BinaryHero hero))
            {
                if (ColorUtility.TryParseHtmlString(hero.color, out Color color))
                {
                    return color;
                }
            }

            return new Color32(0, 0, 0, 255);
        }

        private void SetLists()
        {
            frontHeroes = FrontPanel.GetComponentsInChildren<ArenaHeroBehaviour>().ToList();
            backHeroes = BackPanel.GetComponentsInChildren<ArenaHeroBehaviour>().ToList();
        }

        private void UpdateLayouts()
        {
            if (frontHeroes == null) return;

            if (frontHeroes.Count == 0) return;

            LayoutRebuilder.ForceRebuildLayoutImmediate(MainRect);
            var scale = MainRect.localScale.x;
            var frontWidth = 0f;

            for (int i = 0; i < frontHeroes.Count; i++)
            {
                frontWidth += frontHeroes[i].GetWidth() * scale;
            }

            frontWidth += FrontPanelLayout.spacing * (frontHeroes.Count - 1);

            var backWidth = 0f;

            for (int i = 0; i < backHeroes.Count; i++)
            {
                backWidth += backHeroes[i].GetWidth();
            }

            backWidth += BackPanelLayout.spacing * (backHeroes.Count - 1);

            if (frontWidth > backWidth)
            {
                var diff = frontWidth - backWidth;
                MainLayout.padding.right = (int)diff;
            }
        }

        public void SetHeroesState(bool isGrayedOut)
        {
            if (backHeroes == null) return;

            for (int i = 0; i < backHeroes.Count; i++)
            {
                backHeroes[i].MakeGray(isGrayedOut);
            }

            if (frontHeroes == null) return;

            for (int i = 0; i < frontHeroes.Count; i++)
            {
                frontHeroes[i].MakeGray(isGrayedOut);
            }
        }

        public void ShowHeroesEffect() 
        {
            SetHeroesState(true);

            var allHeroes = frontHeroes.Union(backHeroes);
            foreach (var hero in allHeroes)
            {
                hero.ShowEffect();
            }
        }
    }
}