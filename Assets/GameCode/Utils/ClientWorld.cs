using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Legacy.Client
{

    public class ClientWorld : DefaultWorld
    {
        public static ClientWorld Instance;
        private bool _is_local = false;
        public bool isSandbox { get; private set; }
        public StateMachineSystem stateMachineSystem { get { return GetExistingSystem<StateMachineSystem>(); }  }
        public ProfileInstance Profile { get { return GetExistingSystem<HomeSystems>().UserProfile; }  }
        public void ClearWorld()
        {
            //EntityManager.DestroyEntity(EntityManager.UniversalQuery);
            //GetExistingSystem<Legacy.Server.BattleBucketsSystem>().ClearArrays();
        }
        public ClientWorld(string name) : base(name)
        {
            Instance = this;

            var device_info = new PlayerProfileDevice
            {
                device_id = SystemInfo.deviceUniqueIdentifier,
                device_model = SystemInfo.deviceModel,
                operating_system = SystemInfo.operatingSystem,
                memory_size = SystemInfo.systemMemorySize
            };
            UnityEngine.Debug.Log("ObserverConnect add ClientWorld");
            Instance.ObserverConnect(device_info, SystemInfo.deviceName);

            var _simulation = CreateSystem<SimulationSystemGroup>();
            _simulation.AddSystemToUpdateList(CreateSystem<BeginSimulationEntityCommandBufferSystem>());
            _simulation.AddSystemToUpdateList(CreateSystem<EndSimulationEntityCommandBufferSystem>());
        }

        public void AddSystems(IReadOnlyList<Type> systems)
        {
            var result = new List<Type>();
            for (int i = 0; i < systems.Count; ++i)
            {
                var _system = systems[i];
                if (_system.Namespace == "Legacy.Client")
                {
                    result.Add(_system);
                }
            }
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(this, result);
        }

        public void ObserverConnect(PlayerProfileDevice device_info, string name)
        {
            var connection = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ObserverConnectionAuthorization>());
            if (!connection.IsEmptyIgnoreFilter)
                return;

            var _request = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_request, default(ObserverConnectionRequest));
            EntityManager.AddComponentData(_request, new ObserverConnectionAuthorization
            {
                name = name,
                device_id = new FixedString64(device_info.device_id),
                device_model = new FixedString64(device_info.device_model),
                operating_system = new FixedString64(device_info.operating_system),
                memory_size = device_info.memory_size
            });
        }

        public void StartRatingBattle()
        {
            _is_local = false;
            isSandbox = false;
        }

        public void StartSandbox(ObserverBattle battle)
        {
            StartLocalBattle(battle);
            isSandbox = true;
        }

        public void StartLocalBattle(ObserverBattle battle)
        {
            _is_local = true;
            isSandbox = false;

            var _battle_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_battle_entity, battle);
        }

        public void LocalBattlePlayerReady()
        {
            var _ready_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_ready_entity, default(BattlePlayerReadyRequest));
        }

        public void StartLocalBattle()
        {
            var _ready_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_ready_entity, default(TutorialBattleReadyRequest));
        }

        public void SceneReady()
        {
            var _message = default(NetworkMessageRaw);
            _message.Write((byte)PlayerGameMessage.PlayerPrepare);

            var _message_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_message_entity, _message);
            EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
        }

        public void PlayerReady()
        {
            var _message = default(NetworkMessageRaw);
            _message.Write((byte)PlayerGameMessage.PlayerReady);

            var _message_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_message_entity, _message);
            EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
        }
        public void TutorialTapEvent()
        {
            var _message = default(NetworkMessageRaw);
            _message.Write((byte)PlayerGameMessage.TutorialTap);

            var _message_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_message_entity, _message);
            EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
        }
        public void RequestForMinionInSandbox(ushort minionDb, float hp, ushort dmg, ushort duration, float aggro, ushort hit, ushort bulletspeed, float speed, float collider, ushort countInSquad, byte lvl)
        {
            var _message = default(NetworkMessageRaw);
            _message.Write((byte)PlayerGameMessage.SandboxMinionRequest);
            _message.Write(minionDb);
            _message.Write(hp);
            _message.Write(dmg);
            _message.Write(duration);
            _message.Write(aggro);
            _message.Write(hit);
            _message.Write(bulletspeed);
            _message.Write(speed);
            _message.Write(collider);
            _message.Write(countInSquad);
            _message.Write(lvl);
            var _message_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_message_entity, _message);
            EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
        }
        public void ExitSandbox()
        {
            var _message = default(NetworkMessageRaw);
            _message.Write((byte)PlayerGameMessage.ExitSandbox);
            var _message_entity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(_message_entity, _message);
            EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
        }

        public void ActionPlay(PlayerGameMessage type, byte index, float2 position)
        {

            UnityEngine.Debug.Log($"ActionPlay: index: {index}");
            //if (_is_local)
            //{
            //    // duplicate from GameServerSystem 
            //    // TODO: rewrite ??
            //    var _action = EntityManager.CreateEntity();
            //    EntityManager.AddComponentData(_action, new BattlePlayerAction
            //    {
            //        type = type == PlayerGameMessage.ActionCard ? BattlePlayerActionType.Card : BattlePlayerActionType.Skill,
            //        index = index,
            //        position = position
            //    });
            //} 
            // else
            {
                var _message = default(NetworkMessageRaw);
                _message.Write((byte)type);
                _message.Write(index);
                _message.Write(position.x);
                _message.Write(position.y);

                var _message_entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(_message_entity, _message);
                EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
            }
        }

        public void ActionPlaySkill(byte skill_index, float2 position)
        {
            AnalyticsManager.Instance.SkillUsed(skill_index);

            //if (_is_local)
            //{
            //    var _action = EntityManager.CreateEntity();
            //    EntityManager.AddComponentData(_action, new BattlePlayerAction
            //    {
            //        type = BattlePlayerActionType.Skill,
            //        index = skill_index,
            //        position = position
            //    });
            //} 
            // else 
            {
                var _message = default(NetworkMessageRaw);
                _message.Write((byte)PlayerGameMessage.ActionSkill);
                _message.Write(skill_index);
                _message.Write(position.x);
                _message.Write(position.y);

                var _message_entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(_message_entity, _message);
                EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
            }
        }

        public void ActionPlayRage()
        {
            //if (_is_local)
            //{
            //    //var _action = EntityManager.CreateEntity();
            //    //EntityManager.AddComponentData(_action, new BattlePlayerAction
            //    //{
            //    //    type = BattlePlayerActionType.Rage
            //    //});
            //}
            //else
            {
                var _message = default(NetworkMessageRaw);
                _message.Write((byte)PlayerGameMessage.ActionRage);

                var _message_entity = EntityManager.CreateEntity();
                EntityManager.AddComponentData(_message_entity, _message);
                EntityManager.AddComponentData(_message_entity, default(ServerMessageTag));
            }
        }
    }
}
