/*

namespace Lootbox.Model.States
{
    public class LootBoxQueueState : IState
    {

        private LootBoxModel model;
        public LootBoxQueueState(LootBoxModel lootBehaviour)
        {
            this.model = lootBehaviour;
        }

        public void OnEnter()
        {
            model.UserAction.AddListener(OnClick);
        }

        public void OnLeave()
        {
            model.UserAction.RemoveListener(OnClick);
        }

        public void OnUpdate()
        {
        }

        private void OnClick(LootBoxModel model)
        {
            //model.UserAction.RemoveListener(OnStartWorking);
            //model.QueueEvent.Invoke(model);
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