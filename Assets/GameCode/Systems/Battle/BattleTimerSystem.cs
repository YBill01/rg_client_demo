using Unity.Entities;
using System.Collections.Generic;
using Unity.Collections;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleSimulation))]

    public class BattleTimerSystem : ComponentSystem
    {
        private const int animDuration = 3900;
        private EntityQuery _query_timer;
        private List<uint> times = new List<uint>();
        private bool firstShow = true;

        protected override void OnCreate()
        {
            _query_timer = GetEntityQuery(
                ComponentType.ReadOnly<BattleInstance>()
            );
        }

        protected override void OnUpdate()
        {
            if (BattleInstanceInterface.instance == null) return;
            UpdateTime();
            if (times.Count == 0)
                //times = new List<uint>() { 120000, 60000, 10000 };
                times = new List<uint>() { 121000, 61000, 11900 };
        }

        private void UpdateTime()
        {
            if (_query_timer.IsEmptyIgnoreFilter) return;
            NativeArray<BattleInstance> battles = _query_timer.ToComponentDataArray<BattleInstance>(Allocator.TempJob);
            for (int i = 0; i < battles.Length; i++)
            {
                BattleInstance battle = battles[i];
                AlertsBehaviour.Instance.GetBattle(battle);
                if (battle.status == BattleInstanceStatus.Playing || battle.status == BattleInstanceStatus.Pause)
                {
                    BattleInstanceInterface.instance.TimerText.SetTime(battle.timer);
                    float currentTime = (float)Time.ElapsedTime;

                    ShowAdditionalTimeAlert(battle);

                    foreach (uint time in times)
                    {
                        if (battle.timer <= time)
                        {
                            if (times.Count == 1)
                            {
                                AlertsBehaviour.Instance.ShowAlert(((uint)(battle.timer / 1000f)).ToString(), ref battle);
                            }

                            //var alertEntity = EntityManager.CreateEntity();
                            //EntityManager.AddComponentData(alertEntity, new AlertTypeData { alertType = AlertType.LeftTime });
                            //EntityManager.AddComponentData(alertEntity, new AlertTimeLeft { TimeLeft = battle.timer / 1000 });
                            //EntityManager.AddComponentData(alertEntity, new DelayedEntityKillComponent { DieTime = currentTime + 5 });
                            times.Remove(time);
                            break;
                        }
                    }
                }

                if (battle.status == BattleInstanceStatus.Prepare)
                {
                    StartBattleAnimation.instance.SetTimeBeforeTimer(battle.timer - animDuration);
                }
                if (battle.status == BattleInstanceStatus.FastKillingHeroes)
                {
                    ShowRecountAlert();
                }
                if (battle.status > BattleInstanceStatus.FastKillingHeroes)
                {
                    AlertsBehaviour.Instance.StopAlerts();
                }
            }
            battles.Dispose();
        }

        private void ShowRecountAlert()
        {
            if (!firstShow)
            {
                AlertsBehaviour.Instance.StopAlerts();
                AlertsBehaviour.Instance.ShowAlertQueue(new string[1] { "locale:1138" }, new int[1] { 0 }, 2f);
                firstShow = true;
            }
        }

        private void ShowAdditionalTimeAlert(BattleInstance battle)
        {
            if (battle.isAdditionalTime && firstShow)
            {
                AlertsBehaviour.Instance.StopAlerts();
                AlertsBehaviour.Instance.ShowAlertQueue(new string[2] { "locale:1126", "locale:1120" }, new int[2] { 0, 0 }, 2f);
                firstShow = false;
            }
        }
    }
}
