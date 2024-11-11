using Legacy.Client;
using Legacy.Database;
using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[ExecuteInEditMode]
public class HeroDamageBar : MinionHealthBar
{
    [SerializeField] private RectTransform heroSlider;
    [SerializeField] Animator animator;
    public UnityEvent SliderPartEnded;
    public HealthSliderBehaviour Slider1;
    public HealthSliderBehaviour Slider2;
    public HealthSliderBehaviour Slider3;

    public Color32 redColorFill;
    public StarsBehaviour Stars;
    public AudioSource audioSource;
    public MinionSoundManager soundManager;

    public TextMeshProUGUI HPText;
    public int starsCountOfState = 0;
    private bool needsToHideBar = true;

    public enum State
    {
        One, Two, Three, Dead
    }

    public float Value = 0f;
    public float CurrentValueInCurrentState = 0f;

    private float MinOne = 0.6667f;
    private float MinTwo = 0.3334f;
    public float MinThree = 0.0001f;

    /// <summary>
    /// Sets true from battle interface start animator
    /// </summary>
    public bool animationFinish = false;

    private State previousState;
    public State state;

    public byte finalValue { get; private set; }
    public byte startValue { get; private set; }

    public override void SetValue(float value, bool shouldView = true, GameObject heroGameObject = null, MinionLayerType _layer = MinionLayerType.Ground)
    {
        //Debug.Log("New value: " + value);
        //Debug.Log("Current value: " + Value);
        // if (Value == value) return;

        if (heroGameObject != null && Value > value && Value > 0)
        {
            var damageEffect = heroGameObject.GetComponent<DamageEffect>();
            if (shouldView)
            {
                damageEffect?.Punch();
            }
        }

        ActivateHealthBar(true);
        HPText.text = ((int)Health).ToString();

        if (animator != null && Value > 0)
        {
            if (Value > value)
            {
                animator.Play("RedHeroFlash");
            }
            else
            {
                animator.Play("GreenHeroFlash");
            }

        }

        Value = value;
        GetState();
        switch (state)
        {
            case State.One:
                Slider1.SetValue(CurrentValueInCurrentState);
                Slider2.SetValue(1.0f);
                Slider3.SetValue(1.0f);
                starsCountOfState = 0;
                break;
            case State.Two:

                Slider1.GetComponent<Transform>().Find("Background").gameObject.SetActive(false);
                Slider3.GetComponent<Transform>().Find("Background").gameObject.SetActive(true);
                Slider2.GetComponent<Transform>().Find("Background").gameObject.SetActive(true);
                // Stars.Explosion(1);
                if (!BattleInstanceInterface.instance.IsGameReloaded)
                    if (previousState == State.One)
                    {
                        if (starsCountOfState == 0)
                            soundManager.PlayHpBarState1(audioSource);
                        starsCountOfState = 1;
                        SliderPartEnded.Invoke();
                    }
                Slider1.SetValue(0.0f);
                Slider2.SetValue(CurrentValueInCurrentState);
                Slider3.SetValue(1.0f);
                break;
            case State.Three:
                Slider1.GetComponent<Transform>().Find("Background").gameObject.SetActive(false);
                Slider2.GetComponent<Transform>().Find("Background").gameObject.SetActive(false);
                Slider3.GetComponent<Transform>().Find("Background").gameObject.SetActive(true);

                if (!BattleInstanceInterface.instance.IsGameReloaded)
                    if (previousState == State.Two)
                    {
                        if (starsCountOfState == 1)
                            soundManager.PlayHpBarState2(audioSource);
                        starsCountOfState = 2;
                        SliderPartEnded.Invoke();

                    }
                Slider1.SetValue(0.0f);
                Slider2.SetValue(0.0f);
                Slider3.SetValue(CurrentValueInCurrentState);
                break;
            case State.Dead:

                Slider1.GetComponent<Transform>().Find("Background").gameObject.SetActive(false);
                Slider2.GetComponent<Transform>().Find("Background").gameObject.SetActive(false);
                Slider3.GetComponent<Transform>().Find("Background").gameObject.SetActive(false);
                //gameObject.SetActive(false);
                //     Stars.Explosion(3);
                if (!BattleInstanceInterface.instance.IsGameReloaded)
                    if (previousState == State.Three)
                    {
                        if (starsCountOfState == 2)
                            soundManager.PlayHpBarState3(audioSource);
                        starsCountOfState = 3;
                    }
                SliderPartEnded.Invoke();
                Slider1.SetValue(0.0f);
                Slider2.SetValue(0.0f);
                Slider3.SetValue(0.0f);
                break;
            default:
                break;
        }
        byte newval = (byte)(Math.Min((byte)state, (byte)State.Three) + 1);
        if (newval > finalValue)
        {
            //брать финальное значение
            finalValue = newval;//не заходит finalValue всегда = 3!!!!

        }
    }
    private void GetState()
    {
        if (Value > MinOne)
        {
            float delta = Value - MinOne;
            CurrentValueInCurrentState = delta / (1 - MinOne);
            state = State.One;
        }
        else if (Value > MinTwo)
        {
            float delta = Value - MinTwo;
            CurrentValueInCurrentState = delta / (MinOne - MinTwo);
            previousState = State.One;
            state = State.Two;
        }
        else if (Value > MinThree)
        {
            float delta = Value - MinThree;
            CurrentValueInCurrentState = delta / (MinTwo - MinThree);
            previousState = State.Two;
            state = State.Three;
        }
        if (Value <= 0)
        {
            CurrentValueInCurrentState = 0f;
            previousState = State.Three;
            state = State.Dead;
        }
    }

    public void HideHpInTutorial()
    {
        var _query_is_tutorial = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<TutorialInstance>());
        if (!_query_is_tutorial.IsEmptyIgnoreFilter)
            HPText.gameObject.SetActive(false);
    }

    public override void SetLevel(byte level)
    {
    }

    public override void DeActivateHealthBar()
    {
    }

    public void HideHpBar()
    {
        heroSlider.transform.localScale = new Vector3(0, 1, 1);
        HPText.transform.localScale = Vector3.zero;
    }

    public void PlayCustomClip(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public override void ActivateHealthBar(bool flag)
    {
        if (minionPanel && minionPanel.IsEnemy)
        {
            HPText.color = redColorFill;
            Slider1.SetRedSprites();
            Slider2.SetRedSprites();
            Slider3.SetRedSprites();
        }
        if (needsToHideBar && !ClientWorld.Instance.GetOrCreateSystem<StateMachineSystem>().IsConnectedTooExistedBattle)
        {
            HideHpBar();
        }
    }
    public void ScaleInitHpBar()
    {
        var sequence = DOTween.Sequence()
            .Append(HPText.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).SetEase(Ease.InQuad))
            .Append(HPText.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad));
        
        heroSlider.DOScaleX(0.23f, 0.15f);

        needsToHideBar = false;
    }
}
