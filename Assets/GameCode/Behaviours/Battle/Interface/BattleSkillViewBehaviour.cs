using UnityEngine;
using System.Collections;
using System;
using Legacy.Database;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using TMPro;
using Legacy.Client;
using DG.Tweening;

public class BattleSkillViewBehaviour : MonoBehaviour
{
    [SerializeField] private Image Icon;
    [SerializeField] private Image Frame;
    [SerializeField] private Image Fader;
    [SerializeField] private Image EffectMask;
    [SerializeField] private BattleSkillDragBehaviour dragBehaviour;
    [SerializeField] private SkillChargeEffectBehaviour chargeEffect;
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private GameObject TimerObject;
    [SerializeField] private AudioClip SkillReady;

    public BinarySkill binaryData;

    public bool             IsReady => _isReady;
    public Image            IconContent { get => Icon; }
    public RectTransform    SkillRect { get { if (_skillRect == null) _skillRect = GetComponent<RectTransform>(); return _skillRect; }}


    private RectTransform _skillRect;
    private bool    _isReady = true;

    internal void SetGray(bool value)
    {
        Icon.material = value ? VisualContent.Instance.GrayScaleMaterial : null;
    }

    internal void Init(BinarySkill binarySkill)
    {
        binaryData = binarySkill;
        Icon.sprite = VisualContent.Instance.SkillsIconsAtlas.GetSprite(binaryData.icon);
    }

    public void UpdateSkillView(int time, byte myBridgesCount, float skillSpeed, BattleInstanceStatus status)
    {
        var value = Mathf.Clamp01((float)(time) / dragBehaviour.skillCooldown);

		SetFaderAmount(value);
		SetGray(value > 0 || dragBehaviour.IsBlockedByTutorial);

        if (status > BattleInstanceStatus.Prepare)
            SetAnimatorParams(time == 0);

        float timeToShow = time;
        //timeToShow /= skillSpeed; // time counting with increasing speed on server regardingly to skillSpeed
        UpdateSkillTimer(timeToShow, time == 0);

        chargeEffect.UpdateChargeEffect(time == 0, myBridgesCount);
    }

    private void SetAnimatorParams(bool isFull)
    {
        if (isFull)
        {
            if (!_isReady)
            {
                _isReady = true;
                SkillButtonPunk();
            }
        }
        else
        {
            if (_isReady)
            {
                _isReady = false;
            }
        }
    }

    private void UpdateSkillTimer(float timeToShow, bool isFull)
    {
        TimerObject.SetActive(!isFull);
        TimerText.text = ((byte)(timeToShow / 1000)).ToString();
    }

    private void SetFaderAmount(float value)
    {
        Fader.fillAmount = value;
        EffectMask.fillAmount = 1f - value;
    }

    internal void Glow(bool v)
    {
        //throw new NotImplementedException();
    }

    private void SkillButtonPunk()
    {
        var punchSize = 0.2f;
        var punchDuration = 0.25f;
        var punchVibrato = 1;
        var puncElasticity = 0f;
        var scale = new Vector3(punchSize, punchSize, punchSize);
        SkillRect.DOPunchScale(scale, punchDuration, punchVibrato, puncElasticity);
    }

    public void HideSkillView()
    {
        gameObject.SetActive(false);
    }
}
