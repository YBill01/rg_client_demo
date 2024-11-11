using Unity.Entities;
using Unity.Networking.Transport;
using System.Diagnostics;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ObserverConnectionSystem))]

    public class ObserverReceiveSystem : ComponentSystem
    {
        public long LastCommandElapsedTime => _timer.ElapsedMilliseconds - lastCommandTime;


        private Stopwatch _timer;

        private long lastCommandTime;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ObserverConnectionClient>();
            RequireSingletonForUpdate<ObserverConnectionAuthorization>();

            _timer = new Stopwatch();
            _timer.Start();
        }

        protected override void OnDestroy()
        {
            ObserverConnection.Instance.Dispose();
        }

        protected override void OnUpdate()
        {
            var driver = ObserverConnection.Instance.Driver;

            var _connection = GetSingletonEntity<ObserverConnectionClient>();
            var _client = EntityManager.GetComponentData<ObserverConnectionClient>(_connection);
            var _auth = EntityManager.GetComponentData<ObserverConnectionAuthorization>(_connection);

            driver.ScheduleUpdate().Complete();

            // ping
            if (_client.Status > ObserverPlayerStatus.LoseConnect)
            {
                if (_timer.ElapsedMilliseconds > _client.timer)
                {
                    _client.timer = _timer.ElapsedMilliseconds + 1000;
                    driver.BeginSend(ObserverConnection.Instance.UnreliablePeline, _client.Connection, out DataStreamWriter _writer);
                    _writer.WriteByte((byte)ObserverPlayerMessage.Alive);
                    driver.EndSend(_writer);
                }
            }

            NetworkEvent.Type command;
            while ((command = _client.Connection.PopEvent(driver, out DataStreamReader reader)) != NetworkEvent.Type.Empty)
            {
                lastCommandTime = _timer.ElapsedMilliseconds;

                switch (command)
                {
                    case NetworkEvent.Type.Connect:
                        GameDebug.Log($"NetworkEvent.Type.Connect from observer.");

                        if (_client.Status == ObserverPlayerStatus.Disconnect)
                        {                            
                            Language language = Locales.GetSystemLanguage();
                            PostUpdateCommands.RemoveComponent<ObserverConnectionDisconnected>(_connection);

                            var message = default(NetworkMessageRaw);
                            message.Write((byte)ObserverPlayerMessage.Authorization);
                            message.Write(_auth.name);
                            message.Write(_auth.device_id);
                            message.Write(_auth.device_model);
                            message.Write(_auth.operating_system);
                            message.Write(_auth.memory_size);
                            message.Write((byte)language);
                            message.Send(driver, ObserverConnection.Instance.ReliablePeline, _client.Connection);
                        }
                        _client.Status = ObserverPlayerStatus.Connected;

                        break;

                    case NetworkEvent.Type.Disconnect:
                        {
                            UnityEngine.Debug.LogError("NetworkEvent.Type.Disconnect from observer");
                            _client.Status = ObserverPlayerStatus.LoseConnect;
                            PostUpdateCommands.RemoveComponent<ObserverConnectionClient>(_connection);
                            PostUpdateCommands.AddComponent(_connection, default(ObserverConnectionRequest));
                        }
                        break;

                    case NetworkEvent.Type.Data:

                        var _protocol = (ObserverPlayerMessage)reader.ReadByte();

                        switch (_protocol)
                        {
                            case ObserverPlayerMessage.Alive:
                                break;

                            default:
                                {
                                    var _message = default(NetworkMessageRaw);
                                    _message.Write(reader);
                                    _message.size = 0;

                                    var _entity = EntityManager.CreateEntity();
                                    EntityManager.AddComponentData(_entity, new NetworkMessageHelper
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
    }
}

