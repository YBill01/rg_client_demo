using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using Legacy.Database;
using UnityEngine.EventSystems;

public class LoadingGroup : MonoBehaviour
{
    public static LoadingGroup Instance { get; private set; }
    public UnityEvent LoadingDisabled = new UnityEvent();
    public UnityEvent LoadingEnabled = new UnityEvent();

    private TipsSettings tipsSettings;
    public AsyncOperationHandle<SceneInstance> _sceneLoading;
    public AsyncOperationHandle<SceneInstance> SceneLoading
    {
        private get => _sceneLoading;
        set
        {
            _sceneLoading = value;
            startTime = Time.time;
            prevValue = 0;
        }
    }

    [SerializeField]
    private TextMeshProUGUI Loading_text;

    public TextMeshProUGUI Loading_Tips_text;
    public Slider slider;

    [SerializeField]
    private GameObject VersionWindow;
    [SerializeField] 
    private EventSystem eventSystem;
    [SerializeField]
    private GraphicRaycaster graphicRaycaster;

    private float startTime;
    private float prevValue;
    private float startValue;

    const float loadingPeriod1 = 0.65f;
    const float speedA = 0.5f; //Скороть эмитации загрузки для периода (начали загрузку, эмитация дошла до loadingPeriod1)
    const float speedB = 0.08f; // скорость  эмитации загрузки для периода (эмитация дошла до loadingPeriod1, до конца)
    private float speedC; // скорость  эмитации загрузки для периода (загрузили сцену, загрузили сцену + 2 секунды)
    const float LoadingPeriod1Const = loadingPeriod1 * speedB / speedA; //константа что бы не добавляьт лишние переменные
    const float timeToFinishLoading = 1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        //else
        //    gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        LoadingDisabled.Invoke();
    }

    public void CreateVersionWindow()
    {
        if (eventSystem)
        {
            eventSystem.enabled = true;
        }
        if (graphicRaycaster)
        {
            graphicRaycaster.enabled = true;
        }
        Instantiate(VersionWindow, transform);
    }

    private void OnEnable()
    {
        //вариант загрузки из Scriptable Object
		//Loading_Tips_text.gameObject.SetActive(LoadingTips.Instance);
        //if (LoadingTips.Instance) Loading_Tips_text.text = LoadingTips.Instance.GetRandomTips();

        //загрузка локалей с админки.
        Loading_Tips_text.text = "";
        Loading_Tips_text.gameObject.SetActive(false);
        if (isShowTips)
            ShowTips();

        startValue  = UnityEngine.Random.Range(0.03f, 0.15f);
        prevValue   = startValue;
        UpdateLoadingView(startValue * 100);
        LoadingEnabled.Invoke();
    }

    private bool isShowTips = false; //первая загрузка локали подтягиваются после создания типсы. 
    public void ShowTips()
    {
        tipsSettings = Settings.Instance.Get<TipsSettings>();
        Loading_Tips_text.text = Locales.Get("locale:" + tipsSettings.locale[(byte)UnityEngine.Random.Range(0, tipsSettings.locale.length)]);
        Loading_Tips_text.gameObject.SetActive(true);
        isShowTips = true;
    }

    private void Update()
    {
        if (!SceneLoading.IsValid())
            return;

        float newValue;
        if (SceneLoading.PercentComplete < 1)
        {
            newValue = startValue + (Time.time - startTime) * speedA;

            if (newValue > loadingPeriod1)
            {
                newValue = newValue + (Time.time - startTime) * speedB;
            }
            speedC = (1f - newValue) / (timeToFinishLoading);
        }
        else
        {
            newValue = startValue + prevValue + Time.deltaTime * speedC;
        }
        prevValue = newValue;
        newValue = Mathf.Clamp01(newValue) * 100f;
        UpdateLoadingView(newValue);
    }

    public void UpdateLoadingView(float newValue, string prefix = "")
    {
        slider.value = newValue;
        Loading_text.text = $"{prefix} { (int)newValue } %";
    }

    public void WaitForSeconds(Action callback)
    {
        StartCoroutine(WaitForSecondsCoroutine(callback));
    }

    private IEnumerator WaitForSecondsCoroutine(Action callback)
    {
        yield return new WaitForSeconds(timeToFinishLoading);

        callback?.Invoke();
    }

    public void ResetView()
	{
        SceneLoading = default;
        UpdateLoadingView(0);
    }
}