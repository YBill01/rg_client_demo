using Legacy.Client;
using Legacy.Database;
using Legacy.Network;
using Unity.Entities;
/*
namespace Lootbox.Model.States
{
    public class LootBoxDefaultState : IState
    {

        private LootBoxModel model;
        public LootBoxDefaultState(LootBoxModel model)
        {
            this.model = model;
        }

        public void OnEnter()
        {
            model.UserAction.AddListener(OnStartWorking);
        }

        public void OnLeave()
        {
            model.UserAction.RemoveListener(OnStartWorking);
        }

        public void OnUpdate()
        {
        }

        private void OnStartWorking(LootBoxModel model)
        {
            if (model.IsBlocked) return;
            model.UserAction.RemoveListener(OnStartWorking);
            model.QueueEvent.Invoke(model);

            NetworkMessage.LootBoxCommand(World.Active.EntityManager, model.Id, LootCommandType.Open);
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