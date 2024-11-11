/*using UnityEngine;
using Legacy.Client;
using UnityEngine.UI;
using Legacy.Database;
using TMPro;

public class RageButtonBehaviour : MonoBehaviour
{
	[SerializeField]
	private Image fader;
	[SerializeField]
	private Image effectMask;
	[SerializeField]
	private Image icon;
	[SerializeField]
	private SkillChargeEffectBehaviour chargeEffect;

	[SerializeField]
	private GameObject effectSkillPrepared;

	[SerializeField]
	private TextMeshProUGUI TimerText;
	[SerializeField]
	private GameObject TimerObject;

	[SerializeField]
	private Animator animator;

	private HeroRageSettings settings;
	private bool isCharging;
	private bool isReady;

	private void Start()
	{
		settings = Settings.Instance.Get<HeroRageSettings>();
	}

	public void UpdateSkill(int time, byte myBridgesCount, float skillSpeed)
	{
		float cooldown = settings.cooldown - settings.skill2_cooldown;
		var fill = Mathf.Clamp01(time / cooldown);
		fader.fillAmount = fill;
		effectMask.fillAmount = 1 - fill;

		var allSkillsReady = (settings.cooldown - settings.skill2_cooldown - time) > 0;

		TimerObject.SetActive(time > 0 && allSkillsReady);
		TimerText.text = ((byte)(time / skillSpeed / 1000)).ToString();

		SetAnimatorParams(time, allSkillsReady);
		SetGray(time > 0);

		effectSkillPrepared.SetActive(time == 0);

		chargeEffect.UpdateChargeEffect(allSkillsReady, time == 0, myBridgesCount);
	}

	public void OnButtonClick()
	{
		ServerWorld.Instance.ActionPlayRage();
		TutorialMessageBehaviour.MakeTapEvent();
	}

	private void SetGray(bool value)
	{
		icon.material = value ? VisualContent.Instance.GrayScaleMaterial : null;
		//Frame.material = value ? VisualContent.Instance.GrayScaleMaterial : null;
	}

	private void SetAnimatorParams(int time, bool skillsReady)
	{
		if (skillsReady)
		{
			if (!isCharging)
			{
				isCharging = true;
				animator.SetBool("Charging", isCharging);
			}
		}
		else
		{
			if (isCharging)
			{
				isCharging = false;
				animator.SetBool("Charging", isCharging);
			}
		}

		if (time == 0)
		{
			if (!isReady)
			{
				isReady = true;
				animator.SetBool("Ready", isReady);
			}
		}
		else
		{
			if (isReady)
			{
				isReady = false;
				animator.SetBool("Ready", isReady);
			}
		}
	}
}*/