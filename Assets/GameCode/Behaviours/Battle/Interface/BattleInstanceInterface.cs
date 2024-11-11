using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Legacy.Database;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

namespace Legacy.Client
{
    public class BattleInstanceInterface : MonoBehaviour
    {
        //[Foldout("Rect Transforms", true)]
        [SerializeField] private RectTransform UpPanelRect;
        [SerializeField] private RectTransform DownPanelRect;
        [SerializeField] private RectTransform CardNext_5;

        private Vector3 UpPanelRectStartPos;
        private Vector3 DownPanelRectStartPos;
        private Vector3 CardNext_5StartPos;

        private Vector3 UpPanelRectHiddenPos;
        private Vector3 DownPanelRectHiddenPos;
        private Vector3 CardNext_5HiddenPos;

        //[Foldout("Other Properties", true)]

        public GameObject canvas;
        public Camera MainCamera;
        public Camera UICamera;

        public Text PrepareText;
        public TextMeshProUGUI PlayerName;
        public TextMeshProUGUI EnemyName;

        public UpStarsView PlayerStars;
        public UpStarsView EnemyStars;

        public TimerBehaviour TimerText;

        public ManaUI uimana;
        [HideInInspector] public bool IsGameReloaded = false;

        public HandBehaviour hand;

        [System.Serializable]
        public struct ManaUI
        {
            public TextMeshProUGUI text;
            public ManaSliderBehaviour manaSlider;
        }

        public BattleSkillDragBehaviour Skill1;
        public BattleSkillDragBehaviour Skill2;

        public SkillsPositionsBehaviour skillsPositionsBehaviour;

        public BattleCardDragBehaviour[] Cards;

        public GameObject Prepare;

        public GameObject ManaBooster;
        public GameObject SkillsBooster;

        [SerializeField]
        private GameObject SandboxInterfacePrefab;

        public static BattleInstanceInterface instance;
        private EntityQuery _query_battle;
        private EntityQuery _query_sandbox;

        [SerializeField] private GameObject skillParticle1;
        [SerializeField] private GameObject skillParticle2;

        [SerializeField] private GameObject screenFrame;

        [SerializeField] private AudioSource endBattleSource;

        bool initiated = false;

        void OnEnable()
        {
            var cameraData = MainCamera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(UICamera);

            FinishInPorgress = false;
            canvas = transform.parent.gameObject;
            instance = this;
            _query_battle = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
            skillsPositionsBehaviour.Init();

            InitPoses();
            
            //DownPanelRect.DOLocalMoveY(-DownPanelRect.rect.height, 0);
        }
        public void SetNames(string name, bool me)
        {
            var tmp = me ? PlayerName : EnemyName;
            tmp.text = Locales.Get(name);
        }

        private void InitPoses()
        {
            UpPanelRectStartPos         = UpPanelRect.position;
            DownPanelRectStartPos       = DownPanelRect.position;

            //CardNext_5StartPos          = new Vector3(850, 0, 0);

            UpPanelRectHiddenPos        = UpPanelRectStartPos   .AddCoords(0,  3, 0);
            DownPanelRectHiddenPos      = DownPanelRectStartPos .AddCoords(0, -3, 0);
            //CardNext_5HiddenPos         = CardNext_5StartPos    .AddCoords(1200, -300, 0);

            if (!ClientWorld.Instance.GetOrCreateSystem<StateMachineSystem>().IsConnectedTooExistedBattle)
            {
                UpPanelRect.position = UpPanelRectHiddenPos;
                DownPanelRect.position = DownPanelRectHiddenPos;
                //CardNext_5.position = CardNext_5HiddenPos;
            }
        }

        public IEnumerator BridgeSkill()
        {
            skillParticle1.SetActive(true);
            skillParticle2.SetActive(true);
            StartCoroutine(StopEffect());
            IEnumerator StopEffect()
            {
                yield return new WaitForSeconds(2f);
                skillParticle1.SetActive(false);
                skillParticle2.SetActive(false);
            }
            yield return null;
        }

        public void ShowUpPanel()
        {
            var lasting = 0.75f;
            UpPanelRect.DOMove(UpPanelRectStartPos, lasting).SetEase(Ease.InQuad);
        }

        public void ShowDownPanel()
        {
            var lasting = 0.75f;
            DownPanelRect.DOMove(DownPanelRectStartPos, lasting).SetEase(Ease.InQuad);
        }

        public void HidePanels()
        {
            var lasting = 0.5f;
            UpPanelRect.DOMove(UpPanelRectHiddenPos, lasting).SetEase(Ease.OutSine);
            DownPanelRect.DOMove(DownPanelRectHiddenPos, lasting).SetEase(Ease.OutSine);
        }

        public void CanUpdateHand()
        {
            hand.CanContinue = true;
        }

        public void ShowNextCard()
        {
            //var lasting = 0.2f;
            //CardNext_5.DOMove(CardNext_5StartPos, lasting).SetEase(Ease.OutSine);
        }

        public void UpdateHand()
        {
            var _entity = _query_battle.GetSingletonEntity();
            var _battle = ClientWorld.Instance.EntityManager.GetComponentData<BattleInstance>(_entity);
            hand.UpdateHand(_battle.players[_battle.players.player].hand, true);
        }

