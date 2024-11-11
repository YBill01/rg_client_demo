using DG.Tweening;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class HomeTutorPanelBehaviour : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI desc;
        [SerializeField] private TextMeshProUGUI desc2;
        [SerializeField] private ProgressBarChangeValueBehaviour ProgressBar;
        [SerializeField] private Transform view;
       // [SerializeField] private Transform TitleView;
        [SerializeField] private Transform DescriptionView;
        [SerializeField] private Transform DescriptionView2;
        [SerializeField] private Transform ProgressBarView;
        [SerializeField] private Transform ProgressBarTxt;
        [SerializeField] private TextMeshProUGUI progressText1;
        [SerializeField] private TextMeshProUGUI progressText2;
        [SerializeField] private Transform ProgressBarTxt1;
        private ProfileInstance profile;

        private bool isClose = false;
        private bool isNotAnimation = false;
        public void Init()
        {            
            WindowManager.Instance.MainWindow.GetBattlePassButton.transform.parent.gameObject.SetActive(false);
            WindowManager.Instance.MainWindow.GetBattlePassButton.gameObject.SetActive(false);
            if (profile == null)
                profile = ClientWorld.Instance.Profile; 
            if (!profile.IsBattleTutorial )
            {
                if (profile.battleStatistic.battles > 0 ||
                    !BattleDataContainer.Instance.CheckForNewArenaForChanged())
                {
                    isClose = true;
                    transform.parent.gameObject.SetActive(false);
                    WindowManager.Instance.MainWindow.GetBattlePassButton.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
            SoftTutorialManager.Instance.MenuTutorialPointer.HidePointerTemporary();

            if (profile.IsBattleTutorial &&  profile.battleStatistic.tutor_defeat_3 > 0)
            {
                view.localScale = Vector3.zero;
                isNotAnimation = true;
            }
              
            transform.parent.gameObject.SetActive(true);

            WindowManager.Instance.MainWindow.GetBattlePassButton.gameObject.SetActive(false);
            WindowManager.Instance.GetUpPanel().ShowNewReward(false);
            WindowManager.Instance.GetUpPanel().ShowArena(false);

            /*   if (!profile.IsBattleTutorial)
               {
                   return;
               }*/
            desc.text = Locales.Get("locale:6404");


            NewDescript((byte)(profile.HardTutorialState));
            

            //ProgressBar.Set(profile.HardTutorialState, true, (uint)Tutorial.Instance.TotalCount());
            ProgressBar.SetDelitel((uint)Tutorial.Instance.TotalCount());
            

            if (profile.HardTutorialState >= 3)
            {
                DescriptionView.gameObject.SetActive(false);
                desc2.alignment = TextAlignmentOptions.Center;
            }
            if (!profile.IsBattleTutorial)
            {
                NewDescript((byte)(0));
            }
            //  view.DOScale(Vector3.zero, 1f);
         



            //  OnShow();
         //   StartCoroutine(WaitAnim());

        }


        public void OnStart()
        {
            if (show)
            {
                return;
            }
            if (isActiveAndEnabled)
            {
                StartCoroutine(WaitAnim());
            }
        }

         private IEnumerator WaitAnim()
       {
            
            float delay = .9f; 
            if(isNotAnimation)
                delay = .1f;
            yield return new WaitForSeconds(delay);
            //show = true;
            //  OnStart();
            OnShow();
        }


        private void NewDescript(byte id)
        {
            switch (id)
            {
                case 0: desc2.text = Locales.Get("locale:6173"); break;
                case 1: desc2.text = Locales.Get("locale:6152"); break;
                case 2: desc2.text = Locales.Get("locale:6159"); break;
                case 3: desc2.text = Locales.Get("locale:6166"); break;
                case 4: desc2.text = Locales.Get("locale:6173"); break;
            }
        }

        [Header("Animation")]
        [SerializeField] private bool show = false;
        bool isShow = false;
        [Header("Animation Up")]
        [SerializeField, Range(0.0f, 1.0f)] private float ScaleSpeedUp;
        [SerializeField] private Ease ShowUpViewType;
        [SerializeField] private Vector3 ShowUpSize;
        [Header("Animation Down")]
        [SerializeField, Range(0.0f, 1.0f)] private float ScaleSpeedDown;
        [SerializeField] private Ease ShowDownViewType;
        [SerializeField] private Vector3 ShowDownSize;
        [Header("Animation Progress")]
        [SerializeField, Range(0.0f, 1.0f)] private float ScaleSpeesProgress;
        [SerializeField, Range(0.0f, 1.0f)] private float SpeedProgress;
        [SerializeField] private Ease ShowProgressType;
        [SerializeField] private Vector3 ShowProgressSize;

        [Header("Animation Progress Text")] // прогресс бар 1/4
        [SerializeField, Range(0.0f, 1.0f)] private float SpeedProgressText;
        [SerializeField] private Ease ProgressTextType;
        [SerializeField] private Vector3 ProgressTextSize;

        [Header("Animation Progress Text Number")] // прогресс бар первая цифра
        [SerializeField, Range(0.0f, 1.0f)] private float SpeedProgressNumerUpText;
        [SerializeField] private Ease ProgressNumerUpTextType;
        [SerializeField] private Vector3 ProgressNumerUpTextSize;
        [SerializeField, Range(0.0f, 1.0f)] private float SpeedProgressNumerDownText;
        [SerializeField] private Ease ProgressNumerDownTextType;

        [Header("Animation Descript")]
       // [SerializeField, Range(0.0f, 1.0f)] private float SpeedDescriptonDown;
      //  [SerializeField] private Ease DescriptDownSizeType;
        [SerializeField, Range(0.0f, 1.0f)] private float SpeedDescriptonUp;
        [SerializeField] private Ease DescriptUpType;
        [SerializeField, Range(0.0f, 1.0f)] private float SpeedAlphaLeftDescription;
        [SerializeField, Range(0.0f, 1.0f)] private float SpeedAlphaLeftDescription2;


     /*   private void Update()
        {
            if (show!=isShow)
            {
                isShow = show;
                OnShow();
            } 
        }*/

        public void OnHide()
        {
            Vector3 _size = ShowDownSize;
            Ease aninType = ShowDownViewType;
            float speed = ScaleSpeedDown;
            DOTween.Sequence()
           .Append(view.DOScale(Vector3.zero, speed).SetEase(aninType))
           .AppendCallback(() =>
           {
               //меняем первое значение 1->2
               transform.parent.gameObject.SetActive(false);
           });
            isClose = true;
            MainMenuArenaChangeBehaviour.Instance.Enable();
            WindowManager.Instance.MainWindow.GetBattlePassButton.transform.parent.gameObject.SetActive(true);
        }
      /*  public void Hide()
        {
            if (isFinishAnimShow)
            {
                OnHide();
            }
            else
            {
                StartCoroutine(WaitHide());
            }
        }*/
      /*  private IEnumerator WaitHide()
        {
            while (!isFinishAnimShow)
            {
                yield return new WaitForSeconds(.2f);
            }
            OnHide();
        }*/

        private bool isFinishAnimShow = false;
        private bool isShowAnim = false;
        private void OnShow()
        {
            if (isShowAnim) return;
            if (isClose) return;
           
            isShowAnim = true;
            /*     Vector3 _size = ShowDownSize;
                 Ease aninType= ShowDownViewType;
                 float speed = ScaleSpeedDown;
                 if (isShow)
                 {
                     _size = ShowUpSize;
                     aninType = ShowUpViewType;
                     speed = ScaleSpeedUp;
                 }
                 else
                 {
                     if (profile.IsBattleTutorial) return;
                     DOTween.Sequence()
                         .Append(view.DOScale(_size, speed).SetEase(aninType));//скаил плашки
                     return;
                 }*/
            Vector3 _size = ShowUpSize;
            Ease aninType = ShowUpViewType;
            float speed = ScaleSpeedUp;
            if (isNotAnimation)
            {
                view.localScale = Vector3.one;
                ProgressBar.SetAnim((uint)(profile.HardTutorialState), (uint)Tutorial.Instance.TotalCount());
                progressText1.text = (profile.HardTutorialState).ToString();
                progressText2.text = Tutorial.Instance.TotalCount().ToString();
                return;
            }
            view.localScale = Vector3.zero;
            progressText1.text = (profile.HardTutorialState - 1).ToString();
            progressText2.text = Tutorial.Instance.TotalCount().ToString();

            ProgressBar.SetAnim((uint)(profile.HardTutorialState - 1), (uint)Tutorial.Instance.TotalCount());
            ProgressBarView.localScale = new Vector3(0,1,1);
            DescriptionView2.localScale = Vector3.zero;
            desc.DOFade(0f,0f);
            desc2.DOFade(0f,0f);
            DOTween.Sequence()
            //скаил плашки + титл
            .Append(view.DOScale(_size, speed).SetEase(aninType))
            //скаил прогресс бар
            .Append(ProgressBarView.DOScaleX(ShowProgressSize.x, ScaleSpeesProgress).SetEase(ShowProgressType).OnComplete(() =>
               {
                   //зеленая полоска прогресс бара
                   ProgressBar.SetAnim(profile.HardTutorialState, (uint)Tutorial.Instance.TotalCount(), SpeedProgress, null/* NewTitle*/);
               }))
             //скаил цифр 1/2 вверх
             .Append(ProgressBarTxt.DOScale(ProgressTextSize, SpeedProgressText)).SetEase(ProgressTextType)
             .AppendCallback(() =>
             {
                 //меняем первое значение 1->2
                 progressText1.text = (profile.HardTutorialState).ToString();
             })
              // скаил первой цифры прогресс бара
              .Append(ProgressBarTxt1.DOScale(ProgressNumerUpTextSize, SpeedProgressNumerUpText)).SetEase(ProgressNumerUpTextType)
              // скаил первой цифры прогресс бара вниз
              .Append(ProgressBarTxt1.DOScale(Vector3.one, SpeedProgressNumerDownText)).SetEase(ProgressNumerDownTextType)
              // cкаил 1/2 назад
              .Append(ProgressBarTxt.DOScale(Vector3.one, SpeedProgressText))
              .Append(desc.DOFade(1f, SpeedAlphaLeftDescription))
              .Append(DescriptionView2.DOScale(Vector3.one, SpeedDescriptonUp).SetEase(DescriptUpType))
              .Join(desc2.DOFade(1f, SpeedAlphaLeftDescription2))
               .AppendCallback(() =>
               {

                   if (!profile.IsBattleTutorial)
                   {
                       OnHide();
                   }
                   else
                   {
                       SoftTutorialManager.Instance.MenuTutorialPointer.UnhidePointer();
                   }
               });

            #region вариант ГД Саши
            /*
             вариант ГД Саши
                        DOTween.Sequence()
                            .Append(view.DOScale(_size, speed).SetEase(aninType))//скаил плашки
                            .Append(ProgressBarView.DOScale(ShowProgressSize, ScaleSpeesProgress).SetEase(ShowProgressType).OnComplete(() => //скаиk порогресс бара
                            {
                                ProgressBar.SetAnim(profile.HardTutorialState, (uint)Tutorial.Instance.TotalCount(), SpeedProgress, NewTitle);// запуск заполнения прогресс бара
                            }))
                             // .Append(ProgressBarTxt.DOScale(ProgressTextSize, SpeedProgressText)).SetEase(ProgressTextType).OnComplete(() =>//скаил текста прогресс бара
                             //  {
                               //  NewDescript((byte)(profile.HardTutorialState));
                                 //  progressText1.text = (profile.HardTutorialState).ToString();
                            //   })


                             .Append(ProgressBarTxt1.DOScale(ProgressNumerUpTextSize, SpeedProgressNumerUpText)).SetEase(ProgressNumerUpTextType).OnComplete(() =>    //скаил текста прогресс 2 вверхбара
                              {

                              })
                              .AppendCallback(() =>
                              {
                                  NewDescript((byte)(profile.HardTutorialState));
                                  progressText1.text = (profile.HardTutorialState).ToString();
                              })
                            .Append(ProgressBarTxt1.DOScale(Vector3.one, SpeedProgressNumerDownText)).SetEase(ProgressNumerDownTextType)     //скаил текста прогресс 2 возврат

                            .Append(ProgressBarTxt.DOScale(ProgressTextSize, SpeedProgressText)).SetEase(ProgressTextType).OnComplete(() =>//скаил текста прогресс бара
                            {
                                //  NewDescript((byte)(profile.HardTutorialState));
                                //  progressText1.text = (profile.HardTutorialState).ToString();
                            })
                             .AppendCallback(() =>
                             {
                                 NewDescript((byte)(profile.HardTutorialState));
                             })
                             .Append(ProgressBarTxt.DOScale(Vector3.one, SpeedProgressText)) //возврат скаил текста прогресс бара   
                             .AppendCallback(() =>
                             {
                                 progressText1.text = (profile.HardTutorialState).ToString();
                             })
                             .Append(DescriptionView2.DOScale(Vector3.zero, SpeedDescriptonDown).SetEase(DescriptDownSizeType)).OnComplete(() => // показ новой локали
                              {

                              })
                            .Append(DescriptionView2.DOScale(Vector3.one, SpeedDescriptonUp).SetEase(DescriptUpType)); // показ новой локали

                          //  .Join(DescriptionView2.DOScale(Vector3.zero, SpeedDescriptonDown).SetEase(DescriptDownSizeType).OnComplete(() =>//скрытие текстовки
                             {
                                 NewDescript((byte)(profile.HardTutorialState));//новая текстовка
                          //   }));
                           */
            #endregion
        }

       /* public void NewTitle()
        {

        }*/

    }

}