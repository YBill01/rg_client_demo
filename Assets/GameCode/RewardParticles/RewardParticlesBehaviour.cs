using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Legacy.Client.LootBoxWindowBehaviour;

namespace Legacy.Client {
    public class RewardParticlesBehaviour : MonoBehaviour
    {
        public static RewardParticlesBehaviour Instance;
        public UnityEvent OnParticleCame = new UnityEvent();

        [Header("Reward Particles")]
        [SerializeField] ParticleToTargetBehaviour Soft;
        [SerializeField] ParticleToTargetBehaviour Hard;
        [SerializeField] ParticleToTargetBehaviour Shards;
        [SerializeField] ParticleToTargetBehaviour Exp;
        [SerializeField] ParticleToTargetBehaviour Stars;
        [SerializeField] ParticleToTargetBehaviour HeroExp;
        [SerializeField] ParticleToTargetBehaviour CardsCommon;
        [SerializeField] ParticleToTargetBehaviour CardsRare;
        [SerializeField] ParticleToTargetBehaviour CardsEpic;
        [SerializeField] ParticleToTargetBehaviour CardsLegendary;
        [SerializeField] ParticleToTargetBehaviour Rating;

        [Space(10), Range(0.0f, 2.0f)]
        [SerializeField] float DurationQueueTime;

        Queue<(ParticleToTargetBehaviour, Vector3, byte)> tupleQueue = new Queue<(ParticleToTargetBehaviour, Vector3, byte)>();
        (ParticleToTargetBehaviour, Vector3, byte) currentTuple;

        private float timer;

        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            if(tupleQueue.Count > 0)
            {
                if(currentTuple.Item1 == null)
                {
                    DropEnqueued();
                    timer = 0.0f;
                }
                else
                {
                    timer += Time.deltaTime;
					if (timer >= DurationQueueTime)
					{
                        DropEnqueued();
                        timer = 0.0f;
                    }

                    /*if(currentTuple.Item1.Finished)
                    {
                        DropEnqueued();
                    }*/
                }
            }
        }

        void DropEnqueued()
        {
            currentTuple = tupleQueue.Dequeue();
            currentTuple.Item1.DropParticles(currentTuple.Item2, currentTuple.Item3, OnParticleCame);
        }

        public void Drop(Vector3 position, byte count, LootCardType type, CardRarity rarity = CardRarity.Common)
        {
            GetParticlesType(type, rarity).DropParticles(position, count, OnParticleCame);
        }

        private ParticleToTargetBehaviour GetParticlesType(LootCardType type, CardRarity rarity = CardRarity.Common)
        {
            ParticleToTargetBehaviour particles = Soft;
            switch (type)
            {
                case LootCardType.Hard:
                    particles = Hard;
                    break;
                case LootCardType.Soft:
                    particles = Soft;
                    break;
                case LootCardType.Shards:
                    particles = Shards;
                    break;
                case LootCardType.Cards:
                    switch (rarity)
                    {
                        case CardRarity.Rare:
                            particles = CardsRare;
                            break;
                        case CardRarity.Epic:
                            particles = CardsEpic;
                            break;
                        case CardRarity.Legendary:
                            particles = CardsLegendary;
                            break;
                        default:
                            particles = CardsCommon;
                            break;
                    }
                    break;
                case LootCardType.Exp:
                    particles = Exp;
                    break;
                case LootCardType.Star:
                    particles = Stars;
                    break;
                case LootCardType.HeroExp:
                    particles = HeroExp;
                    break;
                case LootCardType.Rating:
                    particles = Rating;
                    break;
            }
            return particles;
        }

        public void Queue(Vector3 position, byte count, LootCardType type, CardRarity rarity = CardRarity.Common)
        {
            tupleQueue.Enqueue((GetParticlesType(type, rarity), position, count));
        }
    }
}
