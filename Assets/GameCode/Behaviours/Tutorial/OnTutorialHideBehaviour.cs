using UnityEngine;
using System.Collections;
using Legacy.Client;
using Legacy.Database;

public class OnTutorialHideBehaviour : MonoBehaviour
{
	[SerializeField]
	int ShowAfterBattleTutorial = 1;
	[SerializeField]
	int ShowAfterMenuTutorial = 1;

    [Header("Lock With Locale")]
    [SerializeField] bool OnlyLockWithLocale = false;
    [SerializeField] string LocaleKey = "locale:1483";
    [SerializeField] LegacyButton button;

	private void Start()
	{
		//MenuTriggerSystem.MenuTutorialStepEvent.AddListener(OnNewTutorialStep);
		OnNewTutorialStep(default(BinaryMenuTutorialEvent));
	}

	public void OnDestroy()
	{
		//MenuTriggerSystem.MenuTutorialStepEvent.RemoveListener(OnNewTutorialStep);
	}

	void OnNewTutorialStep(BinaryMenuTutorialEvent tutorialEvent)
	{
		if (!ClientWorld.Instance.Profile.IsBattleTutorial)
		{
			return;
		}

		var battleTutorialCondition = ClientWorld.Instance.Profile.HardTutorialState >= ShowAfterBattleTutorial;
		var menuTutorialCondition = ClientWorld.Instance.Profile.MenuTutorialState >= ShowAfterMenuTutorial;
        bool active = battleTutorialCondition && menuTutorialCondition;
        if (OnlyLockWithLocale)
        {
            gameObject.SetActive(true);
            if(button != null)
            {
                button.interactable = active;
                button.isLocked = !active;
                button.localeAlert = Locales.Get(LocaleKey);
            }
        }
        else
        {
            gameObject.SetActive(active);
        }
    }
}
