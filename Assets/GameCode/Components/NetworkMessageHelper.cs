using Unity.Entities;
using Unity.Mathematics;
using Legacy.Database;

namespace Legacy.Client
{
	public struct NetworkMessageHelper : IComponentData
	{
		public byte protocol;
		public NetworkMessageRaw data;

		public static void Battle1x1()
		{
			StaticColliders.campaign = false;
            var entityManager = ClientWorld.Instance.EntityManager;

            var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.Matchmaking1x1);
			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
			ClientWorld.Instance.GetExistingSystem<StateMachineSystem>().StartSearchOpponent();
		}
		public static void BattleBotxBot()
		{
			StaticColliders.campaign = false;
            var entityManager = ClientWorld.Instance.EntityManager;

            var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.BattleBotxBot);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
		}

		public static void CancelBattle1x1()
		{
			StaticColliders.campaign = false;

            var entityManager = ClientWorld.Instance.EntityManager;

            var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.MatchmakingCancel);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
			ClientWorld.Instance.GetExistingSystem<StateMachineSystem>().MissionStartEvent.Invoke();
		}

		public static void Campaign(EntityManager entityManager, ushort campaign, ushort mission)
		{
			StaticColliders.campaign = true;
            var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.Campaign);
			_message.Write(campaign);
			_message.Write(mission);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
			ClientWorld.Instance.GetExistingSystem<StateMachineSystem>().MissionStartEvent.Invoke();
		}

		public static void Tutorial(EntityManager entityManager, /*ushort mission, */ushort tutorial)
		{
			StaticColliders.campaign = true;
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.Tutorial);
			_message.Write(tutorial);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
			ClientWorld.Instance.GetExistingSystem<StateMachineSystem>().MissionStartEvent.Invoke();
		}

		public static void Sandbox(EntityManager entityManager/*, ushort mission, ushort tutorial*/)
		{

			StaticColliders.campaign = true;
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.Sandbox);
			//_message.Write((ushort)tutorial);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
			ClientWorld.Instance.GetExistingSystem<StateMachineSystem>().MissionStartEvent.Invoke();
		}

		public static void SceneReady(EntityManager entityManager)
		{
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)PlayerGameMessage.PlayerPrepare);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(GameMessageTag));
		}

		public static void BattleExit(EntityManager entityManager)
		{
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.BattleExit);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
		}

		public static void ActionPlayCard(EntityManager entityManager, byte hand_index, float2 position)
		{
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)PlayerGameMessage.ActionCard);
			_message.Write(hand_index);
			_message.Write(position.x);
			_message.Write(position.y);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(GameMessageTag));
		}

		public static void ActionDebugCard(EntityManager entityManager, ushort index, float2 position)
		{
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)PlayerGameMessage.ActionDebugCard);
			_message.Write(index);
			_message.Write(position.x);
			_message.Write(position.y);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(GameMessageTag));
		}

		public static void ActionPlaySkill(EntityManager entityManager, byte skill_index, float2 position)
		{
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)PlayerGameMessage.ActionSkill);
			_message.Write(skill_index);
			_message.Write(position.x);
			_message.Write(position.y);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(GameMessageTag));
		}        

        public static void LootBoxCommand(EntityManager entityManager, byte index, LootCommandType command)
        {
            var _message = default(NetworkMessageRaw);
            _message.Write((byte)ObserverPlayerMessage.UserCommand);
            _message.Write((byte)UserCommandType.LootUpdate);
            _message.Write((byte)command);
            _message.Write((byte)LootsType.Box);
            _message.Write(index);

            var _message_entity = entityManager.CreateEntity();
            entityManager.AddComponentData(_message_entity, _message);
            entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
        }

		public static void SelectDeckHero(EntityManager entityManager, byte deckIndex, ushort heroSID)
		{
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.UserCommand);
			_message.Write((byte)UserCommandType.DeckUpdate);
            _message.Write((byte)DeckCommandType.Hero);
            //_message.Write(deckIndex);
			_message.Write(heroSID);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
		}

        public static void UpgradeHero(EntityManager entityManager, ushort index)
        {
            var _message = default(NetworkMessageRaw);
            _message.Write((byte)ObserverPlayerMessage.UserCommand);
            _message.Write((byte)UserCommandType.HeroUpdate);
            _message.Write((byte)HeroCommandType.Upgrade);
            _message.Write(index);

            var _message_entity = entityManager.CreateEntity();
            entityManager.AddComponentData(_message_entity, _message);
            entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
        }

        public static void ChangeCardInDeck(EntityManager entityManager, ushort sid1, ushort sid2)
		{			

			var message = new NetworkMessageRaw();
			message.Write((byte)ObserverPlayerMessage.UserCommand);
			message.Write((byte)UserCommandType.DeckUpdate);
            message.Write((byte)DeckCommandType.Modify);
			message.Write(sid1);
			message.Write(sid2);

			var messageEntity = entityManager.CreateEntity();
			entityManager.AddComponentData(messageEntity, message);
			entityManager.AddComponentData(messageEntity, default(ObserverMessageTag));
		}

        public static void ChangeDeck(byte newDeck)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.DeckUpdate);
            message.Write((byte)DeckCommandType.Change);
            message.Write(newDeck);

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

        public static void NextSort(byte sortID)
        {
            var em = World.DefaultGameObjectInjectionWorld.EntityManager;
            var message = new NetworkMessageRaw();
            message.Write((byte)ObserverPlayerMessage.UserCommand);
            message.Write((byte)UserCommandType.DeckUpdate);
            message.Write((byte)DeckCommandType.ChangeSort);
            message.Write(sortID);

            var messageEntity = em.CreateEntity();
            em.AddComponentData(messageEntity, message);
            em.AddComponentData(messageEntity, default(ObserverMessageTag));
        }

		public static void UpdateTutorialState(ushort hard, ushort soft, int menu, ushort scenarioIndex, ushort step, uint player)
		{
			var entityManager = ClientWorld.Instance.EntityManager;
			UnityEngine.Debug.Log($"<color=red>UpdateTutorialState</color>:  hard {hard} || soft {soft} || menu {menu} || scenarioIndex {scenarioIndex} || step {step} || player {player} ");
			var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.TutorialUpdate);
			_message.Write(hard);
			_message.Write(soft);
			_message.Write(menu);
			_message.Write(scenarioIndex);
			_message.Write(step);
			_message.Write(player);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
		}


	
		public static void RequestForUpdatedProfile()
		{
			var entityManager = ClientWorld.Instance.EntityManager;

			var _message = default(NetworkMessageRaw);
			_message.Write((byte)ObserverPlayerMessage.UpdatedProfile);

			var _message_entity = entityManager.CreateEntity();
			entityManager.AddComponentData(_message_entity, _message);
			entityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
		}
	}
}
