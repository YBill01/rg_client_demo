using Legacy.Client;
using Legacy.Database;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialSkillHardCode : MonoBehaviour
{
	[SerializeField]
	private TutorialMessageBehaviour tutorialMessage;

	[SerializeField]
	private bool isFixedPosition;

	private GameObject heroUnit;
	private void ShowSkills()
	{
		skills = tutorialMessage.StaticColliders.GetComponentsInChildren<BattleSkillDragBehaviour>();
		skills[0].transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
		foreach (var cc in skills)
		{
			cc.gameObject.SetActive(true);
			if (cc.SkillIndex == tutorialMessage.binaryTutorialEvent.param_0)
			{
				chosenSkill = cc;

				if (isFixedPosition)
				{
					var messageEvent = tutorialMessage.binaryTutorialEvent;
					cc.CustomSpawnPosition = new Vector2(messageEvent.param_x, messageEvent.param_y);
				}
				continue;
			}
			//cc.GetComponent<BattleSkillBehaviour>().SkillView.MakeGray(true);
			//cc.transform.localScale *= 0.95f;
		}
	}

	[SerializeField]
	private GameObject tutorialCardPrefab;
	[SerializeField]
	private GameObject skillPlacePrefab;
	private GameObject skillUsePlaceInstance;
	private void ShowSkillPlace()
	{
		skillUsePlaceInstance = GameObject.Instantiate(skillPlacePrefab, tutorialMessage.StaticColliders.transform);
		skillUsePlaceInstance.SetActive(false);
		//LegacyHelpers.SetAlpha(skillUsePlaceInstance, 0);
	}

	public float sceneDragBorder = 5;
	public float sceneOverBorderDrag = 6;

	private BattleSkillDragBehaviour chosenSkill;
	private BattleSkillDragBehaviour[] skills;
	private void ShowSkill()
	{
		/*tutorialSkillInstance = GameObject.Instantiate(tutorialCardPrefab, tutorialMessage.transform).GetComponent<TutorialCard>();
		tutorialSkillInstance.CopySkillView(chosenSkill.gameObject);
		tutorialSkillInstance.gameObject.SetActive(false);
		tutorialSkillInstance.clampPosX = new Vector2(-12f, 12f);
		tutorialSkillInstance.sceneDragBorder = sceneDragBorder;
		tutorialSkillInstance.sceneOverBorderDrag = sceneDragBorder;*/
	}

	[SerializeField]
	private GameObject handPrefab;
	private GameObject pointerPrefabInstance;

	/*[SerializeField]
	private GameObject framePrefab;
	private GameObject framePrefabInstance;*/
	private void ShowHand()
	{
		//framePrefabInstance = GameObject.Instantiate(framePrefab, tutorialMessage.StaticColliders.transform);
		pointerPrefabInstance = GameObject.Instantiate(handPrefab, tutorialMessage.transform);

		var lst = tutorialMessage.StaticColliders.GetComponentsInChildren<BoxCollider>(true);
		foreach (var l in lst)
		{
			if (l.gameObject.name == "TouchCollider")
			{
				plane = l;
				break;
			}
		}

		var hb = pointerPrefabInstance.GetComponent<TutorialPointerBehaviour>();
		hb.plane = plane;
		hb.sceneUnit = skillUsePlaceInstance.GetComponentsInChildren<MeshRenderer>(true)[0].transform.parent;

		Vector2 statrps = RectTransformUtility.WorldToScreenPoint(BattleInstanceInterface.instance.UICamera, chosenSkill.GetComponent<RectTransform>().position);
		
		hb.startPosition = statrps;
	}

	private void SetupDragBehaviour()
	{
		//OnSkillDone(chosenSkill.DragObject);
		var pss = skillUsePlaceInstance.GetComponentsInChildren<MeshRenderer>(true)[0].gameObject;
		pss.transform.parent.position = new Vector3(tutorialMessage.binaryTutorialEvent.param_x, 0.2f, tutorialMessage.binaryTutorialEvent.param_y);
		skillUsePlaceInstance.SetActive(true);
	}

	private void OnDestroy()
	{
		Destroy(skillUsePlaceInstance);
	}

	private BoxCollider plane;
	private TutorialCard tutorialSkillInstance;
	private GameObject minionPrefab;
	/*
	private void OnSkillDone(GameObject skillDragObject)
	{
		minionPrefab = skillDragObject;
		tutorialSkillInstance.dragPrefab = minionPrefab;
		skillUsePlaceInstance.SetActive(true);
		var pss = skillUsePlaceInstance.GetComponentsInChildren<MeshRenderer>(true)[0].gameObject;
		pss.transform.parent.position = new Vector3(tutorialMessage.binaryTutorialEvent.param_x, 0.2f, tutorialMessage.binaryTutorialEvent.param_y);
		var mr = pss.transform;
		tutorialSkillInstance.pointTarget = mr.parent.gameObject;
		tutorialSkillInstance.magnetTarget = mr.gameObject;

		tutorialSkillInstance.plane = plane;
		var rc = tutorialSkillInstance.GetComponent<RectTransform>();
		var rc2 = chosenSkill.GetComponent<RectTransform>();

		rc.localScale = rc2.localScale;
		rc.position = rc2.position;

		chosenSkill.gameObject.SetActive(false);
		tutorialSkillInstance.gameObject.SetActive(true);
		tutorialSkillInstance.taskDoneEvent.AddListener(onSkillCast);
		tutorialSkillInstance.ChangeStateEvent.AddListener(OnDragChangeState);
	}

	private void OnDragChangeState(TutorialCardState tutorialCardState)
	{
		if (tutorialCardState != TutorialCardState.Default)
		{
			pointerPrefabInstance.SetActive(false);
			return;
		}
		pointerPrefabInstance.SetActive(true);
	}

	private void onSkillCast()
	{
		tutorialSkillInstance.ClearAll();
		pointerPrefabInstance.GetComponent<TutorialPointerBehaviour>().DestroyItself();
		skillPlacePrefab.GetComponent<Animator>().SetTrigger("Close");
		DestroyImmediate(pointerPrefabInstance);
		DestroyImmediate(tutorialSkillInstance);
		DestroyImmediate(skillUsePlaceInstance);
		chosenSkill.gameObject.SetActive(true);
		DoSkillApply();
		framePrefabInstance = null;
		tutorialSkillInstance = null;
		skillUsePlaceInstance = null;

		foreach (var cc in skills)
		{
			if (cc == chosenSkill) continue;
			cc.GetComponent<BattleSkillBehaviour>().SkillView.MakeGray(false);
		}

		TutorialMessageBehaviour.TapMessage();

	}

	private void Update()
	{
		if (skills == null) return;
		if (skills.Length == 0) return;
		foreach (var cc in skills)
		{
			if (cc == chosenSkill) continue;
			cc.GetComponent<BattleSkillBehaviour>().SkillView.MakeGray(true);
		}
	}

	#region place minion
	private void DoSkillApply()
	{
		chosenSkill.Use();
		//chosenSkill.UseSkill(Vector3.zero);
	}
	#endregion*/
}
