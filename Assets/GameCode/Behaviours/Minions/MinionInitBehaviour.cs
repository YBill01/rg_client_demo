using DG.Tweening;
using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Legacy.Client
{
    public class MinionInitBehaviour : MonoBehaviour
    {
        public UnityEvent AtBattleEvent { get => atBattleEvent; }
        public UnityEvent OutBattleEvent { get => outBattleEvent; }
        public static List<MinionInitBehaviour> MinionsList { get => minionsList; }

        public bool IsEnemy => isEnemy;
        public bool atBattle;
        public bool atRage;
        public bool isHero;
        public bool isNonTarget;
        public bool shouldBumpAtStart = true;

        [Header("Minion variables")]
        [SerializeField] public GameObject geometry;
        [SerializeField] public GameObject main;

        [SerializeField] private MinionSoundManager soundManager;
        [SerializeField] private GameObject shadow;
        [SerializeField] private GameObject circle;
        [SerializeField] private GameObject collider;
        [SerializeField] private GameObject spawnDust;

        [SerializeField] public InitBehaviour CustomInitBehaviour;

        [SerializeField] private Vector3 PunchPower1;
        [SerializeField] private Ease PunchEase1;
        [SerializeField] private float PunchDuration1;

        [SerializeField] private float timeOfFalling;
        [SerializeField] private Ease EaseOfFalling;
        [SerializeField] private float squadMinionFallingDelay;

        [SerializeField] private AnimationCurve animationCurve;

        private static List<MinionInitBehaviour> minionsList = new List<MinionInitBehaviour>();

        private UnityEvent atBattleEvent = new UnityEvent();
        private UnityEvent outBattleEvent = new UnityEvent();

        private MinionMaterialsBehaviour _minionMaterialsBehaviour;
        public MinionMaterialsBehaviour MinionMatsBeh
        {
            get
            {
                if(_minionMaterialsBehaviour == null)
                {
                    _minionMaterialsBehaviour = GetComponent<MinionMaterialsBehaviour>();
                }
                return _minionMaterialsBehaviour;
            }
        }

        private int numberInSquad;
        private ushort cardIndex;
        private Vector3 timerPosition;
        private bool isEnemy;
        private bool isGray;

        public BinaryEntity Binary;

        private void OnEnable()
        {
            minionsList.Add(this);
            isDeleting = false;
            isForceDisposing = false;
        }

        public void DoMinionInvisible()
        {
            shadow.SetActive(false);
            if (geometry) 
                geometry.SetActive(false);
            if (main) 
                main.SetActive(false);
            if (CustomInitBehaviour) 
                CustomInitBehaviour.DoMinionInvisible();
        }

        public void DoMinionVisible(bool andMain = true)
        {
            shadow.SetActive(true);
            if (geometry)
                geometry.SetActive(true);
            if (main)
                main.SetActive(andMain);
            if (CustomInitBehaviour) 
                CustomInitBehaviour.DoMinionVisible();
            InitTeamColor(isEnemy); //TODO//TODO//TODO//TODO//TODO//TODO//TODO//TODO
        }
        public void DoMinionVisible()
        {
            shadow.SetActive(true);
            if (geometry)
                geometry.SetActive(true);
            if (main)
                main.SetActive(true);
            if (CustomInitBehaviour) 
                CustomInitBehaviour.DoMinionVisible();
            InitTeamColor(isEnemy);//TODO//TODO//TODO//TODO//TODO//TODO//TODO
        }

        public void MakeStarted(byte numberInSquad, bool isEnemy, ref MinionData minionData, ushort db)
        {
            atBattle = true;
            isDeleting = false;
            isForceDisposing = false;
            atRage = false;

            if (Entities.Instance.Get(db, out BinaryEntity binary))
            {
                //UnityEngine.Debug.LogError("db " + db);
                Binary = binary;
                this.numberInSquad = numberInSquad;
                this.cardIndex = minionData.card_index;
                this.isEnemy = isEnemy;
                var appearTime = (minionData.appearTime + (Settings.Instance.Get<BaseBattleSettings>().timeOfAlternateAppear * (numberInSquad + 1))) * 0.001f;

                var chillTime = minionData.chillTime * 0.001f;
                if (isEnemy && !IsSpawnedBySomebody(ref minionData))
                {
                    DoMinionInvisible();
                    if (isGray)
                    {
                        MinionMatsBeh.SetDefaultMaterials();
                    }
                    isGray = false;
                }
                if (!IsSpawnedBySomebody(ref minionData))
                    StartCoroutine(StartFalling(appearTime, minionData, chillTime));
                else
                {
                    DoMinionVisible();
                    if (isGray)
                    {
                        MinionMatsBeh.SetDefaultMaterials();
                    }
                    isGray = false;
                    StartCoroutine(StartFalling(0f, minionData, chillTime, true));
                }

                if(cardIndex > 0)
                {
                    AnalyticsManager.Instance.CardUsed(cardIndex);
                }

                BackManaInUse();
                if (CustomInitBehaviour) 
                    CustomInitBehaviour.Init();
            }
        }
        private void DisableMinions(ref MinionData myMinionData, bool isVisible)
        {
            var _spawn_prefabs = ClientWorld.Instance.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<MinionData>(),
                ComponentType.ReadOnly<EntityDatabase>(),
                ComponentType.ReadOnly<Transform>()
            );
            var _transforms = _spawn_prefabs.ToComponentArray<Transform>();
            var _minionsData = _spawn_prefabs.ToComponentDataArray<MinionData>(Unity.Collections.Allocator.TempJob);
            for (int i = 0; i < _minionsData.Length; i++)
            {
                var minionData = _minionsData[i];
                if (minionData.side == myMinionData.side && myMinionData.card_index == minionData.card_index)
                {
                    if (_transforms[i].GetComponent<MinionInitBehaviour>().isGray)
                    {
                        if (isVisible) _transforms[i].GetComponent<MinionInitBehaviour>().DoMinionVisible();
                        if (!isVisible) _transforms[i].GetComponent<MinionInitBehaviour>().DoMinionInvisible();
                    }
                }
            }
            _minionsData.Dispose();
        }

        private bool IsSpawnedBySomebody(ref MinionData myMinionData)
        {
            if (myMinionData.card_index == 0)
            {
                return true;
            }
            return false;
        }

        public void MakeForceDisposing()
        {
            isForceDisposing = true;
            TimeToForceDispose = 4f;
        }

        private bool isForceDisposing;
        private float TimeToForceDispose;
        private bool isDeleting;
        private float TimeToDelete;
        public void SetupWaitPrefab()
        {
            TimeToDelete = 2000;
        }

        internal void InitTeamColor(bool isEnemy)
        {
            if (CustomInitBehaviour != null)
            {
                CustomInitBehaviour.InitMaterial(isEnemy);
            }
            var team = GetComponent<TeamColorBehaviour>();
            if (team == null)
                return;
            if (isEnemy)
            {
                team.SetTeam(1);
            }
            else
            {
                team.SetTeam(0);
            }
        }

        protected void Update()
        {

            if (isForceDisposing)
            {
                TimeToForceDispose -= Time.deltaTime;
                if (TimeToForceDispose <= 0)
                {
                    isForceDisposing = false;
                    Unspawn();
                }
            }
            if (isDeleting)
            {
                TimeToDelete -= Time.deltaTime;
                if (TimeToDelete <= 0)
                {
                    isDeleting = false;
                    Unspawn();
                }
            }
            return;

            if (atBattleState != atBattle)
            {
                atBattleState = atBattle;
                if (atBattle)
                {
                    Debug.Log("Minion Entered battle");
                    minionsList.Add(this);
                    atBattleEvent.Invoke();
                }
                if (!atBattle)
                {
                    Debug.Log("Minion Left battle");
                    minionsList.Remove(this);
                    outBattleEvent.Invoke();
                }
            }
        }

        public void Unspawn()
        {
            var _active_world = World.DefaultGameObjectInjectionWorld;
            var _visualization = _active_world.GetOrCreateSystem<MinionGameObjectInitializationSystem>();

            _visualization.Unspawn(gameObject);
            ObjectPooler.instance.MinionBack(gameObject);

            GetComponent<DeathEffect>().Effect.SetActive(false);
            gameObject.SetActive(false);
        }

        private bool atBattleState = false;
        private bool finished;

        private void DisableAnimationsOnFinish()
        {
            if (!finished)
            {
                var animator = GetComponent<Animator>();
                finished = true;
                if (animator.GetBool("Death")) return;
                animator.SetBool("Stand", true);
                animator.SetBool("Walk", false);
                animator.SetBool("Attack", false);
            }
        }

        private void OnDisable()
        {
            DisableSpawnEffect();
            isForceDisposing = false;
            isDeleting = false;
            //atBattleState = false;
            //if (atBattle)
            //{
            //	outBattleEvent.Invoke();
            //}
            minionsList.Remove(this);
            atBattle = false;
        }

        public void MakeGray()
        {
            isGray = true;
            MinionMatsBeh.SetTransparentMaterials();
        }

        private IEnumerator StartFalling(float delay, MinionData minionData, float chillDuration, bool spawnBySomebody = false)
        {
            yield return new WaitForSeconds(delay);
            DisableMinions(ref minionData, false);
            soundManager.PlayAppear();
            MinionsSoundsManager.AddSourceToList(ref soundManager, isEnemy);

            float height = 0;
            if (CustomInitBehaviour)
            {
                height = CustomInitBehaviour.SpaWnHeight;
            }
            else if (!spawnBySomebody)
            {
                height = 4;
            }
            var tween = transform.DOLocalMoveY(0, timeOfFalling).From(height).SetEase(EaseOfFalling);

            if (shouldBumpAtStart)
            {
                tween.onComplete += Punch;
            }
            tween.OnStart(() => DoMinionVisible());
            if (minionData.layer != MinionLayerType.Fly) tween.onComplete += EnableSpawnEffect;

            if (isGray)
            {
                MinionMatsBeh.SetDefaultMaterials();
            }
            isGray = false;

            yield return null;
            if (cardIndex != 0)
            {
                ShowTimer(minionData.layer, chillDuration + timeOfFalling);
            }
            var mainTransform = GetComponent<MinionInitBehaviour>().main ? GetComponent<MinionInitBehaviour>().main.transform : transform.Find("Main");

            var basicScale = mainTransform.localScale;
            var nonBasicScale = new Vector3(-basicScale.x, basicScale.y, basicScale.z);

            DoMinionVisible();
            if (!isEnemy)
            {
                mainTransform.transform.localScale = basicScale;
            }
            else 
            { 
                mainTransform.transform.localScale = nonBasicScale;
            }

            if (GetComponent<MinionPanel>())
            {
                //UnityEngine.Debug.LogError("HidePanel ");
                GetComponent<MinionPanel>().HidePanel(false);
            }

        }
        private void SetInHighestPointOnSpawn()
        {
            var height = CustomInitBehaviour ? CustomInitBehaviour.SpaWnHeight : 4;
        }
        private float CalculateDelay(float appearDuration)
        {
            if (!Cards.Instance.Get(cardIndex, out BinaryCard binaryCard))
                return 0;

            var result = squadMinionFallingDelay * numberInSquad;//будут падать через время кратное 0.1с
            return result;
        }

        private void BackManaInUse()
        {
            if (numberInSquad != 0 || isEnemy || cardIndex == 0)
                return;

            if (!Cards.Instance.Get(cardIndex, out BinaryCard binaryCard))
                return;

            ManaUpdateSystem.ManaToUse -= binaryCard.manaCost;
        }

        private void ShowTimer(MinionLayerType layer, float duration)
        {
            if (numberInSquad != 0)
                return;

            var prefab = VisualContent.Instance.customVisualData.UnitTimerPrefab;
            var canvas = BattleInstanceInterface.instance.canvas.transform;

            var timer = Instantiate(prefab, canvas);
            timer.GetComponent<UnitTimerBehaviour>().InitTimer(timerPosition, duration, isEnemy, layer == MinionLayerType.Fly);
        }

        public void SetTimerPosition(Vector3 pos)
        {
            timerPosition = pos;
        }

        private void EnableSpawnEffect()
        {
            if (CustomInitBehaviour) CustomInitBehaviour.Spawn();
            SpawnEffect(true);
        }

        private void DisableSpawnEffect()
        {
            SpawnEffect(false);
        }

        public void SpawnEffect(bool flag)
        {
            if (spawnDust) 
                spawnDust.SetActive(flag);
        }

        Tween elongation = null;
        Tween makingWider = null;
        Tween backToNormal = null;
        Tween yoyoPunch = null;
        public void Punch()
        {
            Sequence mySequence = DOTween.Sequence();
            var realScale = transform.localScale;
            elongation = transform.DOScaleY(realScale.y + 0.3f, 0.15f)
                                .SetLoops(2, LoopType.Yoyo);
            makingWider = transform.DOScale(new Vector3(realScale.x + 0.2f, realScale.y, realScale.z + 0.2f), 0.1f)
                                .SetDelay(0.3f)
                                .SetLoops(2, LoopType.Yoyo);
            backToNormal = transform.DOScale(new Vector3(realScale.x, realScale.y, realScale.z), 0.3f)
                                .SetDelay(0.15f);

            mySequence
             .Join(elongation)
             .Join(makingWider)
             .Append(backToNormal)
             .Play();
        }
    }
}