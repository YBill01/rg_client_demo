using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static LootBoxBehaviour;

namespace Legacy.Client
{

    public enum GameOverWindowAnimStates
    {
        EmptyStartState,
        ZoomCamera,
        HidePanels,
        MainWindowAppear,
        StarsPanelAppear,
        SetStarsToMainPanel,
        MovePanelToPosition,
        ResourcesPanelAppear,
        Resource1Appear,
        Resource2Appear,
        Resource3Appear,
        LootBoxAppear,
        CalmState,
        PreLastState,
        DoNothing
    }

    public class GameOverWindowBehaviour : MonoBehaviour
    {

        public static GameOverWindowBehaviour Instance;
        public float waitTime = 0;
        [SerializeField] private GameOverWindowAnimStates currentState;
        [SerializeField] private GameOverWindowAnimStates previousState;
        [SerializeField] private GameOverAnimatorsBehaviour animationsController;
        [SerializeField] private GameObject tapToContinue;
        [SerializeField] private Image blockImage;

        [InspectorName("Resources")]
        [SerializeField] private BattleOutcomeBehaviour battleOutcomeBehaviour;
        [SerializeField] private StarsControllerBehaviour mainStars;
        [SerializeField] private GameObject LootCurrencyPrefab;
        [SerializeField] private GameObject Content;
        [SerializeField] private GameObject ResourcesView;
        [SerializeField] private GameObject SlotsFullInfoPrefab;
        [SerializeField] private GameObject ResourcesContainerPrefab;
        [SerializeField] private Transform ResourcesLayout;
        [SerializeField] private Transform lootBoxContainer;
        [SerializeField] private TextMeshProUGUI lootboxName;
        [SerializeField] private GameObject touchCameraZone;
        [SerializeField] private AudioSource ChestAppearSource;


        private float stateTime = 0;
        private GameOverWindowAnimStates tempState = GameOverWindowAnimStates.EmptyStartState;

        private List<LootCurrencyBehaviour> currenciesList = new List<LootCurrencyBehaviour>();

        internal void OpenWindow()
        {
            gameObject.SetActive(true);
            ShowLootBoxContainer();
            AnalyticsManager.Instance.BattleEnd();
        }

        private void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            SwitchState(GameOverWindowAnimStates.EmptyStartState);
            waitTime = 0;
            touchCameraZone.SetActive(false);
        }

        private void SwitchState(GameOverWindowAnimStates state, float waitTimeBeforeSwitch = 0)
        {
            if (waitTimeBeforeSwitch == 0 || stateTime >= waitTimeBeforeSwitch && currentState != state)
            {
                previousState = currentState;
                currentState = state;
                stateTime = 0;
                waitTime = 0;
            }
        }

        public void NextState(float waitTime = 0)
        {
            var nextState = currentState;
            SwitchState(++nextState, waitTime);
        }

        public void NextStateOnce()
        {
            if (currentState != GameOverWindowAnimStates.DoNothing)
                tempState = currentState + 1;
            if (waitTime == 0 || stateTime >= waitTime)
            {
                previousState = currentState;
                currentState = tempState;
                stateTime = 0;
                waitTime = 0;
            }
            if (stateTime < waitTime)
            {
                currentState = GameOverWindowAnimStates.DoNothing;
            }
        }

        private void SkipStep()
        {
            stateTime = 0;
            waitTime = 0.2f;
            GameOverAnimatorsBehaviour.SetAnimationToEnd();
        }

        private void Update()
        {
            StatesOrders();
            stateTime += Time.deltaTime;
        }

        private void StatesOrders()
        {
            switch (currentState)
            {
                case GameOverWindowAnimStates.EmptyStartState:
                    waitTime = 0f;
                    animationsController.SetStart();
                    TapToContinue(false);
                    SoundManager.Instance.PlayBattleMusic(true);

                    Content.SetActive(true);
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.ZoomCamera:
                    waitTime = 0;
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.HidePanels:
                    waitTime = 0;
                    BattleInstanceInterface.instance.HidePanels();
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.MainWindowAppear:
                    battleOutcomeBehaviour.SetMainView(BattleDataContainer.Instance.isVictory);
                    GameOverAnimatorsBehaviour.NextStartAnimation(false, 0.5f);
                    break;
                case GameOverWindowAnimStates.StarsPanelAppear:
                    BattleInstanceInterface.instance.OnFinishAnimationDone();
                    GameOverAnimatorsBehaviour.NextStartAnimation(false, 0.8f, true);
                    break;
                case GameOverWindowAnimStates.SetStarsToMainPanel:
                    waitTime = 1;
                    battleOutcomeBehaviour.PlayMusic();
                    CreateLootboxFromPrefab();
                    StartCoroutine(mainStars.SetStars(BattleDataContainer.Instance.starsWeGotInBatte));
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.MovePanelToPosition:
                    GameOverAnimatorsBehaviour.NextStartAnimation(true, 0.5f);
                    break;
                case GameOverWindowAnimStates.ResourcesPanelAppear:
                    waitTime = 0;
                    battleOutcomeBehaviour.ActiveParticles(BattleDataContainer.Instance.isVictory);
                    ResourcesView.SetActive(true);//anim
                    ChestAppearSource.Play();
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.Resource1Appear:
                    waitTime = 1.1f;
                    LootAdditionalInfo();
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.Resource2Appear:
                    waitTime = 0;
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.Resource3Appear:
                    waitTime = 1.7f;
                    StartCoroutine(SpawnAllResources());
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.LootBoxAppear:
                    waitTime = 0;
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.CalmState:
                    waitTime = 0;
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.PreLastState:
                    waitTime = 0;
                    TapToContinue(true);
                    NextStateOnce();
                    break;
                case GameOverWindowAnimStates.DoNothing:
                    NextStateOnce();
                    break;
                default:
                    break;
            }

        }

