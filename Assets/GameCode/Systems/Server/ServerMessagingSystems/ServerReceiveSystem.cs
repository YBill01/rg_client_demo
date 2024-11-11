using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Legacy.Database;
using Legacy.Client;
using UnityEngine;
using Legacy.Server;

namespace Legacy.Client
{
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(BattleSimulation))]
	[UpdateAfter(typeof(ServerConnectionSystem))]

	public class ServerReceiveSystem : ComponentSystem
    {
        private EntityQuery _query_connection;
        private BattleSystems _battle;

        protected override void OnCreate()
        {
            _query_connection = GetEntityQuery(
                ComponentType.ReadWrite<ServerConnectionClient>(),
                ComponentType.ReadOnly<ServerConnectionRequest>(),
                ComponentType.ReadOnly<ServerConnectionAuthorization>(),
				ComponentType.Exclude<ServerDisconnectRequest>()			
			);

            _battle = ClientWorld.Instance.GetOrCreateSystem<BattleSystems>();
        }

        public static void StateMachineMessage(NetworkMessageHelper message)
        {
            var _entity = ClientWorld.Instance.EntityManager.CreateEntity();
            ClientWorld.Instance.EntityManager.AddComponentData(_entity, message);
        }

        protected override void OnUpdate()
        {
            if (_query_connection.IsEmptyIgnoreFilter)
                return;

            var driver = ServerConnection.Instance.Driver;
			driver.ScheduleUpdate().Complete();

            var _connection = _query_connection.GetSingletonEntity();
            var _client = EntityManager.GetComponentData<ServerConnectionClient>(_connection);
            var _auth = EntityManager.GetComponentData<ServerConnectionAuthorization>(_connection);

            // ping
            if (_client.status > PlayerGameStatus.Disconnected)
            {
                if (_battle.CurrentTime > _client.alive)
                {
                    _client.alive = _battle.CurrentTime + 200;
					driver.BeginSend(ServerConnection.Instance.UnreliablePipeline, _client.connection, out DataStreamWriter _writer);
                    _writer.WriteByte((byte)PlayerGameMessage.Alive);
                    driver.EndSend(_writer);
                }
            }

            NetworkEvent.Type command;
            while ((command = _client.connection.PopEvent(driver, out DataStreamReader reader)) != NetworkEvent.Type.Empty)
            {
                switch (command)
                {
                    case NetworkEvent.Type.Connect:
                        _client.status = PlayerGameStatus.Connected;
                        UnityEngine.Debug.Log("[Player < Game] >> NetworkEvent.Type.Connect");
                        PostUpdateCommands.RemoveComponent<ServerConnectionDisconnected>(_connection);

                        // auth request
                        var _auth_request = default(NetworkMessageRaw);
                        _auth_request.Write((byte)PlayerGameMessage.Authorization);
                        _auth_request.Write(_auth.player);
                        _auth_request.Send(driver, ServerConnection.Instance.ReliablePipeline, _client.connection);
						UnityEngine.Debug.Log("Auth");
						break;

                    case NetworkEvent.Type.Disconnect:
                        {
                            UnityEngine.Debug.Log("NetworkEvent.Type.Disconnect >> " + _battle.CurrentTime);
                            // reconnect ??
                            PostUpdateCommands.AddComponent(_connection, new ServerDisconnectRequest() 
							{
								protocol = (byte)PlayerGameMessage.Disconnect 
							});
                        }
                        break;

                    case NetworkEvent.Type.Data:

                        var _protocol = (PlayerGameMessage)reader.ReadByte();

                        var _message = default(NetworkMessageRaw);
                        _message.Write(reader);
                        _message.size = 0;

                        switch (_protocol)
                        {
							case PlayerGameMessage.Authorization:
								_client = ProcessAuthorization(_client, _protocol, _message);
								break;

							case PlayerGameMessage.BattleInstance:
								_client = ProcessBattleInstance(_client, _protocol, _message);
								break;

                            case PlayerGameMessage.BattleFinish:
                                UnityEngine.Debug.Log("[Player < Game] >> PlayerGameMessage.BattleFinish >> Disconnect");
                                PostUpdateCommands.AddComponent(_connection, new ServerDisconnectRequest()
								{
									protocol = (byte)_protocol,
									data = _message
								});
                                break;

							case PlayerGameMessage.Snapshot:
								ProcessSnapshot(_client, _message);
								break;
							case PlayerGameMessage.TutorialSnapshot:
								ProcessTutorialSnapshot(_client, _message);
								break;
							default:
                                {
                                    StateMachineMessage(new NetworkMessageHelper
                                    {
                                        protocol = (byte)_protocol,
                                        data = _message
                                    });
                                }
                                break;
                        }
                        break;
                }
            }

            EntityManager.SetComponentData(_connection, _client);
        }

		private static void ProcessTutorialSnapshot(ServerConnectionClient _client, NetworkMessageRaw _message)
		{
			if (_client.status == PlayerGameStatus.Playing && Application.isFocused)
			{
				//_client.debug = _timer.ElapsedMilliseconds;


				// filter 
				// battle snapshot

				TutorialInstance tutorialInstance = default;
				tutorialInstance.Deserialize(ref _message);

				var _tutorial_snapshot = ClientWorld.Instance.EntityManager.CreateEntity();

				ClientWorld.Instance.EntityManager.AddComponentData(_tutorial_snapshot, new TutorialSnapshot
				{
					instance = tutorialInstance

				});
			}
		}

		private void ProcessSnapshot(ServerConnectionClient _client, NetworkMessageRaw _message)
		{
			if (_client.status == PlayerGameStatus.Playing)
			{
				//_client.debug = _timer.ElapsedMilliseconds;

				var _server_time = _message.ReadLong();
				var _client_time = _server_time - _client.offset;

				// filter 
				if (_battle.CurrentTime - _client_time < 300)
				{
					// battle snapshot
					var _battle = default(BattleInstance);
					_battle.Deserialize(ref _message);

					var _player = default(BattlePlayer);
					_player.Deserialize(ref _message);
					//UnityEngine.Debug.Log($"Get snapshot for player {_player.skill1} {_player.skill2}");

					var _entity = ClientWorld.Instance.EntityManager.CreateEntity();
					ClientWorld.Instance.EntityManager.AddComponentData(_entity, new BattleSnapshot
					{
						instance = _battle,
						player = _player,
						time = _client_time
					});

					//tutorial

					// minions snapshots
					var _minion_message = _message.ReadMessage(true);
					var _minion_count = _minion_message.ReadByte();
					for (byte i = 0; i < _minion_count; ++i)
					{
						var _snapshot = default(MinionSnapshot);
						_snapshot.Deserialize(ref _minion_message);
						_snapshot.time = _client_time;

						if (_snapshot.repl.index > 0)
						{
							var _minion_snapshot = ClientWorld.Instance.EntityManager.CreateEntity();
							ClientWorld.Instance.EntityManager.AddComponentData(_minion_snapshot, _snapshot);
						}
					}
					var _effect_count = _minion_message.ReadByte();
					for (byte i = 0; i < _effect_count; ++i)
					{
						var _snapshot = default(EffectSnapshot);
						_snapshot.Deserialize(ref _minion_message);
						_snapshot.time = _client_time;

						if (_snapshot.database.index > 0)
						{
							var _effect_snapshot = ClientWorld.Instance.EntityManager.CreateEntity();
							ClientWorld.Instance.EntityManager.AddComponentData(_effect_snapshot, _snapshot);
						}
					}



				}
			}
		}

		private ServerConnectionClient ProcessBattleInstance(ServerConnectionClient _client, PlayerGameMessage _protocol, NetworkMessageRaw _message)
		{
			UnityEngine.Debug.Log("[Player < Game] >> PlayerGameMessage.BattleInstance");
			if (_client.status == PlayerGameStatus.Authorized)
			{
				_client.status = PlayerGameStatus.Playing;
				StateMachineMessage(new NetworkMessageHelper
				{
					protocol = (byte)_protocol,
					data = _message
				});
			}

			return _client;
		}

		private ServerConnectionClient ProcessAuthorization(ServerConnectionClient _client, PlayerGameMessage _protocol, NetworkMessageRaw _message)
		{
			if (_client.status == PlayerGameStatus.Connected)
			{
				var _server_time = _message.ReadLong();
				_client.offset = _server_time - _battle.CurrentTime;
				_client.status = PlayerGameStatus.Authorized;

				StateMachineMessage(new NetworkMessageHelper
				{
					protocol = (byte)_protocol,
					data = _message
				});
			}

			return _client;
		}
	}
}

