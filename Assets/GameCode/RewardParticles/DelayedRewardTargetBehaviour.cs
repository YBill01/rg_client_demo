using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class DelayedRewardTargetBehaviour : MonoBehaviour
    {
        private int holdValue = 0;
        public enum RewardType
        {
            Soft,
            Rating,
            Stars,
            Hard
            //Shards
        }

        public void SetDelayed(uint count)
        {
            holdValue = (int)count;
        }

        [SerializeField] RewardType type;
        void Start()
        {
            switch (type)
            {
                case RewardType.Rating:
                    if(BattleDataContainer.Instance.RatingDelta > 0)
                    {
                        holdValue = BattleDataContainer.Instance.RatingDelta;
                    }
                    break;
                case RewardType.Stars:
                    if (BattleDataContainer.Instance.starsWeGot > 0)
                    {
                        holdValue = BattleDataContainer.Instance.starsWeGot;
                    }
                    break;
                case RewardType.Soft:
                    if (BattleDataContainer.Instance.Soft > 0)
                    {
                        holdValue = BattleDataContainer.Instance.Soft;
                    }
                    break;
                default:
                    break;
            }
        }

        public void DropHoldValue()
        {
            holdValue = 0;
        }

        public int CheckHold()
        {
            return holdValue;
        }
    }
}