        private void TapToContinue(bool flag)
        {
            tapToContinue.SetActive(flag);
        }

        public void ShowLootBoxContainer()
        {
            lootBoxContainer.parent.gameObject.SetActive(BattleDataContainer.Instance.isVictory);
        }

        private void CreateLootboxFromPrefab()
        {
            if (Loots.Instance.Get(BattleDataContainer.Instance.GetLootIndex(), out BinaryLoot BinaryBox))
                SpawnExistingLootBox(BinaryBox);
            else
            {
                if (Loots.Instance.Get(1, out BinaryLoot defaultBox))
                    SpawnContentAllSlotsBusy(defaultBox);
            }
        }

        private void SpawnContentAllSlotsBusy(BinaryLoot BinaryBox)
        {
            lootboxName.transform.parent.gameObject.SetActive(false);
            var loaded = Addressables.InstantiateAsync($"Loots/{BinaryBox.prefab}LootBox.prefab", lootBoxContainer);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                var BoxView = async.Result.GetComponent<LootBoxViewBehaviour>();
                BoxView.gameObject.SetActive(false);
                BoxView.Init(BoxState.SlotsFull, BinaryBox);
                BoxView.SetScaleMultiplier(0.9f);
                lootBox = BoxView;
            };
        }

        private LootBoxViewBehaviour lootBox;
        public void SpawnExistingLootBox(BinaryLoot BinaryBox)
        {
            var loaded = Addressables.InstantiateAsync($"Loots/{BinaryBox.prefab}LootBox.prefab", lootBoxContainer);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                var BoxView = loaded.Result.GetComponent<LootBoxViewBehaviour>();
                BoxView.gameObject.SetActive(false);
                BoxView.Init(BoxState.JumpToSlot, BinaryBox);
                BoxView.SetScaleMultiplier(0.9f);
                lootBox = BoxView;
            };
        }
        public void LootAdditionalInfo()
        {
            lootBox.gameObject.SetActive(true);
           
            if (Loots.Instance.Get(BattleDataContainer.Instance.GetLootIndex(), out BinaryLoot BinaryBox))
            {
                lootBox.ChangeState(BoxState.JumpToSlot);
                lootboxName.transform.parent.gameObject.SetActive(true);
                lootboxName.text = Locales.Get(BinaryBox.title);

                SlotsFullInfoPrefab.SetActive(false);
            }
            else
            {
                lootBox.ChangeState(BoxState.SlotsFull);
                SlotsFullInfoPrefab.SetActive(true);

            }
        }
        private IEnumerator SpawnAllResources()
        {
            var ResourcesLayoutChildIndex = -1;

            byte busySlotsCount = BattleDataContainer.Instance.GetRewardResourcesSlotsCount();
            for (int i = 0; i < busySlotsCount; i++)
            {
                Instantiate(ResourcesContainerPrefab, ResourcesLayout);
            }
            lootBox.ChangeState(BoxState.Closed);

            //yield return new WaitForSeconds(0.7f);
            //CreateResourceByType(CurrencyType.HeroExp, (ushort)BattleDataContainer.Instance.HeroExpDelta, CheckCanSpawnResource(BattleDataContainer.Instance.HeroExpDelta, ref ResourcesLayoutChildIndex, busySlotsCount));
            yield return new WaitForSeconds(0.3f);

            var rating = CreateResourceByType(
                CurrencyType.Rating,
                BattleDataContainer.Instance.RatingDelta,
                CheckCanSpawnResource(
                    (ushort)BattleDataContainer.Instance.RatingDelta, 
                    ref ResourcesLayoutChildIndex, 
                    busySlotsCount)
                );

            if (rating) 
                rating.AddComponent<RatingTextBehavior>().PaintText(BattleDataContainer.Instance.RatingDelta);
            yield return new WaitForSeconds(0.3f);

            CreateResourceByType(
                CurrencyType.Soft, 
                BattleDataContainer.Instance.Soft, 
                CheckCanSpawnResource(
                    BattleDataContainer.Instance.Soft, 
                    ref ResourcesLayoutChildIndex, 
                    busySlotsCount)
                );
        }

        private int? CheckCanSpawnResource(int resourceCount, ref int ResourcesLayoutChildIndex, int busySlotsCount)
        {
            return (ResourcesLayoutChildIndex + 1) < busySlotsCount && resourceCount != 0 ? ++ResourcesLayoutChildIndex : new int?();
        }

        private GameObject CreateResourceByType(CurrencyType type, int resouceMinCount, int? childIndex, ushort resourceMaxCount = 0)
        {
            if (childIndex != null)
            {
                LootCurrencyBehaviour resource = Instantiate(LootCurrencyPrefab, ResourcesLayout.GetChild(childIndex.Value)).GetComponent<LootCurrencyBehaviour>();
                resource.Init(type, resouceMinCount, resourceMaxCount);
                currenciesList.Add(resource);
                return resource.gameObject;
            }
            return null;
        }

        private StateMachineSystem stateMachineSystem;

        public void GoHome()
        {
            stateMachineSystem = ClientWorld.Instance.GetExistingSystem<StateMachineSystem>();
            stateMachineSystem.ForceExitBattle(StateMachineSystem.ClientState.MainMenu);
        }


    }
}
