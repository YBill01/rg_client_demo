using Unity.Collections;
using Unity.Entities;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using System.Diagnostics;
using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ObserverReceiveSystem))]

    public class ObserverReachableSystem : ComponentSystem
    {
        public static bool HardDisconect = false;

        private Stopwatch _timer_focused;
        private ObserverReceiveSystem _observerReceiveSystem;

        private long lastFocusTime;
        private bool unfocused;

        protected override void OnCreate()
        {
            _observerReceiveSystem = World.GetOrCreateSystem<ObserverReceiveSystem>();

            RequireSingletonForUpdate<ObserverConnectionClient>();

            _timer_focused = new Stopwatch();
            _timer_focused.Start();
        }

        protected override void OnDestroy()
        {
            ObserverConnection.Instance.Dispose();
        }

        protected override void OnUpdate()
        {
            var _connection = GetSingletonEntity<ObserverConnectionClient>();
            var _client = EntityManager.GetComponentData<ObserverConnectionClient>(_connection);
            var _auth = EntityManager.GetComponentData<ObserverConnectionAuthorization>(_connection);

            //Игра думает что фокус не пропадал если
            if (                
                Application.isFocused || //игра в фокусе
                TouchScreenKeyboard.visible || //включена нативная клавиатура
                IAPManager.InPayment || //делается покупка
                NameWindowBehaviour.IsInputFocused //активно поле для ввода имени игрока
            )
            {
                lastFocusTime = _timer_focused.ElapsedMilliseconds;
            }
                
            if (_client.Status > ObserverPlayerStatus.LoseConnect)
            {
                var timeDeltaFocus = _timer_focused.ElapsedMilliseconds - lastFocusTime;
                var timeDelta = _observerReceiveSystem.LastCommandElapsedTime;
                    
                ConnectionLostBehaviour.ShowLostConnection(timeDelta > 3000 || Application.internetReachability == NetworkReachability.NotReachable);

                if (HardDisconect)
                {
                    HardDisconect = false;
                    PostUpdateCommands.AddComponent(_connection, new ObserverDisconnectRequest());
                    return;
                }

                if (!NameWindowBehaviour.IsInputFocused && !IAPManager.InPayment)
                {
                    if (timeDelta > 10000)
                    {
                        PostUpdateCommands.AddComponent(_connection, new ObserverDisconnectRequest());
                    }
#if !UNITY_EDITOR
                    else
                    if (unfocused || timeDeltaFocus > 20000)
                    {
                        Application.runInBackground = false;
                        unfocused = true;
                        if (Application.isFocused)
                        {
                            Application.runInBackground = true;
                            unfocused = false;
                            PostUpdateCommands.AddComponent(_connection, new ObserverDisconnectRequest());
                        }
                    }
#endif
                }
            }            
        }
    }
}

