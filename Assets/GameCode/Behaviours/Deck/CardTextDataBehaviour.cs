using Legacy.Client;
using Legacy.Database;
using System;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class CardTextDataBehaviour : MonoBehaviour
    {
        [SerializeField]
        private CardViewBehaviour view;

        [SerializeField]
        private TextMeshProUGUI ManaCost;

        [SerializeField]
        private TextMeshProUGUI Level;

        private uint need;
        private uint have;

        public bool CanUpdate
        {
            get
            {
                return have >= need;
            }
        }

        internal void SetManaCost(byte cost)
        {
            view.EnableManaImage(true);
            ManaCost.text = cost.ToString();
            ManaCost.gameObject.SetActive(true);
        }

        internal void SetProgressBar(byte level, ushort _have)
        {
            SetCount(_have, Levels.Instance.GetCountToUpgradeCard(view.db_index, level, UpgradeCostType.CardsCount));
            SetLevel(level);
        }

        internal void SetCount(uint _have, uint _need)
        {
            have = _have;
            need = _need;
            view.SetStateCanUpdate(CanUpdate, (CardGlowState)Convert.ToInt32(CanUpdate));
            view.ProgressBar.SetSlider(have, need);
        }

        internal void SetLevel(byte level)
        {
            Level.text = level.ToString();
            Level.gameObject.SetActive(level > 0);
        }

        internal void AddCards(uint count)
        {
            have += count;
            view.ProgressBar.SetSlider(have, need);
        }
    }
}
