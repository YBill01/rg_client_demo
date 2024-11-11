
/*
namespace Lootbox.Model.States
{
    public class LootBoxEmptyState : IState
    {

        private LootBoxModel model;
        public LootBoxEmptyState(LootBoxModel model)
        {
            this.model = model;
        }

        public void OnEnter()
        {
            model.UserAction.RemoveListener(OnBoxReceive);
        }

        public void OnLeave()
        {
            model.UserAction.RemoveListener(OnBoxReceive);
        }

        public void OnUpdate()
        {
        }

        private void OnBoxReceive(LootBoxModel model)
        {
            model.UserAction.RemoveListener(OnBoxReceive);
            model.ReceiveBoxEvent.Invoke(model);
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