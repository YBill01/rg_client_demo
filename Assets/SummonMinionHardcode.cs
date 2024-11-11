using Legacy.Client;
using Legacy.Database;
using Unity.Mathematics;
using UnityEngine;

public class SummonMinionHardcode : MonoBehaviour
{
	[SerializeField]
	private TutorialMessageBehaviour tutorialMessage;

	public bool anyWhere;

	[SerializeField]
	private GameObject tutorialCardPrefab;
	[SerializeField]
	private GameObject spawnUnitPlacePrefab;
	private GameObject spawnUnitPlaceInstance;

	private BattleCardDragBehaviour chosenCard;
	private BattleCardDragBehaviour[] cards;

	[SerializeField]
	private GameObject handPrefab;
	private GameObject pointerPrefabInstance;

	/*[SerializeField]
	private GameObject framePrefab;
	private GameObject framePrefabInstance;*/

	private BoxCollider plane;

	private void ShowSpawnPlace()
	{
		spawnUnitPlaceInstance = GameObject.Instantiate(spawnUnitPlacePrefab, tutorialMessage.StaticColliders.transform);
		spawnUnitPlaceInstance.gameObject.SetActive(false);
		//LegacyHelpers.SetAlpha(spawnUnitPlaceInstance, 0);
	}

	private void ShowCard()
	{
		cards = BattleInstanceInterface.instance.Cards;
		
		foreach(var cc in cards)
		{
			if(cc.IndexInHand == tutorialMessage.binaryTutorialEvent.param_0)
			{
				chosenCard = cc;
				continue;
			}
			//cc.GetComponent<CardViewBehaviour>().MakeGray(true);
			//cc.transform.localScale *= 0.95f;
		}
		/*tutorialCardInstance = GameObject.Instantiate(tutorialCardPrefab, tutorialMessage.transform).GetComponent<TutorialCard>();
		tutorialCardInstance.CopyCardView(chosenCard.gameObject);
		tutorialCardInstance.gameObject.SetActive(false);*/
	}

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
		//hb.dragFrame = framePrefabInstance;
		hb.sceneUnit = spawnUnitPlaceInstance.GetComponentsInChildren<MeshRenderer>(true)[0].transform.parent;
		Vector2 startPos = RectTransformUtility.WorldToScreenPoint(BattleInstanceInterface.instance.UICamera, chosenCard.GetComponent<RectTransform>().position);

		hb.startPosition = startPos;
	}

	private void SetupDragBehaviour()
	{
		var pss = spawnUnitPlaceInstance.GetComponentsInChildren<MeshRenderer>(true)[0].transform;
		pss.transform.parent.position = new Vector3(tutorialMessage.binaryTutorialEvent.param_x, 0.2f, tutorialMessage.binaryTutorialEvent.param_y);
		var mr = pss.transform;
		/*ushort minionDbID = chosenCard.DBCardData.entities[0];

		if (Entities.Instance.Get(minionDbID, out BinaryEntity entity))
		{
			ObjectPooler.instance.GetMinion(entity.prefab, OnMinionDone);
		}*/
	}
	/*
	private void OnMinionDone(GameObject minion)
	{
		minionPrefab = minion;
		tutorialCardInstance.dragPrefab = minionPrefab;
		minionPrefab.transform.Rotate(new Vector3(0, 90, 0));
		spawnUnitPlaceInstance.gameObject.SetActive(true);
		var pss = spawnUnitPlaceInstance.GetComponentsInChildren<MeshRenderer>(true)[0].transform;
		pss.transform.parent.position = new Vector3(tutorialMessage.binaryTutorialEvent.param_x, 0.2f, tutorialMessage.binaryTutorialEvent.param_y);
		var mr = pss.transform;
		tutorialCardInstance.pointTarget = mr.parent.gameObject;
		tutorialCardInstance.magnetTarget = mr.gameObject;

		tutorialCardInstance.plane = plane;
		tutorialCardInstance.placeAnywhere = anyWhere;
		var rc = tutorialCardInstance.GetComponent<RectTransform>();
		var rc2 = chosenCard.GetComponent<RectTransform>();

		rc.localScale = rc2.localScale;
		rc.position = rc2.position;

		chosenCard.gameObject.SetActive(false);
		tutorialCardInstance.gameObject.SetActive(true);
		tutorialCardInstance.taskDoneEvent.AddListener(OnMinionPlace);
		tutorialCardInstance.ChangeStateEvent.AddListener(OnDragChangeState);
	}

	private void OnDragChangeState(TutorialCardState tutorialCardState)
	{
		if(tutorialCardState != TutorialCardState.Default)
		{
			pointerPrefabInstance.SetActive(false);
			return;
		}
		pointerPrefabInstance.SetActive(true);
	}
	
	private void OnMinionPlace()
	{
		tutorialCardInstance.ClearAll();
		pointerPrefabInstance.GetComponent<TutorialPointerBehaviour>().DestroyItself();
		spawnUnitPlacePrefab.GetComponent<Animator>().SetTrigger("Close");
		DestroyImmediate(pointerPrefabInstance);
		DestroyImmediate(tutorialCardInstance);
		DestroyImmediate(spawnUnitPlaceInstance);
		DoMinionApply();
		framePrefabInstance = null;
		tutorialCardInstance = null;
		spawnUnitPlaceInstance = null;

		chosenCard.gameObject.SetActive(true);
		foreach (var cc in cards)
		{
			if (cc == chosenCard) continue;
			cc.GetComponent<CardViewBehaviour>().MakeGray(false);
			cc.transform.localScale *= 1.053f;
		}
	
	}

	#region place minion
	private void DoMinionApply()
	{
		var _active_world = ClientWorld.DefaultGameObjectInjectionWorld;
		var _visualization = _active_world.GetOrCreateSystem<MinionGameObjectInitializationSystem>();

		_visualization.Spawned(chosenCard.DBCardData.entities[0], tutorialCardInstance.dragPrefab);
		ObjectPooler.instance.MinionBack(tutorialCardInstance.dragPrefab);

		ServerWorld.Instance.ActionPlay(
			PlayerGameMessage.ActionCard,
			chosenCard.hand_index,
			new float2(tutorialCardInstance.dragPrefab.transform.position.x, tutorialCardInstance.dragPrefab.transform.position.z)
		);
		ManaUpdateSystem.ManaToUse = 0;
		ManaUpdateSystem.PlayerMana -= chosenCard.DBCardData.manaCost;
		ManaUpdateSystem.setImmediatelly = true;
	}
	#endregion
	*/
}
