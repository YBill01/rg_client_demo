using Unity.Entities;
using Unity.Collections;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]
    public class BattleCardAgroRadiusSystem : ComponentSystem
    {
        private EntityQuery _draggin;
        private EntityQuery _query_minions;

        protected override void OnCreate()
        {
            _query_minions = GetEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.Exclude<MinionHeroTag>(),
                ComponentType.ReadOnly<Transform>()
            );

            _draggin = GetEntityQuery(
                ComponentType.ReadOnly<StartDragBattleCard>()
            );
            RequireForUpdate(_draggin);
            RequireSingletonForUpdate<BattleInstance>();
        }

        protected override void OnUpdate()
        {
            var _battle = GetSingleton<BattleInstance>();
            var _player = _battle.players[_battle.players.player];
            var minions = _query_minions.ToComponentDataArray<MinionData>(Allocator.TempJob);
            var transforms = _query_minions.ToComponentArray<Transform>();
            var drags = _draggin.ToComponentDataArray<StartDragBattleCard>(Allocator.TempJob);
            var entities = _draggin.ToEntityArray(Allocator.TempJob);
            for (int i = 0; i < drags.Length; i++)
            {
                var draggedCardPosition = drags[i].dragPosition;
                if (drags[i].state == 1)
                {
                    for (int j = 0; j < minions.Length; j++)
                    {
                        var minion = minions[j];
                        if (minion.side != _player.side)//!=
                        {
                            var minionPositionInVector = transforms[j].position;
                            if (Vector3.Distance(minionPositionInVector, draggedCardPosition) < drags[i].agroRadius)
                            {
                                if (transforms[j].GetComponent<MinionPanel>())
                                {
                                    transforms[j].GetComponent<MinionPanel>().SetMinionAgro(true);
                                }
                            }
                            else
                            {
                                if (transforms[j].GetComponent<MinionPanel>())
                                {
                                    transforms[j].GetComponent<MinionPanel>().SetMinionAgro(false);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < minions.Length; j++)
                    {
                        var minion = minions[j];
                        if (minion.side != _player.side)//!=
                            if (transforms[j].GetComponent<MinionPanel>())
                            {
                                transforms[j].GetComponent<MinionPanel>().SetMinionAgro(false);
                            }
                    }
                    EntityManager.DestroyEntity(entities[i]);
                }

            }
            minions.Dispose();
            entities.Dispose();
            drags.Dispose();
        }


        protected override void OnDestroy()
        {
            _draggin.Dispose();
        }
    }
}
