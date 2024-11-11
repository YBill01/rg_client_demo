using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using Legacy.Database;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Legacy.Client.WindowBehaviour;

namespace Legacy.Client
{
    public class WindowManager : MonoBehaviour
    {
        public static WindowManager Instance = null;

        [SerializeField]
        private GameObject CrossSceneCanvasPrefab;

        private GameObject CrossSceneCanvasObject = null;

        [SerializeField]
        public Canvas MainCanvas;

        [SerializeField]
        private CanvasScaler CanvasScaler;

        [SerializeField]
        public MainWindowBehaviour MainWindow;

        [SerializeField]
        private WindowBehaviour FirstWindow = null;

        [SerializeField]
        private RectTransform Render3D;

        public List<WindowBehaviour> openedWindows = new List<WindowBehaviour>();
        public List<WindowBehaviour> notInitedWindows = new List<WindowBehaviour>();

        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private GraphicRaycaster[] graphicRaycasterArr;

        public void EnableEventSystem()
        {
            eventSystem.enabled = true;
            eventTrigger.enabled = true;
            foreach (var item in graphicRaycasterArr)
            {
                item.enabled = true;
            }

            //PushNotifications.Instance.Init();
        }

        public void DisableEventSystem()
        {
            eventSystem.enabled = false;
            eventTrigger.enabled = false;
            foreach (var item in graphicRaycasterArr)
            {
                item.enabled = false;
            }
        }

        internal void SetUpPanelConfig(int config = -1)
        {
            if (config < 0)
            {
                UpPanel.Setup();
            }
            else
            {
                UpPanel.Setup(config);
            }
        }
        internal void SetUpPanelNextLevel()
        {
            UpPanel.AccountUpdateLevel();
        }

        public UpPanelBehaviour GetUpPanel()
        {
            return UpPanel;
        }
        [SerializeField]
        private UpPanelBehaviour UpPanel;

        public void ShowBack(bool v)
        {
            UpPanel.ShowBack(v);
        }
        void Awake()
        {
            Instance = this;
            LoadingGroup.Instance.LoadingEnabled.AddListener(DisableEventSystem);
            LoadingGroup.Instance.LoadingDisabled.AddListener(EnableEventSystem);
        }
        public void ShowErrorWindow(byte index)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("index", (index).ToString());
            NotWait();
            OpenWindow(Windows[21], settings);
        }
        public void LevelUpPlayerWindow()
        {
            if(CurrentWindow is AppExitWindowBehaviour)
            {
                ClosePopUp();
            }
            NotWait();
            OpenWindow(Windows[15]);
        }

        internal void InitWindows(Action callback)
        {
            UpPanel.Init();

            foreach (var window in Windows)
            {
                var windowComponent = window.GetComponent<WindowBehaviour>();
                windowComponent.Init(() =>
                {
                    notInitedWindows.Remove(window);
                });
                windowComponent.Close();
                if (windowComponent.type == WindowType.CrossScene)
                {
                    if (CrossSceneCanvasObject == null)
                    {
                        CrossSceneCanvasObject = Instantiate(CrossSceneCanvasPrefab);
                        var canvas = CrossSceneCanvasObject.GetComponent<Canvas>();
                        canvas.worldCamera = MainCanvas.worldCamera;
                        canvas.sortingLayerName = "CrossScene";
                    }
                    windowComponent.transform.SetParent(CrossSceneCanvasObject.transform, false);
                }
            }
            callback();
            GetComponent<Animator>().Play("Alpha1");
            /*if (BattleDataContainer.Instance.CheckForNewArena())
            {
                OpenWindow(Windows[1]);
                BattleDataContainer.Instance.NewArenaShown();
            }*/
        }

        private Action _onBuyNotEnoughCoinsWindow = null;
        public void CloseNotEnoughCoinsWindow()
        {
            if (_onBuyNotEnoughCoinsWindow != null)
                _onBuyNotEnoughCoinsWindow();
            _onBuyNotEnoughCoinsWindow = null;
        }
        public void OpenNotEnoughCoinsWindow(uint countNeed, Action callback = null)
		{
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("count", (countNeed).ToString());
            OpenWindow(Windows[19], settings);
            _onBuyNotEnoughCoinsWindow = callback;
        }
        
