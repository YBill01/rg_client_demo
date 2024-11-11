using Legacy.Client;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTapSkill : MonoBehaviour
{
    [SerializeField]
    private TutorialMessageBehaviour tutorialMessage;

	private BattleSkillBehaviour chosenSkill;
	private BattleSkillBehaviour[] skills;
	private void ShowSkills()
	{
		skills = tutorialMessage.StaticColliders.GetComponentsInChildren<BattleSkillBehaviour>(true);
        try
        {
		    skills[0].transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        }
        catch
        {
            Debug.Log("Got itS");
        }
		foreach (var cc in skills)
		{
			cc.gameObject.SetActive(true);
			if (cc.Index == tutorialMessage.binaryTutorialEvent.param_0)
			{
				chosenSkill = cc;
				continue;
			}
			cc.GetComponent<BattleSkillBehaviour>().SkillView.MakeGray(true);
		}
	}

	[SerializeField]
	private GameObject handPrefab;
	private GameObject pointerPrefabInstance;
	private void ShowHand()
	{
		pointerPrefabInstance = GameObject.Instantiate(handPrefab, tutorialMessage.transform);

		var hb = pointerPrefabInstance.GetComponent<TutorialPointerBehaviour>();
		Vector2 statrps = RectTransformUtility.WorldToScreenPoint(BattleInstanceInterface.instance.UICamera, chosenSkill.GetComponent<RectTransform>().position);
		hb.startPosition = statrps;
	}

	[SerializeField]
	private GameObject tutorialCardPrefab;

	private TutorialCard tutorialSkillInstance;
	private Button but;
	private void ShowSkill()
	{
		tutorialSkillInstance = GameObject.Instantiate(tutorialCardPrefab, tutorialMessage.transform).GetComponent<TutorialCard>();
		tutorialSkillInstance.simple = true;
		tutorialSkillInstance.CopySkillView(chosenSkill.gameObject);
		tutorialSkillInstance.gameObject.SetActive(false);
	}

	private void OnSkillClick()
	{
		tutorialSkillInstance.taskDoneEvent.RemoveListener(OnSkillClick);
		tutorialSkillInstance.ClearAll();
		DestroyImmediate(tutorialSkillInstance);
		DestroyImmediate(pointerPrefabInstance);
		chosenSkill.gameObject.SetActive(true);
		DoSkillApply();
		tutorialSkillInstance = null;
		pointerPrefabInstance = null;

		foreach (var cc in skills)
		{
			if (cc == chosenSkill) continue;
			cc.GetComponent<BattleSkillBehaviour>().SkillView.MakeGray(false);
		}

		TutorialMessageBehaviour.MakeTapEvent();
	}

	private void SetupDragBehaviour()
	{
		OnSkillDone();
	}

	private GameObject minionPrefab;
	private void OnSkillDone()
	{
		var rc = tutorialSkillInstance.GetComponent<RectTransform>();
		var rc2 = chosenSkill.GetComponent<RectTransform>();

		rc.localScale = rc2.localScale;
		rc.position = rc2.position;

		chosenSkill.gameObject.SetActive(false);
		tutorialSkillInstance.gameObject.SetActive(true);
		tutorialSkillInstance.taskDoneEvent.AddListener(OnSkillClick);
	}

	#region place minion
	private void DoSkillApply()
	{
		chosenSkill.Use();
	}
	#endregion
}