        public static bool FinishInPorgress;
        private void OnFinishStateChanged()
        {
            FinishInPorgress = true;

            //animator.SetBool("ExitBattle", true);

            var s = SceneManager.GetActiveScene();
            var rgo = s.GetRootGameObjects();
            foreach (var go in rgo)
            {
                var ms = go.GetComponentsInChildren<MinionInitBehaviour>();
                foreach (var m in ms)
                {
                    var a = m.GetComponent<Animator>();
                    if (a.GetBool("Death"))
                    {
                        continue;
                    }
                    var clips = a.runtimeAnimatorController.animationClips;
                    bool victorious = false;
                    if (m.isHero)
                    {
                        bool isDieing = false;
                        foreach (var c in clips)
                        {
                            if (c.isLooping && c.name == "Death")
                            {
                                isDieing = true;
                                break;
                            }
                        }
                        if (isDieing) continue;
                        foreach (var c in clips)
                        {
                            if (c.name == "Victory")
                            {
                                a.Play("Victory");
                                victorious = true;
                                break;
                            }
                        }
                    }
                    if (!victorious)
                    {
                        a.SetBool("Landing", false);
                        a.SetBool("Stand", false);
                        a.SetBool("Walk", false);
                        a.SetBool("Death", false);
                        a.SetBool("Skill1", false);
                        a.SetBool("Skill2", false);
                        a.Play("Stand");
                        a.enabled = true;
                    }
                }
            }

        }

        public void InitSkills(BinarySkill skill1, BinarySkill skill2,bool isGameReloaded)
        {
            IsGameReloaded = isGameReloaded;
            Skill1.InitSkillView(skill1);
            Skill2.InitSkillView(skill2);
        }

        public void HideHpInTutorial()
        {
        }

        public int TimeToStart
        {
            get
            {
                if (_query_battle.IsEmptyIgnoreFilter)
                    return 0;
                var _entity = _query_battle.GetSingletonEntity();
                //var _bis = ServerWorld.Instance.EntityManager.GetComponentData<BattleInstance>(_entity);
                //if(_bis.timer != 0)
                //{
                //	return _bis.timer;
                //}
                var _instance = ClientWorld.Instance.EntityManager.GetComponentData<BattleInstance>(_entity);
                if (_instance.status != BattleInstanceStatus.Waiting)
                    return 0;
                return _instance.timer;
            }
        }

        void DelayedInit(BattleInstance _instance)
        {
            initiated = true;

            if (_instance.isSandbox)
            {
                CreateSandboxInterface();
                HideHand();
                TimerText.transform.parent.gameObject.SetActive(false);
                return;
            }

        }
        private bool zoomStarted = false;
        private float timer = 0;
        void Update()
        {
            //if (_query_battle == null) return;

            if (!_query_battle.IsEmptyIgnoreFilter)
            {
                var _entity = _query_battle.GetSingletonEntity();
                var _instance = ClientWorld.Instance.EntityManager.GetComponentData<BattleInstance>(_entity);

                if (!initiated)
                    DelayedInit(_instance);

                //var _bis = ServerWorld.Instance.EntityManager.GetComponentData<BattleInstance>(_entity);
                switch (_instance.status)
                {

                    case BattleInstanceStatus.Waiting:
                    case BattleInstanceStatus.Prepare:
                        PrepareText.text = _instance.timer.ToString();
                        if (_instance.timer == 0)
                        {
                            PrepareText.text = _instance.timer.ToString();
                        }
                        else
                        {
                            PrepareText.text = _instance.timer.ToString();
                        }
                        break;

                    case BattleInstanceStatus.Playing:
                        PrepareText.text = _instance.timer.ToString() + "   " + _instance.status;
                        timer = 0;

                        if (_instance.isAdditionalTime)
                            screenFrame.SetActive(true);

                        break;
                    case BattleInstanceStatus.AdditionalTIme:
                        PrepareText.text = _instance.timer.ToString() + "   " + _instance.status;
                        timer = 0;
                        break;
                    case BattleInstanceStatus.Dispose:
                        if (screenFrame)
                        {
                            screenFrame.GetComponentInChildren<ParticleSystem>().Stop();
                        }
                        endBattleSource.gameObject.SetActive(true);
                        break;
                }
                if (_instance.status > BattleInstanceStatus.FastKillingHeroes)
                {
                    if(screenFrame)
                    {
                        screenFrame.GetComponentInChildren<ParticleSystem>().Stop();
                    }
                }
                if (_instance.status > BattleInstanceStatus.Pause)
                {
                    if (!zoomStarted)
                    {
                        MainCameraMoveBehaviour.instance.StartZoomCamera(7.0f, 0);
                        zoomStarted = true;
                    }
                    timer += Time.deltaTime;
                }
            }
        }

        public void OnFinishAnimationDone()
        {
            OnFinishStateChanged();
        }

        private void CreateSandboxInterface()
        {
            Instantiate(SandboxInterfacePrefab, transform.parent);
        }
        private void HideHand()
        {
                hand.handObjects[0].transform.parent.gameObject.SetActive(false);
        }
    }
}
