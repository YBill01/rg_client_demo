
/*
using Legacy.Client;
using Legacy.Database;
using Legacy.Network;
using Unity.Entities;

namespace Lootbox.Model.States
{
    public class LootBoxReadyState : IState
    {

        private LootBoxModel model;
        public LootBoxReadyState(LootBoxModel lootBehaviour)
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
            model.UserAction.RemoveListener(OnClick);
            model.OpenEvent.Invoke(model);

            NetworkMessage.LootBoxCommand(World.Active.EntityManager, model.Id, LootCommandType.Reward);
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