using DG.Tweening;
using Legacy.Client;
using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class BattleCardDragBehaviour : MonoBehaviour
{
    [SerializeField]
    private int indexInHand;
    [SerializeField]
    public RectTransform nextCard;
    [SerializeField] private AudioClip cantDragCard;
    [SerializeField] private AudioClip startDragCard;
    [SerializeField] private AudioClip deloyDragCard;
    private const float selectedCardUpAt = 30f;
    private const float maxDelta = 80f;
    private const float manaDebt = 0.3f; // Если для выставления юнита не хватает столько маны, то визуал позволит его выставит и дождется набора нужно количества маны

    private RectTransform rectTransform;
    private Canvas rootCanvas;
    private Collider allyZone1;
    private Collider allyZone2;
    private Collider allZone;
    private Plane groundPlane;
    private BattleCardViewBehaviour viewBehaviour;
    private BattleCardManaBehaviour manaBehaviour;

    private Vector3 startScale;
    private Vector2 startPosition;
    private Vector2 startDragPosition;
    private Vector2 startPivot;
    private Vector2 selectedPosition;
    private Vector2 touchStartPosition;
    private Vector2 touchLocalPosition;// позиция прикосновения относительно самой карты

    private int currentTouchId = int.MaxValue;

    private Dictionary<ushort, List<GameObject>> spawnDictionary = new Dictionary<ushort, List<GameObject>>();
    private Entity zoneHighlight;
    private Entity Dragging;

    private bool isHidden = true;
    private bool isDrag;
    private bool isSelected;
    private bool isMinionOnField;
    private bool isInited = false;
    private bool isGoLikeNewCard = false;

    private Transform dragObject;
    private EntityManager EntityManager;
    private EntityQuery battleInstanceQuery;

    private BinaryCard binaryCard;
    private bool isCardIsPartizan;
    private IEnumerator delayedUseCardCoroutine;
    private List<GameObject> spawningMinions = new List<GameObject>();

    //For tutorial
    public Vector2 CustomSpawnPosition;
    //For tutorial
    public bool IsBlockedByTutorial;
    public bool isSandbox;

    public bool IsHidden => isHidden;
    public bool IsDrag => isDrag;
    public bool IsInited => isInited;
    public bool IsGoLikeNewCard => isGoLikeNewCard;
    public int IndexInHand { get => indexInHand; set { indexInHand = value; } }
    public BinaryCard BinaryCard => binaryCard;

    public class BattleCardRightClick : UnityEvent<ushort> { };
    public BattleCardRightClick onRightClick = new BattleCardRightClick();

    public void Init()
    {
        if (isInited)
            return;

        rectTransform = GetComponent<RectTransform>();
        rootCanvas = BattleInstanceInterface.instance.canvas.GetComponent<Canvas>();

        allyZone1 = StaticColliders.instance.AllyZone.transform.GetChild(0).GetComponent<MeshCollider>();
        allyZone2 = StaticColliders.instance.AllyZone.transform.GetChild(1).GetComponent<MeshCollider>();
        allZone = StaticColliders.instance.AllZone.GetComponent<MeshCollider>();
        groundPlane = StaticColliders.instance.GroundPlane;

        viewBehaviour = GetComponent<BattleCardViewBehaviour>();
        manaBehaviour = GetComponent<BattleCardManaBehaviour>();

        SetStartPos();

        dragObject = new GameObject($"DragObject_{indexInHand}").transform;
        dragObject.SetParent(transform.root);

        battleInstanceQuery = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
        isInited = true;

        viewBehaviour.SetGray(true);
        viewBehaviour.Glow(false);
        EntityManager = ClientWorld.Instance.EntityManager;

    }

    public void Unhide()
    {
        isHidden = false;
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        GoLikeNewCard();
    }

    // Нужно спрятать карту и сбросить индекс
    // Что бы раздать новую карту 
    public void TutorialResetCard()
    {
        while (dragObject.childCount > 0)
        {
            var child = dragObject.GetChild(0);
            child.SetParent(ObjectPooler.instance.Minions.transform);
            child.gameObject.SetActive(false);
        }
        StartCoroutine(DelayedReset());
    }

    // Мы показываем большое сообщение и хотим прекратить перетаскивание карты и прекратить задрежанное выставление юнита
    public void TutorialStopUseCard()
    {
        if (delayedUseCardCoroutine != null)
        {
            StopCoroutine(delayedUseCardCoroutine);
            delayedUseCardCoroutine = null;

            foreach (var minion in spawningMinions)
                minion.SetActive(false);
        }

        if (isDrag)
        {
            isDrag = false;
            currentTouchId = int.MaxValue;
            Deselect();
            SetPivot(rectTransform, startPivot);
            dragObject.gameObject.SetActive(false);
        }
        else if (isSelected)
        {
            Deselect();
        }
    }

    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(0.3f);
        isHidden = true;
        binaryCard.index = 0;
    }

    public void Hide()
    {
        isHidden = true;

        isSelected = false;
        viewBehaviour.Glow(false);
        SetupDropZone(false);

        ManaUpdateSystem.ManaSelected -= binaryCard.manaCost;

        // Мы не прячем карту через gameObject.SetActive(false);
        // Потому чт онам нужна работа короутины
        // Потому задвигаем карту подальше
        rectTransform.localPosition = new Vector3(-100000, -100000, 0);
    }

    void SetStartPos()
    {
        startScale = rectTransform.localScale;
        startPosition = rectTransform.localPosition;
        selectedPosition = startPosition + new Vector2(0, selectedCardUpAt);
        startPivot = rectTransform.pivot;
    }

    public void UpdateCardData(ushort db_id)
    {
        if (Cards.Instance.Get(db_id, out binaryCard))
        {
            InitDragObject();
            viewBehaviour.Init(binaryCard);
            manaBehaviour.Init(binaryCard.manaCost);
        }
    }

    private void Update()
    {
        if (IsBlockedByTutorial)
        {
            StopDrag(new Vector2(50, 50), true);
            return;
        }

        if (isHidden)
            return;

        if (battleInstanceQuery.IsEmptyIgnoreFilter)
            return;

        var _battle = battleInstanceQuery.GetSingleton<BattleInstance>();

        isSandbox = _battle.isSandbox;
        //Если бой закончился - сбрасываем драг карты
        var canDragCardsIfIsNotReloaded = !BattleInstanceInterface.instance.IsGameReloaded && _battle.status != BattleInstanceStatus.Playing && _battle.status != BattleInstanceStatus.Pause;
        var canDragCardsIfIsReloaded = BattleInstanceInterface.instance.IsGameReloaded && _battle.status > BattleInstanceStatus.Pause;
       
        if (canDragCardsIfIsNotReloaded || canDragCardsIfIsReloaded)
        {
            if (isDrag)
                StopDrag(new Vector2(50, 50));//Мы прост осбрасываем таскание карты. И куда мы сбросим юнит - не важно

            return;
        }

        if (!Input.touchSupported)
        {
            #region PC input proccessing

            //Для песочницы
            if (isSandbox)
            {
                if (Input.GetMouseButtonDown(1) && IsClickOnMe(out Vector2 localPosition))
                {
                    onRightClick?.Invoke(binaryCard.index);
                }

                if (SandboxUnitEditorBehaviour.isOpen)
                    return;
            }

            //Кнопка не нажата нам нечего тут делать
            if (!Input.GetMouseButton(0))
            {
                //Но если карту таскали - все возвращаем как было
                if (isDrag)
                {
                    StopDrag(Input.mousePosition);
                }
                return;
            }

            //Кликнули по карте
            if (Input.GetMouseButtonDown(0))
            {
                if (IsClickOnMe(out Vector2 localPosition))
                {
                    if (isSelected)
                    {
                        Deselect();
                        return;
                    }

                    Select((Vector2)Input.mousePosition);
                    touchLocalPosition = localPosition;
                }
                else
                {
                    if (!isSelected)
                        return;

                    if (IsClickOnOtherCard())
                    {
                        Deselect();
                    }
                    else
                    {
                        if (TryUseCard(Input.mousePosition))
                        {
                            if (isSandbox)
                                Deselect();
                            else
                                Hide();
                        }
                    }
                }
                return;
            }

            if (isSelected && !isDrag && Input.GetMouseButton(0)/* && IsClickOnMe(out Vector2 localClickPosition)*/)
            {
                var dragDelta = (Vector2)Input.mousePosition - touchStartPosition;

                if (dragDelta.sqrMagnitude < 1)
                    return;

                StartDrag(touchLocalPosition);

                return;
            }

            if (!isDrag)
                return;

            Drag((Vector2)Input.mousePosition);
            #endregion
        }
        else
        {
            //#else
            #region Mobile input proccessing

            var touches = Input.touches;

            if (touches.Length == 0)
                return;

            if (currentTouchId == int.MaxValue)
            {
                // Есть ли клики не по карте(любой карте)
                bool anyNonCardTouch = false;
                Touch nonCardTouch = default;
                // Тут такая выкрученная логика так как выставление юнита должно происходить по любому клику даже за рамками боевой арены
                // Потому для выделеной карты - любой клик не по картам - выставление юнита

                bool anyTouchBegan = false;

                foreach (var touch in touches)
                {
                    if (touch.phase != TouchPhase.Began)
                        continue;

                    anyTouchBegan = true;

                    if (IsTouchOnMe(touch, out Vector2 localPosition))
                    {
                        //if (touch.phase == TouchPhase.Began)
                        currentTouchId = touch.fingerId;
                        touchStartPosition = touch.position;
                        touchLocalPosition = localPosition;
                        //Deselect();
                    }
                    else if (!anyNonCardTouch && !IsTouchOnAnyCard(touch))
                    {
                        nonCardTouch = touch;
                        anyNonCardTouch = true;
                    }
                }

                if (!anyTouchBegan)
                    return;

                // Кликнули куда то не по карте
                if (isSelected && anyNonCardTouch)
                {
                    if (TryUseCard(nonCardTouch.position))
                    {
                        currentTouchId = int.MaxValue;
                        Hide();
                    }
                }
                else if (currentTouchId == int.MaxValue) // Были клики а по нам не кликнули
                {
                    Deselect();
                    currentTouchId = int.MaxValue;
                }

            }
            else
            {
                var myTouch = touches.FirstOrDefault((Touch x) => x.fingerId == currentTouchId);

                if (myTouch.phase == TouchPhase.Moved)
                {
                    if (isDrag)
                    {
                        Drag(myTouch.position);
                    }
                    else
                    {
                        if (!isSelected)
                            Select(myTouch.position);

                        StartDrag(touchLocalPosition);
                    }
                }

                if (myTouch.phase >= TouchPhase.Ended)
                {
                    if (isDrag)
                    {
                        Debug.LogWarning($"StopDrag {myTouch} {myTouch.phase}");
                        StopDrag(myTouch.position);
                    }
                    else if (isSelected)
                    {
                        Debug.LogWarning($"Deselect {myTouch} {myTouch.phase}");
                        Deselect();
                    }
                    else
                    {
                        Debug.LogWarning($"Select {myTouch} {myTouch.phase}");
                        Select(myTouch.position);
                    }
                    currentTouchId = int.MaxValue;
                }
            }
            #endregion
        }

        //#endif
    }

    private bool TryUseCard(Vector2 pointerPosition)
    {
        if (!isSandbox && ManaUpdateSystem.PlayerMana /*- ManaUpdateSystem.ManaToUse*/ < binaryCard.manaCost)
        {
            if (ManaUpdateSystem.PlayerMana /*- ManaUpdateSystem.ManaToUse*/ + manaDebt < binaryCard.manaCost)
            {
                PlayClip(cantDragCard);
                PopupAlertBehaviour.ShowBattlePopupAlert(pointerPosition, Locales.Get("locale:1132"));
                return false;
            }

            //UseCardPrepare(pointerPosition);
            //delayedUseCardCoroutine = DelayedUseCard(dragObject.position, binaryCard.manaCost);
            //StartCoroutine(delayedUseCardCoroutine);
            return false;
        }

        UseCardPrepare(pointerPosition);
        UseCard(dragObject.position, binaryCard.manaCost);

        if (isSandbox)
            InitDragObject();

        return true;
    }

    private void UseCardPrepare(Vector2 pointerPosition)
    {
        ManaUpdateSystem.ManaToUse += binaryCard.manaCost;

        dragObject.gameObject.SetActive(true);
        SetDragObjectPosition(pointerPosition);

        if (CustomSpawnPosition != Vector2.zero)
        {
            dragObject.position = new Vector3(CustomSpawnPosition.x, 0, CustomSpawnPosition.y);
        }

        spawningMinions.Clear();
        while (dragObject.childCount > 0)
        {
            var minion = dragObject.GetChild(0);
            minion.SetParent(ObjectPooler.instance.Minions.transform);
            spawningMinions.Add(minion.gameObject);
        }

        var _visualization = ClientWorld.Instance.GetOrCreateSystem<MinionGameObjectInitializationSystem>();
        foreach (var pair in spawnDictionary)
        {
            for (byte i = 0; i < pair.Value.Count; i++)
            {
                var mib = pair.Value[i].GetComponent<MinionInitBehaviour>();
                mib.SetupWaitPrefab();
                _visualization.Spawned(pair.Key, pair.Value[i]);
                ObjectPooler.instance.MinionBack(pair.Value[i]);
                mib.MakeGray();
            }
        }
    }

    private void UseCard(Vector3 position, byte manaCost)
    {
        ShowSpendMana(position, manaCost);
        var pos = new float2(position.x, position.z);

        if (CustomSpawnPosition != Vector2.zero)
        {
            pos = CustomSpawnPosition;
            CustomSpawnPosition = Vector2.zero;
        }

        ClientWorld.Instance.ActionPlay(
            PlayerGameMessage.ActionCard,
            (byte)indexInHand,
            pos
        );
    }

    private IEnumerator DelayedUseCard(Vector3 position, byte manaCost)
    {
        while (ManaUpdateSystem.PlayerMana < binaryCard.manaCost)
        {
            yield return null;
        }

        UseCard(position, manaCost);
        delayedUseCardCoroutine = null;
    }

    public void FailedCardToNormalState()
    {
        Unhide();
        Deselect();
        HideWaitPrefab();
        InitDragObject();
    }

    public void HideWaitPrefab()
    {
        foreach (var pair in spawnDictionary.ToList())
        {
            for (byte i = 0; i < pair.Value.Count; i++)
            {
                if (binaryCard.entities.Contains(pair.Key))
                {
                    var mib = pair.Value[i].GetComponent<MinionInitBehaviour>();
                    mib.MinionMatsBeh.SetDefaultMaterials();
                    mib.Unspawn();

                    spawnDictionary.Remove(pair.Key);
                }
            }
        }
    }
    private void Drag(Vector2 pointerPosition)
    {
        //Высчитываем смещение карты, но при этом потянуть ниже начальной не получится
        var touchDelta = pointerPosition - touchStartPosition;
        touchDelta.y = touchDelta.y < 0 ? 0 : touchDelta.y;

        //Считаем новую позицию карте
        var newPos = startDragPosition + touchDelta;

        //Считаем скейл карты
        var scale = 1f;
        var afterThisValueStartScale = NearBorderScreenPosition() - maxDelta;
        if (pointerPosition.y > afterThisValueStartScale)
        {
            var delta = pointerPosition.y - afterThisValueStartScale;
            scale = Mathf.Clamp01((maxDelta - delta) / maxDelta);
        }

        DragCore(pointerPosition, scale, newPos);
    }

    private float NearBorderScreenPosition()
    {
        var border = StaticColliders.instance.BattleFieldNearBorder;

        var screenPos = BattleInstanceInterface.instance.MainCamera.WorldToScreenPoint(border.position);

        return screenPos.y;
    }

    private void DragCore(Vector2 pointerPosition, float scale, Vector3 newPosition)
    {
        rectTransform.localPosition = newPosition;

        transform.localScale = startScale * scale;

        //Если карта заскейлилась в ноль
        if (scale == 0)
        {
            //Показываем Драг объект
            dragObject.gameObject.SetActive(true);
            if (!isMinionOnField)
            {
                SetupAgroRadius(true);
                var children = dragObject.GetComponentsInChildren<MinionInitBehaviour>();
                foreach (var child in children)
                {
                    child.DoMinionVisible();
                    child.MakeGray();
                }
            }

            SetDragObjectPosition(pointerPosition);
            isMinionOnField = true;
        }
        else
        {
            //Прячем Драг объект
            // удаять StartDragBattleCard когда юнит выставляется на поле !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            if (isMinionOnField)
                SetupAgroRadius(false);

            dragObject.gameObject.SetActive(false);

            isMinionOnField = false;
        }
    }

    private void StartDrag(Vector2 localClickPosition)
    {
        isDrag = true;

        PlayClip(startDragCard);

        //Устанавливаем пивот в точку где игрок взял карту
        var newPivot = (localClickPosition + rectTransform.pivot * rectTransform.rect.size) / rectTransform.rect.size;

        SetPivot(rectTransform, newPivot);
        startDragPosition = rectTransform.localPosition;

        rectTransform.DOKill();
    }

    private void Deselect()
    {
        ManaUpdateSystem.ManaSelected -= binaryCard.manaCost;

        isSelected = false;
        viewBehaviour.Glow(false);
        BackToStartPos();

        SetupDropZone(false);
    }

    private void Select(Vector2 pointerPosition)
    {
        touchStartPosition = pointerPosition;

        if (isSelected)
            return;

        isSelected = true;
        viewBehaviour.Glow(true);
        ManaUpdateSystem.ManaSelected += binaryCard.manaCost;

        AnimateSelected();
        SetupDropZone(true);
    }

    private void StopDrag(Vector2 pointerPosition, bool withoutDeploySound = false)
    {
        isDrag = false;

        SetPivot(rectTransform, startPivot);

        if (isMinionOnField)
        {

            if (TryUseCard(pointerPosition))
            {
                if (isSandbox)
                {
                    Deselect();
                }
                else
                {
                    Hide();
                }
            }
            else
            {
                BackToStartPos();
            }

            SetupAgroRadius(false);
            dragObject.gameObject.SetActive(false);
        }
        else
        {
            BackToStartPos(withoutDeploySound);
        }

        isMinionOnField = false;
    }

    private void AnimateSelected()
    {
        var timeToMove = 0.15f;

        var moveTo = selectedPosition + new Vector2(0, 5f);

        var sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMove(moveTo, timeToMove));
        sequence.Append(rectTransform.DOLocalMove(selectedPosition, 0.05f));

        rectTransform.DOScale(startScale, timeToMove);
    }

    private void BackToStartPos(bool withoutDeploySound = false)
    {
        var targetPos = isSelected ? selectedPosition : startPosition;

        var timeToMove = 0.25f;

        var delta = targetPos - (Vector2)rectTransform.localPosition;
        var overmove = ((Vector2)rectTransform.localPosition) + delta * 1.05f;

        var sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMove(overmove, timeToMove));
        sequence.Append(rectTransform.DOLocalMove(targetPos, 0.05f));

        if (!withoutDeploySound)
        {
            sequence.OnStart(() =>
            {
                PlayClip(deloyDragCard);
            });
        }

        sequence.onComplete = () =>
        {
            manaBehaviour.ShowManaCostAndFader(true);
            isGoLikeNewCard = false;
        };

        sequence.onKill = () =>
        {
            isGoLikeNewCard = false;
        };

        rectTransform.DOScale(startScale, timeToMove);
    }

    private void GoLikeNewCard()
    {
        rectTransform.position = nextCard.position;
        rectTransform.localScale = Vector3.one * 0.2f;

        manaBehaviour.ShowManaCostAndFader(false);
        viewBehaviour.SetGray(true);
        isGoLikeNewCard = true;

        BackToStartPos();
    }

    private void SetDragObjectPosition(Vector2 pointerPosition)
    {
        var ray = BattleInstanceInterface.instance.MainCamera.ScreenPointToRay(pointerPosition);

        groundPlane.Raycast(ray, out float distance);

        var pos = ray.GetPoint(distance);

        Vector3 resultPos;
        if (isCardIsPartizan || isSandbox)
        {
            resultPos = allZone.ClosestPointOnBounds(pos);
        }
        else
        {
            var resultPos1 = allyZone1.ClosestPointOnBounds(pos);
            var resultPos2 = allyZone2.ClosestPointOnBounds(pos);

            resultPos = (resultPos1 - pos).sqrMagnitude < (resultPos2 - pos).sqrMagnitude ? resultPos1 : resultPos2;
        }

        if (CustomSpawnPosition != Vector2.zero)
        {
            if (resultPos.z > CustomSpawnPosition.y)
                resultPos.z = CustomSpawnPosition.y;

            if (resultPos.x > CustomSpawnPosition.x)
                resultPos.x = CustomSpawnPosition.x;
        }

        dragObject.position = resultPos;

        ChangeDragComponentPosition();

        ArenaZonesBehaviour.instance.SetRegularPosition(resultPos);
    }

    private bool IsClickOnMe(out Vector2 localClickPosition)
    {
        var ray = BattleInstanceInterface.instance.UICamera.ScreenPointToRay(Input.mousePosition);

        localClickPosition = Vector2.zero;
        var hits = Physics.RaycastAll(ray, 30f);

        if (hits.Length != 1)
            return false;

        if (hits[0].transform.parent.GetComponent<BattleCardDragBehaviour>() == null)
            return false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, BattleInstanceInterface.instance.UICamera, out localClickPosition);
        return rectTransform.rect.Contains(localClickPosition);
    }

    private bool IsTouchOnMe(Touch touch, out Vector2 localClickPosition)
    {
        var ray = BattleInstanceInterface.instance.UICamera.ScreenPointToRay(touch.position);

        localClickPosition = Vector2.zero;
        var hits = Physics.RaycastAll(ray, 30f);

        if (hits.Length != 1)
            return false;

        if (hits[0].transform.parent.GetComponent<BattleCardDragBehaviour>() == null)
            return false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touch.position, BattleInstanceInterface.instance.UICamera, out localClickPosition);
        return rectTransform.rect.Contains(localClickPosition);
    }

    private bool IsClickOnOtherCard()
    {
        var ray = BattleInstanceInterface.instance.UICamera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        return true;
    }

    private bool IsTouchOnAnyCard(Touch touch)
    {
        var ray = BattleInstanceInterface.instance.UICamera.ScreenPointToRay(touch.position);

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        return true;
    }

    private void InitDragObject()
    {
        if (isSandbox)
        { spawningMinions.Clear();
            HideWaitPrefab();
                }
        dragObject.gameObject.SetActive(false);
        SetupSquad(binaryCard.entities);
    }

    private void SetupSquad( List<ushort> DBEntities)
    {
        uint count = (uint)DBEntities.Count;
        spawnDictionary = new Dictionary<ushort, List<GameObject>>();

        if (Entities.Instance.Get(DBEntities[0], out BinaryEntity entity))
        {
            for (int i = 0; i < count; i++)
            {
                ushort eDBID = DBEntities[i];
                CreateEntityPrefab(eDBID, (obj) =>
                {
                    if (spawnDictionary.ContainsKey(eDBID))
                    {
                        spawnDictionary[eDBID].Add(obj);
                    }
                    else
                    {
                        List<GameObject> list = new List<GameObject>() { obj };
                        spawnDictionary.Add(eDBID, list);
                    }
                    obj.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                    var customPos = SquadPosUtils.GetCustomSquadUnitPosition((byte)count, (byte)i, 
                        binaryCard.squadPositionType, BattlePlayerSide.Left, entity.collider);
                    obj.transform.localPosition = new Vector3(customPos.x, 0, customPos.y);
                    obj.SetActive(true);

                    if (TryGetComponent<MinionInitBehaviour>(out var InitMinBeh))
                    {
                        InitMinBeh.DoMinionVisible(false);
                    }
                });

            }
        }
    }
    private void CreateEntityPrefab(ushort DBEntity, Action<GameObject> callback)
    {
        if (Entities.Instance.Get(DBEntity, out BinaryEntity entity))
        {
            isCardIsPartizan = false;
            foreach (var c in entity.components)
            {
                if (c == 86)
                {
                    isCardIsPartizan = true;
                    break;
                }
            }

            ObjectPooler.instance.GetMinion(entity, (obj) =>
            {
                obj.transform.SetParent(dragObject);
                var animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("Landing", false);
                    animator.SetBool("Stand", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Death", false);
                    animator.SetBool("Skill1", false);
                    animator.SetBool("Skill2", false);
                    animator.Play("Stand");
                    animator.enabled = true;
                }
                callback(obj);
            });
        }
    }

    private void ShowSpendMana(Vector3 targetPosition, byte manaCost)
    {
        if (VisualContent.Instance.customVisualData.SpendManaPrefab == null)
            Debug.LogError("SPEND MANA PREFAB NULL");

        var icon = Instantiate(VisualContent.Instance.customVisualData.SpendManaPrefab, rootCanvas.transform);
        icon.GetComponent<SpendManaBehaviour>().SetInitValues(targetPosition, manaCost);
    }

    private void SetupDropZone(bool show)
    {
        var em = ClientWorld.Instance.EntityManager;
        if (show)
        {
            zoneHighlight = em.CreateEntity();
            em.AddComponentData(zoneHighlight, new ArenaZoneHighlight { allZone = isCardIsPartizan || isSandbox });
        }
        else
        {
            em.DestroyEntity(zoneHighlight);
        }
    }

    private void ChangeDragComponentPosition()
    {
        if (EntityManager.HasComponent<StartDragBattleCard>(Dragging))
        {
            var d = EntityManager.GetComponentData<StartDragBattleCard>(Dragging);
            d.dragPosition = dragObject.position;
            d.state = 1;
            EntityManager.SetComponentData(Dragging, d);
        }
    }


    private void SetupAgroRadius(bool show)
    {
        if(!isSandbox)
        if (dragObject.gameObject.activeSelf)
            if (show)
            {
                Dragging = EntityManager.CreateEntity();
                float agroradius = 1f;
                agroradius = GetMinionAgro(agroradius);

                EntityManager.AddComponentData(Dragging, new StartDragBattleCard
                {
                    dragPosition = dragObject.position,
                    state = 1,
                    agroRadius = agroradius
                });
            }
            else
            {
                var d = EntityManager.GetComponentData<StartDragBattleCard>(Dragging);
                d.state = 0;
                EntityManager.SetComponentData(Dragging, d);
            }
    }

    private float GetMinionAgro(float agroradius)
    {
        if (Entities.Instance.Get(binaryCard.entities[0], out BinaryEntity binaryMinion))
        {
            if (Components.Instance.Get<MinionOffence>().TryGetValue(binaryMinion.index, out MinionOffence offence))
            {
                agroradius = offence.aggro;
            }
        }

        return agroradius;
    }

    /// <summary>
    /// Устанавливает пивот и корректирует положение, что бы объект не двигался.
    /// </summary>
    public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        var scale = rectTransform.localScale;
        var size = rectTransform.rect.size;
        var deltaPivot = rectTransform.pivot - pivot;

        var deltaPosition = new Vector3(
            deltaPivot.x * size.x * scale.x,
            deltaPivot.y * size.y * scale.y);

        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    private void PlayClip(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    public bool IsEnoughtMana()
    {
        return binaryCard.manaCost <= (byte)ManaUpdateSystem.PlayerMana;
    }
}
