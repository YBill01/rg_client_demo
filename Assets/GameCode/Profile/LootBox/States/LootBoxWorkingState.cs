using Legacy.Database;
using System;
using Unity.Entities;
/*
namespace Legacy.Client
{
    public class LootBoxWorkingState : IState
    {

        private LootBoxModel model;

        public LootBoxWorkingState(LootBoxModel model)
        {
            this.model = model;
        }

        public void OnEnter()
        {
            model.UserAction.AddListener(OnClick);
            model.TimerEvent.AddListener(OnTimerUpdate);
        }

        public void OnLeave()
        {
            model.UserAction.RemoveListener(OnClick);
            model.TimerEvent.RemoveListener(OnTimerUpdate);
        }

        public void OnUpdate()
        {
        }

        private void OnTimerUpdate()
        {
            var leftSpan = LeftTime();
            model.TimeUpdateEvent.Invoke(model, leftSpan);
            if (leftSpan.Ticks < 0)
            {
                model.UserAction.RemoveListener(OnClick);
                model.TimerEvent.RemoveListener(OnTimerUpdate);

                model.ReadyEvent.Invoke(model);
            }
        }

        private void OnClick(LootBoxModel model)
        {
            var leftTime = LeftTime();
            int price = (int)Math.Ceiling((leftTime.Milliseconds / 1000f) / Loots.SKIP_PRICE);
            var profile = World.Active.GetExistingSystem<HomeSystems>().UserProfile;
            if (!profile.Stock.CanTake((ushort)CurrencyType.Hard, price)) return;
            profile.Stock.take((ushort)CurrencyType.Hard, price);
            World.Active.EntityManager.AddComponentData(World.Active.EntityManager.CreateEntity(), default(AllStockUpdateRequest));

            model.UserAction.RemoveListener(OnClick);
            model.TimerEvent.RemoveListener(OnTimerUpdate);

            model.SkipEvent.Invoke(model, LeftTime());

            NetworkMessage.LootBoxCommand(World.Active.EntityManager, model.Id, LootCommandType.Skip);
        }

        public TimeSpan LeftTime()
        {
            TimeSpan finalTime = new TimeSpan();
            foreach(var m in model.Queue)
            {
                if (m == model)
                    break;
                finalTime = finalTime.Add(m.OpenTime);
            }
            //var observerNow = DateTime.Now.AddTicks(HomeSystems.ObserverTimeDelta.Ticks);
            TimeSpan finishTime = new TimeSpan(model.StartOpenTime.Ticks + model.OpenTime.Ticks);
            finishTime = finishTime.Add(finalTime);
            return new TimeSpan(finishTime.Ticks - DateTime.Now.Ticks);
        }

        public void MonoThreadEnter()
        {
        }

        public void MonoThreadUpdate()
        {
        }

        public void MonoThreadLeave()
        {
        }

    }
}
*/