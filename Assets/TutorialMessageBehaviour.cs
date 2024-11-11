using Legacy.Client;
using Legacy.Database;
using Legacy.Server;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class TutorialMessageBehaviour : MonoBehaviour
{

    public BinaryTutorialEvent binaryTutorialEvent;

    [SerializeField]
    private bool nextStepOnTap;

    [SerializeField]
    private Animator animator;


    public bool messageViewDone { get; private set; } = false;
    public Animator Animator { get => animator; }
    public GameObject MinionsContainer { get => minionsContainer; }
    public StaticColliders StaticColliders { get => staticColliders; }

    #region Init Containers
    private StaticColliders staticColliders;
    private GameObject minionsContainer;
    private void ResetCommonContainers()
    {
        if (staticColliders != null && minionsContainer != null)
            return;
        var s = SceneManager.GetActiveScene();
        var list = s.GetRootGameObjects();
        foreach (var l in list)
        {
            var cList = l.GetComponentsInChildren<StaticColliders>(true);
            if (cList.Length > 0)
            {
                staticColliders = cList[0];
            }
            var mList = l.GetComponentsInChildren<ObjectsPool>(true);
            foreach (var m in mList)
            {
                if (m.gameObject.name == "Minions")
                {
                    minionsContainer = m.gameObject;
                }
            }
            if (staticColliders != null && minionsContainer != null)
            {
                break;
            }
        }
    }
    #endregion

    public static void InitBattleTriggerSystem()
    {
        TutorialSnapshotSystem.TutorialStepEvent.RemoveListener(OnMessageSpawn);
        TutorialSnapshotSystem.TutorialStepEvent.AddListener(OnMessageSpawn);
    }

    private static LoadInstance loadInstance;
    private static void OnMessageSpawn(BinaryTutorialEvent tutorialEventParams)
    {
        Debug.Log(
            "\n" +
            "TutorialMSG " + "<color=green>" + tutorialEventParams.trigger + "</color>\n" +
            "<color=green>" + tutorialEventParams.type + "</color>\n" +
            "<color=white>" + tutorialEventParams.message + "</color>"
            );
        if (tutorialEventParams.analytic_event > 0)
        {
            AnalyticsManager.Instance.SendTutorialStep(tutorialEventParams.analytic_event);
        }
        //if (loadInstance != null)
        //{
        //    loadInstance.ClearLoad();
        //    if (loadInstance.message == tutorialEventParams.message.ToString())
        //    {
        //        Debug.Log("Next message act break");
        //        return;
        //    }
        //}

        if (tutorialEventParams.message.ToString() == "")
            return;

        if (tutorialEventParams.message.ToString() == "ShowCards")
        {
            //BattleInstanceInterface.instance.ShowCards();
            return;
        }

        if (tutorialEventParams.message.ToString() == "HideCards")
        {
            //BattleInstanceInterface.instance.HideCards();
            return;
        }

        if (tutorialEventParams.message.ToString() == "ShowSkills")
        {
            //  BattleInstanceInterface.instance.ShowSkills();
            return;
        }

        loadInstance = new LoadInstance(tutorialEventParams);
        loadInstance.Load(BattleInstanceInterface.instance.transform);
    }

    public static void DoRipple()
    {
        if (Input.GetMouseButton(0))
        {
            var tp = Instantiate(VisualContent.Instance.TouchPrefab);
            tp.transform.position = Input.mousePosition;
            return;
        }

        if (Input.touchCount > 0)
        {
            var tp = Instantiate(VisualContent.Instance.TouchPrefab);
            var t = Input.touches[0];
            tp.transform.position = t.position;
        }
    }

    void Start()
    {
        ResetCommonContainers();
        TutorialSnapshotSystem.TutorialStepEvent.RemoveListener(OnNextMessage);
        TutorialSnapshotSystem.TutorialStepEvent.AddListener(OnNextMessage);
        touchStarted = false;
    }

    private void OnNextMessage(BinaryTutorialEvent tutorialEventParams)
    {
        if (binaryTutorialEvent.message.ToString() == tutorialEventParams.message.ToString())
        {
            Debug.Log("Next message act " + "<color=green>" + tutorialEventParams.message + "</color>");
            if (animator != null)
            {
                animator.ResetTrigger("NextAct");
                animator.ResetTrigger("CurrentAct");
                animator.SetTrigger("NextAct");
            }
            return;
        }
        Debug.Log("Next Message event invoke " + "<color=green>" + tutorialEventParams.message + "</color>");
        TutorialSnapshotSystem.TutorialStepEvent.RemoveListener(OnNextMessage);
        DoHide();
    }

    private bool tapToNext;
    public void NextAct()
    {
        tapToNext = true;
    }

    public void ForceNextAct()
    {
        MakeTapEvent();
    }

    private bool touchStarted;

    void Update()
    {
        if (tapToNext)
        {
            if (!touchStarted)
            {
                if (Input.touchCount > 0 || Input.GetMouseButton(0) && !touchStarted)
                {
                    touchStarted = true;
                }
                return;
            }
            if (Input.touchCount == 0 || !Input.GetMouseButton(0))
            {
                DoRipple();
                MakeTapEvent();
                touchStarted = false;
                tapToNext = false;
            }
            return;
        }
        if (!nextStepOnTap) return;
        if (!messageViewDone) return;
        if ((Input.touchCount > 0 || Input.GetMouseButton(0)) && !touchStarted)
        {
            touchStarted = true;
            return;
        }
        if (Input.touchCount == 0 && !Input.GetMouseButton(0) && touchStarted)
        {
            DoRipple();
            //MakeTapEvent();
        }
        touchStarted = false;
    }

    public static void MakeTapEvent()
    {
        Debug.Log("<color=red>Static</color> Tap Message");
        ClientWorld.Instance.TutorialTapEvent();
    }

    private void DoHide()
    {
        DestroyImmediate(gameObject);
    }

    private void OnDestroy()
    {
        TutorialSnapshotSystem.TutorialStepEvent.RemoveListener(OnNextMessage);
    }

    public void MessageAnimationDone()
    {
        Debug.Log("Message view Done");
        messageViewDone = true;
    }

    internal class LoadInstance
    {
        private BinaryTutorialEvent tutorialEventParams;
        private AsyncOperationHandle<GameObject> loaded;

        public string message { get => tutorialEventParams.message.ToString(); }
        public LoadInstance(BinaryTutorialEvent tutorialEventParams)
        {
            this.tutorialEventParams = tutorialEventParams;
        }

        public void ClearLoad()
        {
            if (!isSet) return;
            if (loaded.IsValid())
            {
                loaded.Completed -= AsyncHandle;
            }
            isSet = false;
        }

        private bool isSet;
        public void Load(Transform parent)
        {
            loaded = Addressables.InstantiateAsync("TutorialBattle/" + tutorialEventParams.message.ToString() + ".prefab", parent);
            Debug.Log(tutorialEventParams.message.ToString() + " - StartLoading");
            loaded.Completed += AsyncHandle;
            isSet = true;
        }

        private void AsyncHandle(AsyncOperationHandle<GameObject> async)
        {
            GameObject obj = async.Result;
            if (obj)
            {
                var tmb = obj.GetComponent<TutorialMessageBehaviour>();
                var animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = true;
                }
                tmb.binaryTutorialEvent = tutorialEventParams;
                Debug.Log("Message prefab loaded " + "<color=green>" + obj.name + "</color>");
                loaded.Completed -= AsyncHandle;
                //loadInstance = null;
            }

        }
    }
}

public enum HeroIDs
{
    Galahard = 3,
    Zakzar = 15,
    Ascalia = 46,
    Sonnelon = 54,
    Meris = 56,
    AscaliaB = 88,
    ZakzarTutorial = 100,
}