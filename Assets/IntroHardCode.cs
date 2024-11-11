using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroHardCode : MonoBehaviour
{
	#region Resources Links
	[SerializeField]
	private TutorialMessageBehaviour tutorialMessage;
	[SerializeField]
	private PlaceTutorialUnitScript tutorialUnitScript;

	[SerializeField]
	private GameObject targetHighlightPrefab;
	[SerializeField]
	private GameObject allyHighlightPrefab;
	[SerializeField]
	private GameObject enemyHighlightPrefab;
	[SerializeField]
	private GameObject planePrefab;
	[SerializeField]
	private Material allyMaterial;
	[SerializeField]
	private Material enemyMaterial;
	#endregion

	[SerializeField] private GameObject tapToContinue;
	[SerializeField] private float _delayBeforeTapToContinue;

	[SerializeField]
	private float autoTouchAfterSeconds;
	[SerializeField]
	private bool startBattleAfterMe;
	[SerializeField]
	private bool stopDragCardsAndSkills;

	private StaticColliders staticColliders;
	private GameObject minionsContainer;
	private bool touchStarted;

	void Start()
	{
		ResetCommonContainers();

		StartCoroutine(TapToContinueRout());

		if (autoTouchAfterSeconds != 0)
			StartCoroutine(MakeTouchAfter(autoTouchAfterSeconds));

		if (stopDragCardsAndSkills)
		{
			for (int i = 0; i < 4; ++i)
			{
				var card = BattleInstanceInterface.instance.Cards[i];
				card.TutorialStopUseCard();
			}

			BattleInstanceInterface.instance.Skill1.TutorialStopUseSkill();
			BattleInstanceInterface.instance.Skill2.TutorialStopUseSkill();
		}
	}

	private IEnumerator MakeTouchAfter(float autoTouchAfterSeconds)
	{
		yield return new WaitForSeconds(autoTouchAfterSeconds);

		CloseMessage();
	}

	private IEnumerator TapToContinueRout()
    {
		yield return new WaitForSeconds(_delayBeforeTapToContinue);
		tapToContinue.SetActive(true);
	}

	private void CloseMessage()
	{
		TutorialMessageBehaviour.MakeTapEvent();
		if (startBattleAfterMe)
		{
			StartBattleAnimation.instance.ShowInterface();
			ClientWorld.Instance.StartLocalBattle();
		}
	}

	#region Init Containers
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

	#region Update
	void Update()
	{
		if (Input.GetMouseButtonDown(0) || Input.touchCount != 0)
		{
			CloseMessage();
		}

		UpdateAllySide();
		UpdateEnemySide();
		UpdateMinionPlaceState();
	}
	#endregion

	#region Show Ally Side
	private bool isAllySideShow;
	private GameObject allyZoneInstance;
	[SerializeField]
	private GameObject allyPointCameraContentPrefab;
	[SerializeField]
	private float highlightAllySpeed = 1;
	[SerializeField]
	private float highlightAllyPointerTime = 1;
	[SerializeField]
	private float highlightAllyPointerSpeed = 1;
	[SerializeField]
	private float fadeShowAllySpeed = 1;
	private float showAllySideHighlightProgress;
	private float showAllyHighlightProgress;
	private float showAllyFadeProgress;
	private float timeToAllyPointerShow = 1;
	private GameObject allyInstance;
	private GameObject allyPointerInstance;
	private GameObject allyPointCameraContentPrefabInstance;
	private bool showAllyFade;
	public void ShowAllySide()
	{
		isAllySideShow = true;
		allyZoneInstance = GameObject.Instantiate(planePrefab, minionsContainer.transform);
		var ps = allyZoneInstance.transform.localPosition;
		ps.y += 0.2f;
		allyZoneInstance.transform.localPosition = ps;
		LegacyHelpers.SetAlpha(allyZoneInstance, 0);
		showAllySideHighlightProgress = 0;
		showAllyHighlightProgress = 0;
		var mlist = minionsContainer.GetComponentsInChildren<MinionInitBehaviour>(true);
		foreach (var m in mlist)
		{
			if (!m.gameObject.activeSelf) continue;
			var a = m.GetComponent<Animator>();
			if (a == null) continue;
			if (a.avatar.name.IndexOf("Galahard") != -1)
			{
				allyInstance = m.gameObject;
				break;
			}
		}

		timeToAllyPointerShow = highlightAllyPointerTime;
		allyPointerInstance = Instantiate(allyHighlightPrefab, minionsContainer.transform);
		LegacyHelpers.SetAlpha(allyZoneInstance, 0);
		LegacyHelpers.SetAlpha(allyPointerInstance, 0);
		var npos = allyInstance.transform.position;
		npos.y += 1;
		allyPointerInstance.transform.position = npos;
	}

	private void UpdateAllySide()
	{
		if (!isAllySideShow) return;

		if (showAllyFade)
		{
			showAllyFadeProgress += Time.deltaTime * fadeShowAllySpeed;
			if (showAllyFadeProgress >= 1)
				showAllyFadeProgress = 1;
			var alphaVal = 1f - showAllyFadeProgress;
			LegacyHelpers.SetAlpha(allyPointerInstance, alphaVal);
			LegacyHelpers.SetAlpha(allyZoneInstance, alphaVal);
			if (showAllyFadeProgress >= 1)
			{
				DestroyImmediate(allyZoneInstance);
				DestroyImmediate(allyPointerInstance);
				DestroyImmediate(allyPointCameraContentPrefabInstance);
				allyZoneInstance = null;
				allyPointerInstance = null;
				allyInstance = null;
				allyPointCameraContentPrefabInstance = null;
				showAllyFade = false;
				isAllySideShow = false;
				TutorialMessageBehaviour.MakeTapEvent();
			}
			return;
		}
		if (showAllySideHighlightProgress < 1)
		{
			showAllySideHighlightProgress += Time.deltaTime * highlightSpeed;
			if (showAllySideHighlightProgress >= 1)
				showAllySideHighlightProgress = 1;
			LegacyHelpers.SetAlpha(allyZoneInstance, showAllySideHighlightProgress);
		}
		if (timeToAllyPointerShow > 0)
		{
			timeToAllyPointerShow -= Time.deltaTime;
			if (timeToAllyPointerShow > 0) return;
			timeToAllyPointerShow = 0;
			return;
		}
		if (showAllyHighlightProgress < 1)
		{
			showAllyHighlightProgress += Time.deltaTime * highlightAllyPointerSpeed;
			if (showAllyHighlightProgress >= 1)
				showAllyHighlightProgress = 1;
			LegacyHelpers.SetAlpha(allyPointerInstance, showAllyHighlightProgress);
		}

		allyPointerInstance.transform.localScale = Vector3.one * 0.7f + Mathf.Sin(Time.time * Mathf.PI) * Vector3.one * 0.1f;
		if (!touchStarted)
		{
			if (Input.GetMouseButton(0) || Input.touchCount > 0)
				touchStarted = true;
			return;
		}
		if (!Input.GetMouseButton(0) && Input.touchCount == 0)
		{
			TutorialMessageBehaviour.DoRipple();
			touchStarted = false;
			showAllyFadeProgress = 0;
			showAllyFade = true;
		}
	}
	#endregion

	#region Show Enemy Side
	private GameObject enemyZoneInstance;
	private bool isEnemySide;
	[SerializeField]
	private GameObject EnemyPointCameraContentPrefab;
	[SerializeField]
	private float highlightSpeed = 1;
	[SerializeField]
	private float highlightPointerTime = 1;
	[SerializeField]
	private float highlightPointerSpeed = 1;
	[SerializeField]
	private float fadeShowEnemySpeed = 1;
	private float showEnemySideHighlightProgress;
	private float showEnemyHighlightProgress;
	private float showEnemyFadeProgress;
	private float timeToPointerShow = 1;
	private GameObject enemyInstance;
	private GameObject enemyPointerInstance;
	private GameObject EnemyPointCameraContentPrefabInstance;
	private bool ShowEnemyFade;

	public void ShowEnemySide()
	{
		if (isEnemySide) return;
		ResetCommonContainers();

		isEnemySide = true;
		enemyZoneInstance = GameObject.Instantiate(planePrefab, minionsContainer.transform);
		var ps = enemyZoneInstance.transform.localPosition;
		ps.x = -ps.x;
		ps.y += 0.2f;
		enemyZoneInstance.transform.localPosition = ps;
		LegacyHelpers.SetAlpha(enemyZoneInstance, 0, enemyMaterial);
		showEnemySideHighlightProgress = 0;
		showEnemyHighlightProgress = 0;
		var mlist = minionsContainer.GetComponentsInChildren<MinionInitBehaviour>(true);
		foreach (var m in mlist)
		{
			if (!m.gameObject.activeSelf) continue;
			var a = m.GetComponent<Animator>();
			if (a == null) continue;
			if (a.runtimeAnimatorController.name.IndexOf("ZacZar") != -1)
			{
				enemyInstance = m.gameObject;
				break;
			}
		}

		timeToPointerShow = highlightPointerTime;
		enemyPointerInstance = Instantiate(enemyHighlightPrefab, minionsContainer.transform);
		LegacyHelpers.SetAlpha(enemyZoneInstance, 0);
		LegacyHelpers.SetAlpha(enemyPointerInstance, 0);
		var npos = enemyInstance.transform.position;
		npos.y += 1;
		enemyPointerInstance.transform.position = npos;
	}

	private void UpdateEnemySide()
	{
		if (!isEnemySide) return;
		if (ShowEnemyFade)
		{
			showEnemyFadeProgress += Time.deltaTime * fadeShowEnemySpeed;
			if (showEnemyFadeProgress >= 1)
			{
				showEnemyFadeProgress = 1;
			}
			var alphaVal = 1f - showEnemyFadeProgress;
			Debug.Log("Enemy zone Alpha is " + alphaVal);
			LegacyHelpers.SetAlpha(enemyPointerInstance, alphaVal);
			LegacyHelpers.SetAlpha(enemyZoneInstance, alphaVal);
			if (showEnemyFadeProgress >= 1)
			{
				DestroyImmediate(enemyZoneInstance);
				DestroyImmediate(enemyPointerInstance);
				DestroyImmediate(EnemyPointCameraContentPrefabInstance);
				enemyZoneInstance = null;
				enemyPointerInstance = null;
				EnemyPointCameraContentPrefabInstance = null;
				ShowEnemyFade = false;
				isEnemySide = false;
			}
			return;
		}
		if (showEnemySideHighlightProgress < 1)
		{
			showEnemySideHighlightProgress += Time.deltaTime * highlightSpeed;
			if (showEnemySideHighlightProgress >= 1)
				showEnemySideHighlightProgress = 1;
			LegacyHelpers.SetAlpha(enemyZoneInstance, showEnemySideHighlightProgress);
		}
		if (timeToPointerShow > 0)
		{
			timeToPointerShow -= Time.deltaTime;
			if (timeToPointerShow > 0) return;
			timeToPointerShow = 0;
			return;
		}
		if (showEnemyHighlightProgress < 1)
		{
			showEnemyHighlightProgress += Time.deltaTime * highlightPointerSpeed;
			if (showEnemyHighlightProgress >= 1)
			{
				tutorialMessage.Animator.SetTrigger("CurrentAct");
				tutorialMessage.NextAct();
				showEnemyHighlightProgress = 1;
			}
			LegacyHelpers.SetAlpha(enemyPointerInstance, showEnemyHighlightProgress);
		}

		//enemyPointerInstance.transform.localScale = Vector3.one * 0.7f + Mathf.Sin(Time.time * Mathf.PI) * Vector3.one * 0.1f;
		
		//if (!touchStarted)
		//{
		//	if (Input.GetMouseButton(0) || Input.touchCount > 0)
		//		touchStarted = true;
		//	return;
		//}
		//if (!Input.GetMouseButton(0) && Input.touchCount == 0)
		//{
		//	TutorialMessageBehaviour.DoRipple();
		//	touchStarted = false;
		//	showEnemyFadeProgress = 0;
		//	ShowEnemyFade = true;
		//	TutorialMessageBehaviour.TapMessage();
		//}
		//enemyInstance.transform.SetParent(minionsContainer.transform);
	}

	private void StopShowEnemySide()
	{
		showEnemyFadeProgress = 0;
		ShowEnemyFade = true;
	}

	#endregion

	#region Show Minion Place State
	private bool isMinionPlaceState;
	[SerializeField]
	private GameObject targetPointCameraContentPrefab;
	[SerializeField]
	private float highlighTargetSpeed = 1;
	[SerializeField]
	private float highlightTargetPointerTime = 1;
	[SerializeField]
	private float highlightTargetPointerSpeed = 1;
	[SerializeField]
	private float fadeShowTargetSpeed = 1;
	private float showTargetSideHighlightProgress;
	private float showTargetHighlightProgress;
	private float showTargetFadeProgress;
	private float timeToTargetPointerShow = 1;
	private GameObject targetInstance;
	private GameObject targetPointerInstance;
	private GameObject targetPointCameraContentPrefabInstance;
	private bool showTargetFade;
	[SerializeField]
	private GameObject tutorialCardPrefab;
	private TutorialCard tutorialCardInstance;
	private BattleCardBehaviour chosenCard;
	private GameObject minionPrefab;


	public void MinionPlaceState()
	{
		isMinionPlaceState = true;

		showTargetSideHighlightProgress = 0;
		showTargetHighlightProgress = 0;

		var mlist = targetPointCameraContentPrefabInstance.GetComponentsInChildren<MeshRenderer>(true);
		foreach (var m in mlist)
		{
			targetInstance = m.gameObject;
			break;
		}

		tutorialCardInstance = GameObject.Instantiate(tutorialCardPrefab, tutorialMessage.transform).GetComponent<TutorialCard>();

		var cards = staticColliders.GetComponentsInChildren<BattleCardBehaviour>(true);
		chosenCard = cards[0];
		foreach (var c in cards)
		{
			if (c.isNext) continue;
			if (c.transform.localPosition.x < chosenCard.transform.localPosition.x)
				chosenCard = c;
		}
		timeToTargetPointerShow = highlightTargetPointerTime;
		targetPointerInstance = Instantiate(targetHighlightPrefab, minionsContainer.transform);
		LegacyHelpers.SetAlpha(targetPointerInstance, 0);
		var npos = targetInstance.transform.position;
		npos.y += 1;
		targetPointerInstance.transform.position = npos;

		ushort minionDbID = chosenCard.DBCardData.entities[0];
		if (Entities.Instance.Get(minionDbID, out BinaryEntity entity))
		{
			ObjectPooler.instance.GetMinion(entity, OnMinionDone);
		}
		
	}

	private bool isMInionDanone;
	private void OnMinionDone(GameObject minion)
	{
		minionPrefab = minion;
		tutorialCardInstance.dragPrefab = minionPrefab;
		var mr = targetPointCameraContentPrefabInstance.GetComponentsInChildren<MeshRenderer>(true)[0].transform;
		tutorialCardInstance.pointTarget = mr.parent.gameObject;
		tutorialCardInstance.magnetTarget = mr.gameObject;
		var lst = staticColliders.GetComponentsInChildren<BoxCollider>(true);
		foreach(var l in lst)
		{
			if(l.gameObject.name == "TouchCollider")
			{
				tutorialCardInstance.plane = l;
				break;
			}
		}
		var rc = tutorialCardInstance.GetComponent<RectTransform>();
		var rc2 = chosenCard.GetComponent<RectTransform>();

		//LegacyHelpers.CopyTransform(rc, rc2);

		rc.position = rc2.position;
		rc.localScale = rc2.localScale;

		chosenCard.gameObject.SetActive(false);
		isMInionDanone = true;
		tutorialCardInstance.taskDoneEvent.AddListener(onMinionPLace);
	}

	private void onMinionPLace()
	{
		tutorialCardInstance.taskDoneEvent.RemoveListener(onMinionPLace);
		tutorialMessage.Animator.SetTrigger("NextAct");
		showTargetFade = true;
		chosenCard.gameObject.SetActive(true);
		showTargetFadeProgress = 0;
	}

	private void UpdateMinionPlaceState()
	{
		if (!isMinionPlaceState) return;
		if (!isMInionDanone) return;

		if (showTargetFade)
		{
			showTargetFadeProgress += Time.deltaTime * fadeShowTargetSpeed;
			if (showTargetFadeProgress >= 1)
				showTargetFadeProgress = 1;
			var alphaVal = 1f - showTargetFadeProgress;
			LegacyHelpers.SetAlpha(targetPointerInstance, alphaVal);
			if (showTargetFadeProgress >= 1)
			{
				#region place minion

				var _active_world = World.DefaultGameObjectInjectionWorld;
				var _visualization = _active_world.GetOrCreateSystem<MinionGameObjectInitializationSystem>();

				_visualization.Spawned(chosenCard.DBCardData.entities[0], tutorialCardInstance.dragPrefab);
				ObjectPooler.instance.MinionBack(tutorialCardInstance.dragPrefab);

				ClientWorld.Instance.ActionPlay(
					PlayerGameMessage.ActionCard,
					(byte)chosenCard.hand_index,
					new float2(tutorialCardInstance.dragPrefab.transform.position.x, tutorialCardInstance.dragPrefab.transform.position.z)
				);
				tutorialCardInstance.dragPrefab.gameObject.SetActive(false);
				ManaUpdateSystem.setImmediatelly = true;

				#endregion

				DestroyImmediate(targetPointerInstance);
				DestroyImmediate(targetPointCameraContentPrefabInstance);
				targetPointerInstance = null;
				targetInstance = null;
				targetPointCameraContentPrefabInstance = null;
				showTargetFade = false;
				isMinionPlaceState = false;
				//TutorialMessageBehaviour.TapMessage();
			}
			return;
		}
		if (showTargetSideHighlightProgress < 1)
		{
			showTargetSideHighlightProgress += Time.deltaTime * highlightSpeed;
			if (showTargetSideHighlightProgress >= 1)
				showTargetSideHighlightProgress = 1;
		}
		if (timeToTargetPointerShow > 0)
		{
			timeToTargetPointerShow -= Time.deltaTime;
			if (timeToTargetPointerShow > 0) return;
			timeToTargetPointerShow = 0;
			return;
		}
		if (showTargetHighlightProgress < 1)
		{
			showTargetHighlightProgress += Time.deltaTime * highlightTargetPointerSpeed;
			if (showTargetHighlightProgress >= 1)
				showTargetHighlightProgress = 1;
			LegacyHelpers.SetAlpha(targetPointerInstance, showTargetHighlightProgress);
		}

		targetPointerInstance.transform.localScale = Vector3.one * 0.7f + Mathf.Sin(Time.time * Mathf.PI) * Vector3.one * 0.1f;
	}
	#endregion
}
