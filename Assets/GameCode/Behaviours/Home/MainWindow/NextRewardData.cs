using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class NextRewardData : MonoBehaviour
    {
        [SerializeField] private ushort MaxReting;

        private ProfileInstance profile;
        private ushort tutorialArena;
        private byte arenasCount = 0;
        private List<RewardData> rewards=new List<RewardData>();
        private ushort nextReward =0;
        private byte nextArena=0;
        private ushort startReting = 0;
        private NextRewardBehaviour nextRewardBehaviour;
        private struct RewardData
        {
            public ushort idArena;
            public ushort idReward;
            public ushort reward;
        }
        public void Init()
        {
            profile = ClientWorld.Instance.Profile;
            nextRewardBehaviour = GetComponent<NextRewardBehaviour>();
            BinaryList arenasT = Settings.Instance.Get<ArenaSettings>().queue;
            tutorialArena = Settings.Instance.Get<ArenaSettings>().tutorial;
            arenasCount = (byte)arenasT.length;

          /*  BinaryList arenas;
            if (tutorialArena > 0)
            {
                arenas = new BinaryList();
                //arenas.Add(tutorialArena);
                for (int i = 0; i < arenasCount; i++)
                {
                    arenas.Add(arenasT[(byte)i]);
                }
                arenasCount++;
            }
            else
            {
                arenas = arenasT;
            }*/
            
            for (byte i = 0; i < arenasT.length; i++)
            {
                if (profile.Rating.max > startReting)
                {
                    AddRewardInArena(arenasT[i]);
                }
                else
                {
                    if (nextReward == 0)
                    {
                        if (Battlefields.Instance.Get(arenasT[i], out BinaryBattlefields binaryArena))
                        {
                            nextArena = (byte)binaryArena.index;
                        }
                    }

                    break;
                }
                
            }
           // if (rewards.Count > 1)
         //   {
             //   rewards.Reverse();
         //   }
            //Debug.Log("Finish");
        }

        private void AddRewardInArena(ushort v)
        {
            if (Battlefields.Instance.Get(v, out BinaryBattlefields binaryArena))
            {
              //  if (profile.Rating.max > startReting + binaryArena.rating)
              //  {
                    byte index = 0;
                    foreach (var reward in binaryArena.rewards)
                    {
                        if (profile.Rating.max > (startReting+ reward.rating))  
                        {
                            if (!profile.Rating.HasReward(binaryArena.index, (byte)reward.reward))
                            {
                                RewardData rew = new RewardData();
                                rew.idArena = binaryArena.index;
                                rew.idReward = index;
                                rew.reward = reward.reward;
                                rewards.Add(rew);
                            }
                        }
                        else
                        {
                            nextReward = reward.reward;
                            break;
                        }
                        index++;
                    }
                    startReting += binaryArena.rating;
               // }
            }
        }

        public void GetShow()
        {
            bool find = false;
            if (rewards.Count > 0)
            {
                foreach (var rew in rewards)
                {
                    if (!profile.Rating.HasReward(rew.idArena, (byte)rew.idReward))
                    {
                        nextRewardBehaviour.ShowReward(rew.reward,false, true);
                        find = true;
                        break;
                    }
                }
            }
            if (!find)
            {
                if(profile.Rating.max> MaxReting)
                {
                    return;
                }
                if (nextReward > 0)
                {
                    nextRewardBehaviour.ShowReward(nextReward);
                }else if (nextArena > 0)
                {
                    nextRewardBehaviour.ShowReward(nextArena,true);
                }

            }
        }
    }
}