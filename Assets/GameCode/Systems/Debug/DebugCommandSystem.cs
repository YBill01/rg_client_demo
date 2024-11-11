using Unity.Entities;
using UnityEngine;
using Legacy.Database;

namespace Legacy.Client
{
#if UNITY_EDITOR

    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
	public class DebugCommandSystem : ComponentSystem
	{
		protected override void OnUpdate()
		{
            if (Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    //var _base_settings = Database.Settings.Instance.Get<BaseGameSettings>();
                    var _message = default(NetworkMessageRaw);
                    _message.Write((byte)ObserverPlayerMessage.Campaign);
                    _message.Write(AppInitSettings.Instance.campaign.index);
                    _message.Write(AppInitSettings.Instance.campaign.mission);

                    var _message_entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(_message_entity, _message);
                    EntityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
                } else if (Input.GetKeyDown(KeyCode.W))
                {
                    var _message = default(NetworkMessageRaw);
                    _message.Write((byte)ObserverPlayerMessage.UserCommand);
                    _message.Write((byte)UserCommandType.LootUpdate);
                    _message.Write((byte)LootCommandType.Open);
                    _message.Write((byte)0);

                    var _message_entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(_message_entity, _message);
                    EntityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    var _message = default(NetworkMessageRaw);
                    _message.Write((byte)ObserverPlayerMessage.UserCommand);
                    _message.Write((byte)UserCommandType.LootUpdate);
                    _message.Write((byte)LootCommandType.Reward);

                    var _message_entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(_message_entity, _message);
                    EntityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    var _message = default(NetworkMessageRaw);
                    _message.Write((byte)ObserverPlayerMessage.UserCommand);
                    _message.Write((byte)UserCommandType.ArenaUpdate);
                    _message.Write((byte)ArenaCommandType.Reward);
                    _message.Write((byte)0);
                    _message.Write((byte)1);

                    var _message_entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(_message_entity, _message);
                    EntityManager.AddComponentData(_message_entity, default(ObserverMessageTag));
                }
            }
		}

	}
#endif
}