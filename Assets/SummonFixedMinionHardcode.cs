using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SummonFixedMinionHardcode : MonoBehaviour
{
    [SerializeField]
    private TutorialMessageBehaviour tutorialMessage;

    public bool anyWhere;
    public bool HardPosition = true; // false - позволяет выставлять в любом месте карту, а не по указанным координатам. 
    [SerializeField]
    private GameObject tutorialCardPrefab;
    [SerializeField]
    private GameObject spawnUnitPlacePrefab;
    private GameObject spawnUnitPlaceInstance;

    private BattleCardDragBehaviour chosenCard;
    private bool chosenCardIsBlockedOnStart;
    private BattleCardDragBehaviour[] cards;

    [SerializeField]
    private GameObject handPrefab;
    private GameObject pointerPrefabInstance;
    /*
	[SerializeField]
	private GameObject framePrefab;
	private GameObject framePrefabInstance;*/

    private BoxCollider plane;
    private TutorialCard tutorialCardInstance;
    private GameObject minionPrefab;
    private RectTransform choosenRect;
    private TutorialPointerBehaviour hb;
    private bool isEnoughtMana;
    [SerializeField] private bool playIfHaveMana;
    [SerializeField] private float prefabMana;

    private EntityQuery _query_battle;

    void OnEnable()
    {
        _query_battle = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
    }

    private void ShowSpawnPlace()
    {
        spawnUnitPlaceInstance = GameObject.Instantiate(spawnUnitPlacePrefab, StaticColliders.instance.transform);
        spawnUnitPlaceInstance.gameObject.SetActive(false);
        //LegacyHelpers.SetAlpha(spawnUnitPlaceInstance, 0);
    }

    private void ShowCard()
    {
        cards = BattleInstanceInterface.instance.Cards;

        foreach (var cc in cards)
        {
            if (cc.IndexInHand == tutorialMessage.binaryTutorialEvent.param_0)
            {
                chosenCard = cc;
                var messageEvent = tutorialMessage.binaryTutorialEvent;
                if (HardPosition)
                    cc.CustomSpawnPosition = new Vector2(messageEvent.param_x, messageEvent.param_y);

                chosenCardIsBlockedOnStart = cc.IsBlockedByTutorial;
                cc.IsBlockedByTutorial = false;

                continue;
            }
            //cc.GetComponent<CardViewBehaviour>().MakeGray(true);
            //cc.transform.localScale *= 0.95f;
        }
        /*tutorialCardInstance = GameObject.Instantiate(tutorialCardPrefab, tutorialMessage.transform).GetComponent<TutorialCard>();
		tutorialCardInstance.CopyCardView(chosenCard.gameObject);
		tutorialCardInstance.gameObject.SetActive(false);*/
    }
    private void OnDisable()
    {
        if (chosenCard == null)
        {
            cards = BattleInstanceInterface.instance.Cards;

            foreach (var cc in cards)
            {
                if (cc.IndexInHand == tutorialMessage.binaryTutorialEvent.param_0)
                {
                    chosenCard = cc;
                }
            }
        }
        chosenCard.IsBlockedByTutorial = chosenCardIsBlockedOnStart;
    }
    private void OnDestroy()
    {
        if (chosenCard == null)
        {
            cards = BattleInstanceInterface.instance.Cards;

            foreach (var cc in cards)
            {
                if (cc.IndexInHand == tutorialMessage.binaryTutorialEvent.param_0)
                {
                    chosenCard = cc;
                }
            }
        }
        chosenCard.IsBlockedByTutorial = chosenCardIsBlockedOnStart;
    }

    private void Update()
    {
        if (_query_battle.IsEmptyIgnoreFilter)
            return;
        var _entity = _query_battle.GetSingletonEntity();

        var _instance = ClientWorld.Instance.EntityManager.GetComponentData<BattleInstance>(_entity);
        if (_instance.status >= BattleInstanceStatus.Result)
        {
           if(hb) hb.gameObject.SetActive(false);
            return;

        }

        if (hb && choosenRect && !chosenCard.IsDrag && chosenCard.IsEnoughtMana() && !playIfHaveMana ||
          hb && choosenRect && !chosenCard.IsDrag && chosenCard.IsEnoughtMana() && playIfHaveMana && ManaUpdateSystem.PlayerMana >= prefabMana)
        {
            hb.gameObject.SetActive(true);
            Vector2 startPos = RectTransformUtility.WorldToScreenPoint(BattleInstanceInterface.instance.UICamera, choosenRect.position);

            hb.startPosition = startPos;
        }
        if (hb && choosenRect && !chosenCard.IsDrag && !chosenCard.IsEnoughtMana() && !playIfHaveMana ||
            hb && choosenRect && !chosenCard.IsDrag && playIfHaveMana && ManaUpdateSystem.PlayerMana < prefabMana)
        {
            hb.gameObject.SetActive(false);
        }
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
        choosenRect = chosenCard.GetComponent<RectTransform>();
        hb = pointerPrefabInstance.GetComponent<TutorialPointerBehaviour>();
        hb.plane = plane;
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
			ObjectPooler.instance.GetMinion(entity, OnMinionDone);
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
		//framePrefabInstance = null;
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
			(byte)chosenCard.hand_index,
			new float2(tutorialCardInstance.dragPrefab.transform.position.x, tutorialCardInstance.dragPrefab.transform.position.z)
		);
		ManaUpdateSystem.setImmediatelly = true;
	}
	#endregion*/
}
