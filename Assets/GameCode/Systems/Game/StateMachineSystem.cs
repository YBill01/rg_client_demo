using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class OpponentFoundEvent : UnityEvent<ObserverBattlePlayer> { };

    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class StateMachineSystem : ComponentSystem
    {
        private UnityEvent _opponentSearchStartEvent = new UnityEvent();
        private OpponentFoundEvent _opponentFoundEvent = new OpponentFoundEvent();
        private UnityEvent _battleLoadedEvent = new UnityEvent();
        private UnityEvent _battleCancelEvent = new UnityEvent();
        private UnityEvent _missionStartEvent = new UnityEvent();
        private UnityEvent _missionLoaded = new UnityEvent();
        private UnityEvent _homeLoaded = new UnityEvent();

        public UnityEvent MissionStartEvent { get => _missionStartEvent; }
        public UnityEvent MissionLoaded { get => _missionLoaded; }

        class GameScene
        {
            public string Scene { get; private set; }
            public bool isLoaded = false;
            private AsyncOperationHandle<SceneInstance> _async;

            public void Load(string scene, Action<Scene> callback = null)
            {
                if (scene != Scene)
                {
                    Scene = scene;
                    isLoaded = false;

                    UnityEngine.Debug.Log($"Start Loading Scene: {Scene}");

                    _async = Addressables.LoadSceneAsync(scene);
                    LoadingGroup.Instance.SceneLoading = _async;

                    _async.Completed += loading =>
                    {
                        switch (loading.Status)
                        {
                            case AsyncOperationStatus.Failed:
                                UnityEngine.Debug.Log($"Load Scene Failed: {loading.OperationException.Message}");
                                break;

                            case AsyncOperationStatus.Succeeded:
                                {
                                    UnityEngine.Debug.Log($"Load Scene Succeeded: {Scene}");

                                    LoadingGroup.Instance.WaitForSeconds(() =>
                                    {
                                        isLoaded = true;
                                        UnityEngine.Debug.Log($"Wait Scene Succeeded: {Scene}");
                                        if (loading.IsValid())
                                        {
                                            callback?.Invoke(loading.Result.Scene);
                                        }
                                    });

                                    break;
                                }
                        }
                    };
                }
            }

            internal void LogLoading()
            {
                //UnityEngine.Debug.Log($"Scene Loading: {_async.PercentComplete}");
            }
        }

        public enum ClientState
        {
            None,
            Loading,
            MainMenu,
            Decks,
            Heroes,
            Browsing,
            Battle,
            Campaign
        }

        private EntityQuery _query_signals;
        private EntityQuery _query_dispose;
        private EntityQuery _query_observer;

        private StateMachine<ClientState> _state_machine;
        private GameScene _scene;
        private UnityEvent settingsLoadedEvent = new UnityEvent();
        struct EventHash
        {
            public NetworkMessageHelper message;
            public Entity entity;
        }
        private List<EventHash> _events;
        //private BattleSettings _battle_settings;

        private BattleSystems _battle;
        private HomeSystems _home;

        private bool isSettingsLoaded = false;
        public UnityEvent OpponentSearchStartEvent { get => _opponentSearchStartEvent; }
        public OpponentFoundEvent OpponentFoundEvent { get => _opponentFoundEvent; }
        public UnityEvent BattleLoadedEvent { get => _battleLoadedEvent; }
        public UnityEvent BattleCancelEvent { get => _battleCancelEvent; }
        public UnityEvent SettingsLoadedEvent { get => settingsLoadedEvent; }
        public UnityEvent HomeLoaded { get => _homeLoaded; }

        private const string loadingSceneName = "Gameplay/Loading.unity";
        private bool _gameReloading = false;
        private bool isReloadedSession = true;
        public bool gameReloading { get => _gameReloading; }

        protected override void OnCreate()
        {
            _scene = new GameScene();
            _events = new List<EventHash>();
            isReloadedSession = true;
            _state_machine = new StateMachine<ClientState>();

            _state_machine.Add(ClientState.Loading, EnterLoading, UpdateLoading, null);
            _state_machine.Add(ClientState.MainMenu, EnterMainMenu, UpdateMainMenu, ExitMainMenu);
            //_state_machine.Add(ClientState.Campaign, EnterCampaign, UpdateCampaign, ExitCampaign);
            _state_machine.Add(ClientState.Battle, EnterBattle, UpdateBattle, ExitBattle);

            _query_signals = GetEntityQuery(
                ComponentType.ReadOnly<NetworkMessageHelper>()
            );

            _query_observer = GetEntityQuery(
                ComponentType.ReadOnly<ObserverConnectionClient>(),
                ComponentType.ReadOnly<ObserverConnectionAuthorization>()
            );

            _battle = World.GetOrCreateSystem<BattleSystems>();
            _home = World.GetOrCreateSystem<HomeSystems>();
            _state_machine.SwitchTo(ClientState.Loading);
        }

        enum LoadingOperaiton
        {
            LoadSettingsList,
            LoadSettings,
            CheckNeedDownload,
            DownloadAddressables,
            AddressablesDownloaded
        }

        private AsyncOperationHandle _download;
        public bool IsConnectedTooExistedBattle => _state_machine.ConnectedToExistedBattle;
        public void ForceExitBattle(ClientState state = ClientState.Loading)
        {
            ShowLoadingStub(() => { _state_machine.SwitchTo(state); });
        }

        void EnterLoading()
        {
            if (BattleSystems.versionResult > 0)
            {
                GameDebug.Log($"-=- Different Version");
                LoadingGroup.Instance.CreateVersionWindow();
                return;
            }
            if (gameReloading)
            {
                _scene.Load(loadingSceneName, (s) =>
                {
                    {
                        StageComplete(LoadingOperaiton.CheckNeedDownload);
                    }
                });
                return;
            }

            _home.Initialization(false);
            _home.Simulation(false);
            _battle.Initialization(false);
            _battle.Simulation(false);
            _battle.Presentation(false);

            Addressables.InitializeAsync().Completed += op =>
            {
                StageComplete(LoadingOperaiton.LoadSettingsList, op);
            };
        }

        private void StageComplete(LoadingOperaiton type, AsyncOperationHandle handle = default)
        {
            if (handle.IsValid())
            {
                if (handle.Status == AsyncOperationStatus.Failed)
                {
                    UnityEngine.Debug.Log($"t:{Time.ElapsedTime} >> {handle.OperationException.Message}");
                    return;
                }
                UnityEngine.Debug.Log($"t:{Time.ElapsedTime} >> LoadingOperaiton.{type} >> {handle.Status}");
            }

            switch (type)
            {
                case LoadingOperaiton.LoadSettingsList:
                    Addressables.LoadResourceLocationsAsync("settings").Completed += op =>
                    {
                        StageComplete(LoadingOperaiton.LoadSettings, op);
                    };

                    break;

                case LoadingOperaiton.LoadSettings:
                    var _result_list = (IList<IResourceLocation>)handle.Result;
                    var _length = _result_list.Count;
                    for (int k = 0; k < _length; ++k)
                    {
                        Addressables.LoadAssetAsync<SettingObject>(_result_list[k]).Completed += op1 =>
                        {
                            op1.Result.Init();
                            _length--;
                            if (_length == 0)
                            {
                                UnityEngine.Debug.Log("Addressables >> Settings >> Complete");
                                isSettingsLoaded = true;
                                settingsLoadedEvent.Invoke();
                            }
                        };
                    }
                    break;

                case LoadingOperaiton.CheckNeedDownload:
                    Addressables.GetDownloadSizeAsync("server").Completed += op =>
                    {
                        UnityEngine.Debug.Log($"t:{Time.ElapsedTime} >> GetDownloadSize: {op.Result}");
                        StageComplete(LoadingOperaiton.DownloadAddressables, op);
                    };
                    break;

                case LoadingOperaiton.DownloadAddressables:
                    _download = Addressables.DownloadDependenciesAsync("server");
                    _download.Completed += op =>
                    {
                        StageComplete(LoadingOperaiton.AddressablesDownloaded, op);
                    };
                    break;

                case LoadingOperaiton.AddressablesDownloaded:
                    UnityEngine.Debug.Log($"t:{Time.ElapsedTime} >> AddressablesDownloaded: {handle.Status}");
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                        GoToMainMenu();
                    else
                        UnityEngine.Debug.LogError($"t:{Time.ElapsedTime} >> AddressablesDownloaded Failed: {handle.OperationException.Message}");
                    break;
            }
        }

        void LoadingMessage(float message)
        {
            var _slider = UnityEngine.GameObject.Find("DownloadSlider").GetComponent<Slider>();
            if (_slider != null)
            {
                _slider.value = message;
            }

            LoadingGroup.Instance.UpdateLoadingView((int)message, Locales.Get("locale:6677"));
        }

        void UpdateLoading()
        {
            if (_download.IsValid())
            {
                if (_download.PercentComplete != 1)
                {
                    LoadingMessage(_download.PercentComplete * 100);
                }
                else
                {
                    Addressables.Release(_download);
                }
            }


            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (isSettingsLoaded)
                {
                    if (gameReloading)
                    {
                       Locales.Init(ClientWorld.Instance.Profile.playerSettings.language, AppInitSettings.Instance.LocalesDomain);
                        // GoToMainMenu();
                        _gameReloading = false;
                        // return;
                    }

                    while (_events.Count > 0)
                    {
                        var _event = _events[0];
                        _events.RemoveAt(0);

                        switch ((ObserverPlayerMessage)_event.message.protocol)
                        {
                            case ObserverPlayerMessage.Disconnect:
                                //LoginPanel.Instance.Disconnect("Disconnect");
                                break;

                            case ObserverPlayerMessage.Authorization:
                                {
                                    var _profile = new PlayerProfileInstance();
                                    _profile.Deserialize(ref _event.message.data);

                                    IAPManager.Instance.SetPayer(_profile.payer);
                                    Locales.Init(_profile.playerSettings.language, AppInitSettings.Instance.LocalesDomain);
                                    LoadingGroup.Instance.ShowTips();
                                    _home.UserProfile.BuildFromObserverInstance(_profile, true);

                                    AnalyticsManager.Instance.Init();
                                    IAPManager.Instance.Init();
                                    if (_profile.tutorial.hard_tutorial_state < 4)//if tutorial
                                        TutorialMessageBehaviour.InitBattleTriggerSystem();
                                    var _battle_session = _event.message.data.ReadBool();
                                    UnityEngine.Debug.Log("_battle_session " + _battle_session);
                                    if (_battle_session)
                                    {
                                        var _entity = _query_observer.GetSingletonEntity();
                                        var _auth = EntityManager.GetComponentData<ObserverConnectionAuthorization>(_entity);
                                        _auth.player_id = _profile._id;
                                        EntityManager.SetComponentData(_entity, _auth);

                                        var em = ClientWorld.Instance.EntityManager;
                                        var _connect_entity = em.CreateEntity();
                                        em.AddComponentData(_connect_entity, new ServerConnectionRequest
                                        {
                                            ip = _event.message.data.ReadString64(),
                                            port = _event.message.data.ReadUShort()
                                        });
                                        em.AddComponentData(_connect_entity, new ServerConnectionAuthorization
                                        {
                                            player = _profile._id
                                        });

                                        _events.Clear();

                                        _state_machine.SwitchTo(ClientState.Battle);
                                        _state_machine.ConnectedToExistedBattle = true;
                                        ClientWorld.Instance.StartRatingBattle();
                                        break;
                                    }
                                    if (_profile.tutorial.hard_tutorial_state == 0)
                                    {
                                        NetworkMessageHelper.Tutorial(
                                            ClientWorld.Instance.EntityManager,
                                            1
                                        );

                                        _events.Clear();
                                        break;
                                    }

                                    if (_profile.rating.current > 500)
                                    {
                                        StageComplete(LoadingOperaiton.CheckNeedDownload);
                                    }
                                    else
                                    {
                                        GoToMainMenu();
                                    }
                                }
                                break;
                            case ObserverPlayerMessage.Tutorial:
                                {
                                    UnityEngine.Debug.LogError("update loading tutorial0");
                                    // observer
                                    var _observer_battle = default(ObserverBattle);
                                    _observer_battle.Deserialize(ref _event.message.data);

                                    if (Tutorial.Instance.Get(_observer_battle.tutorial.index, out BinaryTutorial _tutorial))
                                    {
                                        if (Missions.Instance.Get(_tutorial.mission, out BinaryMission _mission))
                                        {
                                            if (Battlefields.Instance.Get(_mission.battlefield, out BinaryBattlefields _binary_field))
                                            {
                                                _state_machine.ConnectedToExistedBattle = false;
                                                _state_machine.SwitchTo(ClientState.Battle);

                                                ShowLoadingStub(() =>
                                                {
                                                    _scene.Load(_binary_field.prefabs.client, (Scene scene) =>
                                                    {
                                                        ClientWorld.Instance.StartLocalBattle(_observer_battle);
                                                        //HideLoadingStub();
                                                    });
                                                });
                                            }
                                        }
                                    }
                                    break;
                                }


                            case ObserverPlayerMessage.BattleReady:
                                if (!_query_observer.IsEmptyIgnoreFilter)
                                {
                                    //var _entity = _query_observer.GetSingletonEntity();
                                    //var _auth = EntityManager.GetComponentData<ObserverConnectionAuthorization>(_entity);

                                    var ip = _event.message.data.ReadString64();
                                    var port = _event.message.data.ReadUShort();
                                    GameDebug.Log($"Port: {port}");
                                    byte enemiesLength = _event.message.data.ReadByte();
                                    ObserverBattlePlayer[] enemies = new ObserverBattlePlayer[enemiesLength];
                                    for (byte i = 0; i < enemiesLength; i++)
                                    {
                                        var enemy = default(ObserverBattlePlayer);
                                        enemy.Deserialize(ref _event.message.data);
                                        enemies[i] = enemy;
                                    }

                                    ClientWorld.Instance.StartRatingBattle();

                                    ShowLoadingStub(() =>
                                    {
                                        var _connect_entity = ClientWorld.Instance.EntityManager.CreateEntity();
                                        ClientWorld.Instance.EntityManager.AddComponentData(
                                            _connect_entity,
                                            new ServerConnectionRequest
                                            {
                                                ip = ip,
                                                port = port
                                            }
                                        );
                                        ClientWorld.Instance.EntityManager.AddComponentData(
                                            _connect_entity,
                                            new ServerConnectionAuthorization
                                            {
                                                player = ClientWorld.Instance.Profile.index
                                            }
                                        );

                                        _state_machine.ConnectedToExistedBattle = false;
                                        _state_machine.SwitchTo(ClientState.Battle);

                                    });



                                }
                                break;
                        }
                    }
                }
            }
        }

        private void GoToMainMenu()
        {
            UnityEngine.Debug.Log($"SwitchTo MainMenu");
            _state_machine.SwitchTo(ClientState.MainMenu);
        }

        void EnterMainMenu()
        {
            //TutorialEntityClass.getInstance().CreateTutorialEntity();
            Input.multiTouchEnabled = false;

            _scene.Load("Home", OnMainComplete);
        }

        void ShowLoadingStub(Action onFinish = null)
        {
            LoadingGroup.Instance.ResetView();
            FaderCanvas.Instance.Hide = true;

            UnityAction onHideComplite = null;
            onHideComplite = () =>
            {
                FaderCanvas.Instance.CompleteStateEvent.RemoveListener(onHideComplite);

                LoadingGroup.Instance.gameObject.SetActive(true);
                FaderCanvas.Instance.Hide = false;

                if (onFinish == null)
                    return;

                UnityAction onUnhideComplete = null;
                onUnhideComplete = () =>
                {
                    onFinish.Invoke();
                    FaderCanvas.Instance.CompleteStateEvent.RemoveListener(onUnhideComplete);
                };
                FaderCanvas.Instance.CompleteStateEvent.AddListener(onUnhideComplete);
            };

            FaderCanvas.Instance.CompleteStateEvent.AddListener(onHideComplite);
        }

        void HideLoadingStub()
        {
            FaderCanvas.Instance.Hide = true;

            UnityAction onHideComplete = null;
            onHideComplete = () =>
            {
                FaderCanvas.Instance.CompleteStateEvent.RemoveListener(onHideComplete);

                LoadingGroup.Instance.gameObject.SetActive(false);
                FaderCanvas.Instance.Hide = false;
            };

            FaderCanvas.Instance.CompleteStateEvent.AddListener(onHideComplete);
        }

        private void OnMainComplete(Scene scene)
        {
            Debug.Log("============= State Machine OnMainComplete ================");
            HideLoadingStub();

            _home.Initialization(true);
            _home.Simulation(true);

            WindowManager.Instance.InitWindows(() =>
            {
                WindowManager.Instance.StartMenu();
            });
            SoundManager.Instance.PlayMenuMusic();
            AnalyticsManager.Instance.HomeLoaded();
            _homeLoaded.Invoke();
        }

        void UpdateMainMenu()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                if (_scene.isLoaded)
                {

                    while (_events.Count > 0)
                    {
                        var _event = _events[0];
                        _events.RemoveAt(0);

                        switch ((ObserverPlayerMessage)_event.message.protocol)
                        {
                            case ObserverPlayerMessage.MatchmakingCancel:
                                Debug.Log("Cancel approved");
                                BattleCancelEvent.Invoke();
                                break;

                            case ObserverPlayerMessage.Disconnect:

                                break;
                            case ObserverPlayerMessage.PaymentResult:

                                var paymentResult = default(ObserverPlayerPaymentResult);
                                paymentResult.Deserialize(ref _event.message.data);
                                Debug.Log($"Payment hash: {paymentResult.receiptHash}");
                                if (paymentResult.success)
                                {
                                    Debug.Log($"Payment success: {paymentResult.receiptHash}");
                                    IAPManager.Instance.CompletePurchase(paymentResult.payment);
                                }
                                else
                                {
                                    Debug.Log($"Payment not valid on server: {paymentResult.receiptHash}");
                                    WindowManager.Instance.OpenFailPurchasePopup(paymentResult);
                                }
                                break;
                            case ObserverPlayerMessage.OpenLootResult:
                                var loot = default(PlayerUpdateLootEvent);
                                loot.Deserialize(ref _event.message.data);

                                AnalyticsManager.Instance.CardsReceive(loot);

                                UnityEngine.Debug.Log($"ObserverPlayerMessage.OpenLootResult >> {loot}");
                                if (WindowManager.Instance.CurrentWindow is LootBoxWindowBehaviour)
                                {
                                    var window = WindowManager.Instance.CurrentWindow as LootBoxWindowBehaviour;
                                    window.Bang(loot);
                                }
                                else if (WindowManager.Instance.CurrentWindow is ArenaWindowBehaviour)
                                {
                                    var window = WindowManager.Instance.CurrentWindow as ArenaWindowBehaviour;
                                    if (window.IsClickedReward())
                                    {
                                        window.SendClickedReward(loot);
                                        window.ClearClickedReward();
                                    }
                                }
                                break;

                            case ObserverPlayerMessage.UpdatedProfile:
                                var _profile = new PlayerProfileInstance();
                                _profile.Deserialize(ref _event.message.data);
                                _home.DefaultProfile = _profile;

                                _home.UserProfile.BuildFromObserverInstance(_profile);
                                break;

                            case ObserverPlayerMessage.RequestError:
                                /*var _error = new UserErrorData { };
                                _error.Deserialize(ref _event.message.data);
                                var errEntity = EntityManager.CreateEntity();
                                EntityManager.AddComponentData(errEntity, _error);
                                EntityManager.AddComponentData(errEntity, default(TechnicalErrorMessage));*/
                                break;

                            case ObserverPlayerMessage.Error:
                                ObserverPlayerErrorMessage errorType = default;
                                errorType.Deserialize(ref _event.message.data);
                                WindowManager.Instance.ShowErrorWindow((byte)errorType.error);
                                break;

                            case ObserverPlayerMessage.Campaign:
                                {
                                    // observer
                                    var _observer_battle = default(ObserverBattle);
                                    _observer_battle.Deserialize(ref _event.message.data);

                                    if (Missions.Instance.Get(_observer_battle.campaign.mission, out BinaryMission _mission))
                                    {
                                        if (Battlefields.Instance.Get(_mission.battlefield, out BinaryBattlefields _binary_field))
                                        {
                                            ObserverBattlePlayer[] enemies = new ObserverBattlePlayer[1]
                                            { _observer_battle.player2 };

                                            _scene.Load(_binary_field.prefabs.client, (Scene scene) =>
                                            {
                                                ShowLoadingStub();

                                                var startWindow = WindowManager.Instance.CurrentWindow as BattleStartWindowBehaviour;
                                                startWindow.SetEnemies(enemies, () =>
                                                {
                                                    ClientWorld.Instance.StartLocalBattle(_observer_battle);
                                                    _state_machine.ConnectedToExistedBattle = false;

                                                    _state_machine.SwitchTo(ClientState.Battle);
                                                    startWindow.Close();
                                                });
                                            });
                                        }
                                    }
                                }
                                break;
                            case ObserverPlayerMessage.Tutorial:
                                {
                                    // observer
                                    var _observer_battle = default(ObserverBattle);
                                    _observer_battle.Deserialize(ref _event.message.data);

                                    StartTutorial(_observer_battle);
                                }
                                break;
                            case ObserverPlayerMessage.Sandbox:
                                {
                                    // observer
                                    //var _observer_battle = default(ObserverBattle);
                                    //_observer_battle.Deserialize(ref _event.message.data);
                                    UnityEngine.Debug.LogError(" ObserverPlayerMessage.Sandbox");
                                    var deck = new NativeList<BinaryBattleCard>(Allocator.TempJob);
                                    foreach (var card in Cards.Instance.List)
                                    {
                                        deck.Add(new BinaryBattleCard() { index = card.index, level = 1 });
                                    }
                                    var battleEntity = EntityManager.CreateEntity();
                                    SandboxPlayerDeck sandboxDeck = SandboxPlayerDeck.PrepareDeck(deck);
                                    EntityManager.AddComponentData<SandboxPlayerDeck>(battleEntity, sandboxDeck);
                                    deck.Dispose();

                                }
                                break;

                            case ObserverPlayerMessage.BattleReady:
                                UnityEngine.Debug.Log("[Observer] << ObserverPlayerMessage.BattleReady");
                                if (!_query_observer.IsEmptyIgnoreFilter)
                                {
                                    //var _entity = _query_observer.GetSingletonEntity();
                                    //var _auth = EntityManager.GetComponentData<ObserverConnectionAuthorization>(_entity);

                                    var ip = _event.message.data.ReadString64();
                                    var port = _event.message.data.ReadUShort();
                                    GameDebug.Log($"Update menu Port: {port}");
                                    byte enemiesLength = _event.message.data.ReadByte();
                                    ObserverBattlePlayer[] enemies = new ObserverBattlePlayer[enemiesLength];
                                    for (byte i = 0; i < enemiesLength; i++)
                                    {
                                        var enemy = default(ObserverBattlePlayer);
                                        enemy.Deserialize(ref _event.message.data);
                                        enemies[i] = enemy;
                                    }

                                    ClientWorld.Instance.StartRatingBattle();
                                    isReloadedSession = false;
                                    if (WindowManager.Instance.CurrentWindow.GetType() == typeof(BattleStartWindowBehaviour))
                                    {
                                        var startWindow = WindowManager.Instance.CurrentWindow as BattleStartWindowBehaviour;
                                        //_battleLoadedEvent.AddListener(startWindow.CloseWindow);
                                        startWindow.SetEnemies(
                                            enemies,
                                            () =>
                                            {
                                                ShowLoadingStub(() =>
                                                {
                                                    var _connect_entity = ClientWorld.Instance.EntityManager.CreateEntity();
                                                    ClientWorld.Instance.EntityManager.AddComponentData(
                                                        _connect_entity,
                                                        new ServerConnectionRequest
                                                        {
                                                            ip = ip,
                                                            port = port
                                                        }
                                                    );
                                                    ClientWorld.Instance.EntityManager.AddComponentData(
                                                        _connect_entity,
                                                        new ServerConnectionAuthorization
                                                        {
                                                            player = ClientWorld.Instance.Profile.index
                                                        }
                                                    );

                                                    _state_machine.ConnectedToExistedBattle = false;
                                                    _state_machine.SwitchTo(ClientState.Battle);

                                                });
                                            });
                                    }
                                }
                                break;
                            case ObserverPlayerMessage.MatchmakingError:
                                UnityEngine.Debug.Log("ObserverPlayerMessage.MatchmakingError !!!!");
                                break;
                            case ObserverPlayerMessage.Alive:
                                UnityEngine.Debug.Log("Alive");
                                HideLoadingStub();
                                //ConnectionLostBehaviour.ShowLostConnection(_client.Status == ObserverPlayerStatus.Disconnect);
                                break;
                            case ObserverPlayerMessage.CurrencyChangeEvent:
                                var info = default(ObserverPlayerCurrencyChangeEventInfo);
                                info.Deserialize(ref _event.message.data);

                                if (info.difference > 0)
                                {
                                    AnalyticsManager.Instance.CurrencyEarn(info.difference, info.currencyType, (int)info.source_id, info.changeSourceType);
                                }
                                else if (info.difference < 0)
                                {
                                    AnalyticsManager.Instance.CurrencySpend(info.difference, info.currencyType, (int)info.source_id, info.changeSourceType);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    _scene.LogLoading();
                }
            }
        }

        private void StartTutorial(ObserverBattle _observer_battle)
        {
            if (!Tutorial.Instance.Get(_observer_battle.tutorial.index, out BinaryTutorial _tutorial))
                return;

            if (!Missions.Instance.Get(_tutorial.mission, out BinaryMission _mission))
                return;

            if (!Battlefields.Instance.Get(_mission.battlefield, out BinaryBattlefields _binary_field))
                return;

            ObserverBattlePlayer[] enemies = new ObserverBattlePlayer[1]
            { _observer_battle.player2 };

            var startWindow = WindowManager.Instance.CurrentWindow as BattleStartWindowBehaviour;
            startWindow.DelayedSetEnemies(2.5f, enemies, () =>
            {
                _state_machine.SwitchTo(ClientState.Battle);
                startWindow.Close();

                ShowLoadingStub(() =>
                {
                    _scene.Load(_binary_field.prefabs.client, (Scene scene) =>
                    {
                        //   ServerWorld.Instance.StartLocalBattle(_observer_battle);
                        //HideLoadingStub();
                    });
                });
            });
        }

        private ObserverBattle CreateSandboxBattle(BinaryMission _mission)
        {
            return new ObserverBattle
            {
                //tutorial = battle_tutorial,
                isSandbox = 1,
                group = new ObserverBattleGroup
                {
                    type = MatchmakingType.BattlePvE1x1,
                    current = 2,
                    need = 2
                },
                player1 = new ObserverBattlePlayer
                {
                    profile = new BattlePlayerProfile
                    {
                        //freeslot = profile.loots.GetFreeIndex,
                        hero = new BattlePlayerProfileHero
                        {
                            index = ClientWorld.Instance.Profile.SelectedHero,
                            level = 1
                            //exp = 0
                        }
                    },
                    playerID = ClientWorld.Instance.Profile.index/*,
                    deck = BattlePlayerDeck.PrepareDeck(deck)*/
                },
                player2 = new ObserverBattlePlayer
                {
                    profile = new BattlePlayerProfile
                    {
                        is_bot = true,
                        hero = new BattlePlayerProfileHero
                        {
                            index = 1
                        }
                    },
                    playerID = 0,
                    //deck = BattlePlayerDeck.Shuffle(enemy_bot.deck, _random)
                }
            };
        }

        void ExitMainMenu()
        {
            _home.Initialization(false);
            _home.Simulation(false);

        }

        void EnterBattle()
        {
            Input.multiTouchEnabled = true;
            SoftTutorialManager.Instance?.OnGoToBattle();

            _battle.Initialization(true);
            _battle.Simulation(true);
            _battle.Presentation(true);
        }

        void UpdateBattle()
        {
            while (_events.Count > 0)
            {
                var _event = _events[0];
                _events.RemoveAt(0);

                switch ((PlayerGameMessage)_event.message.protocol)
                {
                    case PlayerGameMessage.Disconnect:
                        ForceExitBattle();
                        NetworkMessageHelper.BattleExit(EntityManager);
                        break;

                    case PlayerGameMessage.Authorization:
                        //UnityEngine.Debug.Log($"PlayerGameMessage.Authorization >> {_event.message.size}");

                        var _battle_index = _event.message.data.ReadUShort();
                        UnityEngine.Debug.Log($"_battle_index: {_battle_index}");
                        if (Battlefields.Instance.Get(_battle_index, out BinaryBattlefields _binary_field))
                        {
                            _scene.Load(_binary_field.prefabs.client, (Scene scene) =>
                            {
                                // message => iam ready
                                ClientWorld.Instance.SceneReady();
                                // NetworkMessage.SceneReady();
                                // enable systems
                                _battle.Initialization(true);
                                _battle.Presentation(true);

                                if (_state_machine.CurrentState == ClientState.Battle)
                                {
                                    _battleLoadedEvent.Invoke();
                                    _missionLoaded.Invoke();
                                }
                            });
                        }
                        break;

                    case PlayerGameMessage.BattleInstance:

                        UnityEngine.Debug.Log("PlayerGameMessage.BattleInstance");

                        var _buffer = PostUpdateCommands;
                        var _reader = _event.message.data;

                        #region create battle instance
                        var _battle_instance = default(BattleInstance);
                        _battle_instance.players.Deserialize(ref _reader);
                        _battle_instance.Deserialize(ref _reader);
                        var _skills = Components.Instance.Get<MinionSkills>();
                        //_battle_instance.players.player
                        for (byte i = 0; i < _battle_instance.players.count; ++i)
                        {

                            var _player = _battle_instance.players[i];

                            if (i < _battle_instance.players.count - 1)
                                ObjectPooler.instance.InitDeck(_player.deck);
                            else
                                ObjectPooler.instance.InitDeck(_player.deck, () =>
                                {
                                    ClientWorld.Instance.PlayerReady();
                                    HideLoadingStub();
                                });

                            ObjectPooler.instance.InitHero(_player.profile.hero.index);

                            // we are player
                            if (_battle_instance.players.player == i)
                            {
                                //mySide = _player.side;
                                if (Heroes.Instance.Get(_player.profile.hero.index, out BinaryHero binary))
                                {
                                    if (_skills.TryGetValue(binary.minion, out MinionSkills item))
                                    {
                                        if (Database.Skills.Instance.Get(item.skill1, out BinarySkill binary_skill1))
                                        {
                                            if (Database.Skills.Instance.Get(item.skill2, out BinarySkill binary_skill2))
                                            {

                                                BattleInstanceInterface.instance.InitSkills(binary_skill1, binary_skill2, isReloadedSession);
                                            }
                                        }
                                    }
                                }
                            }
                            var name = ClientWorld.Instance.Profile.IsBattleTutorial && _battle_instance.players.player == i ? "locale:1639" : _player.profile.name.ToString();
                            BattleInstanceInterface.instance.SetNames(name, _battle_instance.players.player == i);

                            _battle_instance.players[i] = _player;
                        }

                        BattleInstanceInterface.instance.HideHpInTutorial();

                        var _entity = PostUpdateCommands.CreateEntity();
                        PostUpdateCommands.AddComponent(_entity, _battle_instance);

                        if (_battle_instance.isTutorial)
                        {
                            //var _tutorial_Instance = default(TutorialInstance);
                            //var entity = PostUpdateCommands.CreateEntity();
                            //PostUpdateCommands.AddComponent(entity, _tutorial_Instance);

                            //var tutorial_event = _tutorial_Instance.GetCurrentTriggerEvent();
                            //var event_entity = PostUpdateCommands.CreateEntity();
                            //var event_capture = new EventCaptureInstance { _event = tutorial_event.type };
                            //PostUpdateCommands.AddComponent(event_entity, event_capture);

                            //UnityEngine.Debug.LogError($"<color=green>tutorial_event.currentTrigger {tutorial_event.type}</color>");
                        }
                        #endregion
                        break;

                    case PlayerGameMessage.BattleFinish:
                        _event.message.data.size = 0;
                        //GameOverWindowBehaviour.Instance.OpenWindow();
                        break;

                    //case PlayerGameMessage.BattleClose:
                    //    ForceExitBattle();
                    //    break;

                    case PlayerGameMessage.PlayerActionFailed:


                        var index = _event.message.data.ReadByte();

                        BattleInstanceInterface.instance.hand.handObjects[index].FailedCardToNormalState();

                        break;

                }

                switch ((ObserverPlayerMessage)_event.message.protocol)
                {
                    case ObserverPlayerMessage.Disconnect:
                        // TODO: any suggestion ?
                        break;

                    case ObserverPlayerMessage.UpdatedProfile:
                        var _profile = new PlayerProfileInstance();
                        _profile.Deserialize(ref _event.message.data);
                        _home.DefaultProfile = _profile;

                        _home.UserProfile.BuildFromObserverInstance(_profile);
                        break;

                    case ObserverPlayerMessage.BattleRatingResult:
                        {
                            var _rating_reward = default(BattleRatingResultReward);
                            _rating_reward.Deserialize(ref _event.message.data);
                            BattleDataContainer.Instance.SetBattleData(_rating_reward);
                            GameOverWindowBehaviour.Instance.OpenWindow();

                            Debug.Log(" >>>> BattleRatingResult!");
                            var _auth = GetSingleton<ObserverConnectionAuthorization>();

                            NetworkMessageHelper.RequestForUpdatedProfile();
                        }
                        break;

                    case ObserverPlayerMessage.BattleMissionResult:
                        {
                            var _mission_reward = default(BattleRatingResultReward);
                            _mission_reward.Deserialize(ref _event.message.data);
                            BattleDataContainer.Instance.SetBattleData(_mission_reward);
                            GameOverWindowBehaviour.Instance.OpenWindow();

                            var _auth = GetSingleton<ObserverConnectionAuthorization>();
                            Debug.Log(" >>>> BattleMissionResult! " + BattleDataContainer.Instance.isVictory);

                            AnalyticsManager.Instance.SendTutorBattleResultEvent(BattleDataContainer.Instance.isVictory);

                            NetworkMessageHelper.RequestForUpdatedProfile();
                        }
                        break;
                    case ObserverPlayerMessage.CurrencyChangeEvent:
                        var info = default(ObserverPlayerCurrencyChangeEventInfo);
                        info.Deserialize(ref _event.message.data);

                        if (info.difference > 0)
                        {
                            AnalyticsManager.Instance.CurrencyEarn(info.difference, info.currencyType, (int)info.source_id, info.changeSourceType);
                        }
                        else if (info.difference < 0)
                        {
                            AnalyticsManager.Instance.CurrencySpend(info.difference, info.currencyType, (int)info.source_id, info.changeSourceType);
                        }
                        break;
                }

            }

        }

        void ExitBattle()
        {
            MinionsSoundsManager.ClearAll();

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MinionSnapshot>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TutorialSnapshot>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BattleSnapshot>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<EventInstance>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<EventCaptureInstance>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<EntityDatabase>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<BattleInstance>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TutorialInstance>()
            ));

            EntityManager.DestroyEntity(EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MinionAnimation>()
            ));

            BridgeHighlightSystem.bridge1 = false;
            BridgeHighlightSystem.bridge2 = false;
            BridgeHighlightSystem.currentSide2 = BattlePlayerSide.None;
            BridgeHighlightSystem.currentSide1 = BattlePlayerSide.None;
            ManaUpdateSystem.PlayerMana = 0;
            ManaUpdateSystem.ManaToUse = 0;
            ManaUpdateSystem.ManaSelected = 0;

            _battle.Initialization(false);
            _battle.Simulation(false);
            _battle.Presentation(false);

            ClientWorld.Instance.GetExistingSystem<BattleBucketsSystem>().Minions.Clear();
            ClientWorld.Instance.GetExistingSystem<BattleBucketsSystem>().Effects.Clear();

            ClientWorld.Instance.ClearWorld();

            ClientWorld.Instance.GetExistingSystem<MinionGameObjectInitializationSystem>().ClearMinionCenters();

        }

        private void OnCompleteClick()
        {
            _state_machine.SwitchTo(ClientState.MainMenu);
        }

        protected override void OnUpdate()
        {
            if (!_query_signals.IsEmptyIgnoreFilter)
            {
                var _signals = _query_signals.ToComponentDataArray<NetworkMessageHelper>(Allocator.TempJob);
                var _entities = _query_signals.ToEntityArray(Allocator.TempJob);
                for (int i = 0; i < _signals.Length; ++i)
                {
                    Debug.Log("signal: " + _signals[i].protocol);
                    _events.Add(new EventHash
                    {
                        entity = _entities[i],
                        message = _signals[i]
                    });
                    PostUpdateCommands.DestroyEntity(_entities[i]);
                }
                _entities.Dispose();
                _signals.Dispose();
            }
            _state_machine.Update();
        }

        public void ReloadGame()
        {

            Debug.Log("Start reloading");

            _gameReloading = true;
            isReloadedSession = true;

            var device_info = new PlayerProfileDevice
            {
                device_id = SystemInfo.deviceUniqueIdentifier,
                device_model = SystemInfo.deviceModel,
                operating_system = SystemInfo.operatingSystem,
                memory_size = SystemInfo.systemMemorySize
            };

            ShowLoadingStub(() =>
            {
                _state_machine.SwitchTo(ClientState.Loading);
               ClientWorld.Instance.ObserverConnect(device_info, SystemInfo.deviceName);
            });
        }

        internal void StartSearchOpponent()
        {
            _opponentSearchStartEvent.Invoke();
        }
    }
}
