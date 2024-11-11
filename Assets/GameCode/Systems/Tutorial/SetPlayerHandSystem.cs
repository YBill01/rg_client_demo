using Legacy.Database;
using Legacy.Client;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Legacy.Server
{
    [UpdateInGroup(typeof(BattleSimulation))]

    // Эта система переставляет карты в руке игрока захвардкоженым образом 
    // Ниже есть соответсвующие константы

    public class SetPlayerHandSystem : JobComponentSystem
    {
        EntityQuery eventsQuery;
        private EndSimulationEntityCommandBufferSystem barrier;

        private NativeArray<bool> needResetCards;

        protected override void OnCreate()
        {
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            needResetCards = new NativeArray<bool>(1, Allocator.Persistent);
            needResetCards[0] = false;

            eventsQuery = GetEntityQuery(ComponentType.ReadWrite<EventCaptureInstance>());

            RequireForUpdate(eventsQuery);
            RequireSingletonForUpdate<TutorialInstance>();
            RequireSingletonForUpdate<BattleInstance>();
        }

        protected override void OnDestroy()
        {
            needResetCards.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var buffer = barrier.CreateCommandBuffer().AsParallelWriter();
            var battle = GetSingleton<BattleInstance>();
            var battleEntity = GetSingletonEntity<BattleInstance>();

            var _job = new EventJob
            {
                buffer = buffer,
                battle = battle,
                battleEntity = battleEntity,
                needResetCards = needResetCards
            };

            inputDeps = _job.Schedule(eventsQuery, inputDeps);
            inputDeps.Complete();

            TryResetCards();

            return inputDeps;
        }

        private void TryResetCards()
        {
            if (!needResetCards[0])
                return;

            foreach (var card in BattleInstanceInterface.instance.Cards)
            {
                card.TutorialResetCard();
            }
            needResetCards[0] = false;
        }

        struct EventJob : IJobForEachWithEntity<EventCaptureInstance>
        {
            internal EntityCommandBuffer.ParallelWriter buffer;
            internal BattleInstance battle;
            internal Entity battleEntity;
            internal NativeArray<bool> needResetCards;

            private const ushort firstCardIndex = 58;
            private const ushort secondCardIndex = 64;
            private const ushort thirdCardIndex = 57;

            public void Execute(
                Entity entity,
                int index,
                ref EventCaptureInstance eventInstance
            )
            {
                if (eventInstance._event == TutorialEvent.ResetCards)
                {
                    buffer.DestroyEntity(index, entity);
                    needResetCards[0] = true;

                    var player = battle.players[battle.players.player];

                    var firstCard = player.hand[0];
                    if (firstCard.index != firstCardIndex)
                        MixCards(ref player, 0, firstCardIndex);

                    var secondCard = player.hand[1];
                    if (secondCard.index != secondCardIndex)
                        MixCards(ref player, 1, secondCardIndex);

                    var thirdCard = player.hand[2];
                    if (thirdCard.index != thirdCardIndex)
                        MixCards(ref player, 2, thirdCardIndex);

                    battle.players[battle.players.player] = player;

                    buffer.SetComponent(index, battleEntity, battle);
                }
            }

			private static void MixCards(ref BattlePlayer player, int cardToCheck, ushort cardIndex)
			{
				//Ищем ее в руке и меняем местами если нужно
				for (int i = 0; i < BattlePlayerHand.length; i++)
				{
					var card = player.hand[i];
					if (card.index != cardIndex)
						continue;

					var firstCard = player.hand[cardToCheck];
					player.hand[cardToCheck] = card;
					player.hand[i] = firstCard;
					break;
				}

				int loopCounter = 0;
                var cardInHand = player.hand[cardToCheck];
                // Пока не вытащим из колоды нужную карту 
                while (cardInHand.index != cardIndex)
				{
					// берем следующую
					var firstCard = player.hand[cardToCheck];

					var _next_hand = player.hand.Next;
					player.hand[cardToCheck] = _next_hand;

					var _next_deck = player.deck._next();
					player.hand.Next = _next_deck;
					player.deck._free(firstCard);

					// проверка на зацикливание
					loopCounter++;
					if (loopCounter > 10)
					{
						UnityEngine.Debug.LogError("Can not found right card in deck");
                        break;
					}
                    cardInHand = player.hand[cardToCheck];

                }
			}

		}
    }
}
