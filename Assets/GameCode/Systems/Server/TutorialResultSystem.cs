using Legacy.Client;
using Legacy.Database;
using System.Diagnostics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Networking.Transport;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleSimulation))]
    public class TutorialResultSystem : JobComponentSystem
    {
        private EntityQuery _query_battles;

        protected override void OnCreate()
        {
            _query_battles = GetEntityQuery(
                    ComponentType.ReadOnly<BattleInstance>()
                );

            RequireForUpdate(_query_battles);
            RequireSingletonForUpdate<TutorialInstance>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_query_battles.IsEmptyIgnoreFilter)
                return inputDeps;

            var battle = GetSingleton<BattleInstance>();
            if (battle.status != BattleInstanceStatus.Result) return inputDeps;

            var battle_entity = GetSingletonEntity<BattleInstance>();
            var player = battle.players[0];
            var tutorial = GetSingleton<TutorialInstance>();


            EntityManager.DestroyEntity(GetSingletonEntity<TutorialInstance>());
            EntityManager.DestroyEntity(battle_entity);

            var profile = ClientWorld.Instance.Profile;


            //Message TutorialUpdate
            //NetworkMessageHelper.UpdateTutorialState(tutorial.index, 0, profile.MenuTutorialState, 0, tutorial.currentTrigger, profile.index);

            return inputDeps;
        }

        void SendAnalytics(byte battleIndex, bool winner)
        {
            switch (battleIndex)
            {
                case 1:
                    if (winner)
                    {
                        AnalyticsManager.Instance.SendTutorialStep(AnalyticTutorialStep.Battle1Win);
                    }
                    break;
                case 2:
                    if (winner)
                    {
                        AnalyticsManager.Instance.SendTutorialStep(AnalyticTutorialStep.Battle2Win);
                    }
                    break;
                case 3:
                    if (winner)
                    {
                        AnalyticsManager.Instance.SendTutorialStep(AnalyticTutorialStep.Battle3Win);
                    }
                    break;
                case 4:
                    if (winner)
                    {
                        AnalyticsManager.Instance.SendTutorialStep(AnalyticTutorialStep.Battle4Win);
                    }
                    break;
                default:
                    break;

                    //case 1:
                    //    AnalyticsManager.Instance.SendTutorialStep(winner ? AnalyticTutorialStep.Battle1Win : AnalyticTutorialStep.Battle1Lose);
                    //    break;
                    //case 2:
                    //    AnalyticsManager.Instance.SendTutorialStep(winner ? AnalyticTutorialStep.Battle2Win : AnalyticTutorialStep.Battle2Lose);
                    //    break;
                    //case 3:
                    //    AnalyticsManager.Instance.SendTutorialStep(winner ? AnalyticTutorialStep.Battle3Win : AnalyticTutorialStep.Battle3Lose);
                    //    break;
                    //case 4:
                    //    AnalyticsManager.Instance.SendTutorialStep(winner ? AnalyticTutorialStep.Battle4Win : AnalyticTutorialStep.Battle4Lose);
                    //    break;
                    //default:
                    //    break;
            }
        }
    }
}