        public void ClosePopUp()
        {
            if (CurrentWindow.type == WindowType.PopUp)
            {
                PreviousWindow = CurrentWindow;
                CurrentWindow.Close();
                openedWindows.Remove(CurrentWindow);
                CurrentWindow = CurrentWindow.parent;
                UpPanel.ShowNewReward(CurrentWindow is MainWindowBehaviour);
                SoftTutorialManager.Instance.CheckTutorialsForCurrentWindow();
            }
        }

        internal void ShakeCamera()
        {
            MainCanvas.worldCamera.GetComponent<Animator>().Play("CameraBattleStartShake");
        }

        private bool _waitOpen = true;
        private IEnumerator WaitOpen()
        {
            yield  return new WaitForSeconds(.5f);
            _waitOpen = true;
        }

        public void NotWait()
        {
            StopCoroutine(WaitOpen());
            _waitOpen = true;
        }
        public void OpenWindow(WindowBehaviour window, Dictionary<string, string> settings = null)
        {
            UpPanel.ShowNewReward(window is MainWindowBehaviour);

            if (!_waitOpen) return;
            if (window == CurrentWindow) return;
            _waitOpen = false;
            StartCoroutine(WaitOpen());
            if (!openedWindows.Contains(window))
            {
                openedWindows.Add(window);
            }
            PreviousWindow = CurrentWindow;
            window.SetParent(window is MainWindowBehaviour ? null : CurrentWindow);
            if (window.type != WindowType.PopUp)
            {
                MenuHeroesBehaviour.Instance.EnableRender(window.Need3DHero);
                MainBGBehaviour.Instance.ResetToDefault();
                if (CurrentWindow != null)
                {
                    CurrentWindow.Close();
                    openedWindows.Remove(CurrentWindow);
                }
                if (window.HideUpPanel)
                {
                    UpPanel.Hide();
                }
                else
                {
                    SetUpPanelConfig(window.GetUpPanelConfig());
                }
                CanvasScaler.matchWidthOrHeight = window.ScaleMatch;
            }
            else
            {
                //ClosePopUp();

                /*if (window.gameObject.GetComponent<Canvas>() == null)
                {
                    var canvas = window.gameObject.AddComponent<Canvas>();
                    canvas.sortingLayerName = "PopUpWindows";
                    window.gameObject.AddComponent<GraphicRaycaster>();
                    canvas.overrideSorting = true;
                }*/

                //return;
            }

            if (window.Need3DHero)
            {
                if (window.ParentFor3D != null)
                {
                    Set3DRenderParent(window.ParentFor3D);
                    Render3D.GetComponent<Render3DBehaviour>().SetScale(window.Scale3DRender);
                    Render3D.GetComponent<EventTrigger>().enabled = !window.CanRotateHero;
                }
            }
            if(settings != null)
            {
                window.OnSettings(settings);
            }
            window.Open();
            SoftTutorialManager.Instance.CheckTutorialsForCurrentWindow(); 
        }

        internal void Set3DRenderParent(RectTransform parentFor3D)
        {
            Render3D.SetParent(parentFor3D);
        }

        public WindowBehaviour CurrentWindow;
        public WindowBehaviour PreviousWindow;

        public void Home()
        {
			if (!_waitOpen)
			{
                return;
			}

            foreach (WindowBehaviour win in openedWindows)
            {
                win.Close();
            }
            CurrentWindow = null;
            openedWindows = new List<WindowBehaviour>();
            OpenWindow(MainWindow);
        }

        public List<WindowBehaviour> Windows;

        public bool IsCliCkBack = true; // блокировка кнопки Back (esc) на телефоне
        internal void Back()
        {
            if (!IsCliCkBack) return;
            if (!_waitOpen)
            {
                return;
            }

            if(CurrentWindow.type == WindowType.PopUp)
            {
                ClosePopUp();
                return;
            }
             OpenWindow(CurrentWindow.parent);

        }

        internal void StartMenu()
        {
            OpenWindow(FirstWindow ?? MainWindow);

            CheckNeedDownloadAddressables();
        }

		private void CheckNeedDownloadAddressables()
		{
            if (ClientWorld.Instance.Profile.Rating.current < 500)
            {
                return;
            }

            Addressables.GetDownloadSizeAsync("server").Completed += op =>
            {
                GameDebug.Log($"Check GetDownloadSize: {op.Result}");

				if (op.Result > 0)
				{
					NotWait();
					OpenWindow(Windows[17]);
				}
            };
        }

        internal void OpenFailPurchasePopup(ObserverPlayerPaymentResult paymentResult)
        {
            GameDebug.Log("OpenPopuPFailPurchaseWindow");
        }
    }
}