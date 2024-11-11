using DG.Tweening;
using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleSkillDragBehaviour : MonoBehaviour
{
    [SerializeField]
    private int skillIndex; // 0 или 1 скилл героя
    [SerializeField] private AudioClip cantDragSkill; // 0 или 1 скилл героя

    private const float selectedCardUpAt = 30f;
    private const float maxDelta = 80f;
    private const int timeDebt = 3000; // Если до зарядки скила не хватает столько времени, то визуал позволит его выставить и дождется его зарядки

    private RectTransform rectTransform;
    private Canvas rootCanvas;
    private Plane groundPlane;
    private BattleSkillViewBehaviour _viewBehaviour;
    public BattleSkillViewBehaviour viewBehaviour { get { if(_viewBehaviour == null) _viewBehaviour = GetComponent<BattleSkillViewBehaviour>(); return _viewBehaviour; }}

    private Vector3 startScale;
    private Vector2 startPosition;
    private Vector2 startDragPosition;
    private Vector2 startPivot;
    private Vector2 selectedPosition;
    private Vector2 touchStartPosition;
    private Vector2 touchLocalPosition;// позиция прикосновения относительно самой карты

    private int currentTouchId = int.MaxValue;
    private int currentTimerValue;

    private bool isDrag;
    private bool isSelected;
    private bool isPointerOnField;
    private bool isInited = false;

    [HideInInspector] public Transform dragObject;
    private ParticleSystem dragSkillParticle;
    private EntityQuery battleInstanceQuery;
    private IEnumerator delayedUseCardCoroutine;

    private BinarySkill binarySkill;

    //For tutorial
    public Vector2 CustomSpawnPosition;
    //For tutorial
    public bool IsBlockedByTutorial;
    public bool isSandbox;

    public bool IsInited => isInited;
    public bool IsDraged => isDrag;
    public int SkillIndex { get => skillIndex; set { skillIndex = value; } }
    public BinarySkill BinarySkill => binarySkill;

    //public int skillCooldown { get { return skillIndex == 0 ? settings.skill1_cooldown : settings.skill2_cooldown; } }
    public int skillCooldown => (int)binarySkill.cooldown.value;
    private List<ParticleSystem> pss;
    private List<MeshRenderer> mrs;
    private List<SkinnedMeshRenderer> smrs;

    private EntityManager entityManager;
    private Entity zoneHighlight;
    private void Init()
    {
        if (isInited)
            return;
        entityManager = ClientWorld.Instance.EntityManager;
        rectTransform = GetComponent<RectTransform>();
        rootCanvas = BattleInstanceInterface.instance.canvas.GetComponent<Canvas>();

        groundPlane = StaticColliders.instance.GroundPlane;

        dragObject = new GameObject($"DragObject_{skillIndex}").transform;
        dragObject.SetParent(transform.root);
        dragObject.gameObject.SetActive(true);

        battleInstanceQuery = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
        isInited = true;

        viewBehaviour.UpdateSkillView((int)binarySkill.cooldown.value, 0, 1, BattleInstanceStatus.Waiting);

        SetStartPos();
    }

    public void InitSkillView(BinarySkill binary)
    {
        binarySkill = binary;
        Init();

        if (!string.IsNullOrEmpty(binary.drag_prefab))
        {
            var loaded = Addressables.InstantiateAsync("Drag/" + binary.drag_prefab + ".prefab", dragObject.transform);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
                {
  
                    var dragPrefab = async.Result;
                    var scaler = dragPrefab.GetComponent<DragRadiusScale>();
                    dragSkillParticle = dragPrefab.GetComponent<ParticleSystem>();
                    pss = dragPrefab.GetComponentsInChildren<ParticleSystem>().ToList();
                    mrs = dragPrefab.GetComponentsInChildren<MeshRenderer>().ToList();
                    smrs = dragPrefab.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
                    HideDragSkillPrefab(true);

                    if (scaler != null)
                    {
                        for (byte i = 0; i < binary.effects.Count; i++)
                        {
                            if (Effects.Instance.Get(binary.effects[i], out BinaryEffect binaryEffect))
                            {
                                float scale = CheckEffects(binaryEffect);
                                if (scale > 0)
                                {
                                    scaler.Scale(scale);
                                    break;
                                }
                            }
                        }
                    }
                    
                };
        }
        viewBehaviour.Init(binarySkill);
    }

    private void SetStartPos()
    {
        startScale          = rectTransform.localScale;
        startPosition       = rectTransform.localPosition;
        selectedPosition    = startPosition + new Vector2(0, selectedCardUpAt);
        startPivot          = rectTransform.pivot;
    }


    public void UpdateSkill(int time, byte myBridgesCount, float skillSpeed, BattleInstanceStatus status)
    {
        currentTimerValue = time;

        viewBehaviour.UpdateSkillView(time, myBridgesCount, skillSpeed, status);
    }

    private void Update()
    {
        if (!isInited)
            return;

        if (IsBlockedByTutorial)
            return;

        if (battleInstanceQuery.IsEmptyIgnoreFilter)
            return;

        var _battle = battleInstanceQuery.GetSingleton<BattleInstance>();

        isSandbox = _battle.isSandbox;
        //Если бой закончился - сбрасываем драг карты
        if (!BattleInstanceInterface.instance.IsGameReloaded && _battle.status != BattleInstanceStatus.Playing && _battle.status != BattleInstanceStatus.Pause ||
             BattleInstanceInterface.instance.IsGameReloaded && _battle.status > BattleInstanceStatus.Pause)
        {
            if (isDrag)
                StopDrag(new Vector2(50, 50));//Мы прост осбрасываем таскание карты. И куда мы сбросим юнит - не важно

            return;
        }

        if (!Input.touchSupported)
        {
            #region PC input proccessing
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
                        if (TryUseSkill(Input.mousePosition))
                        {
                            if (isSandbox)
                                Deselect();
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
                    if (TryUseSkill(nonCardTouch.position))
                    {
                        currentTouchId = int.MaxValue;
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

    private bool TryUseSkill(Vector2 pointerPosition)
    {
        if (!isSandbox && currentTimerValue > 0)
        {
            PopupAlertBehaviour.ShowBattlePopupAlert(pointerPosition, Locales.Get("locale:1285"));
            PlayClip(cantDragSkill);
            HideDragSkillPrefab();
            Deselect();
            return false;
        }

        UseSkillPrepare(pointerPosition);
        UseSkill(dragObject.position);
        Deselect(binarySkill.duration / 1000);

        if (isSandbox)
            InitDragObject();

        return true;
    }

    private void UseSkillPrepare(Vector2 pointerPosition)
    {
        SetDragObjectPosition(pointerPosition);

        if (CustomSpawnPosition != Vector2.zero)
        {
            dragObject.position = new Vector3(CustomSpawnPosition.x, 0.1f, CustomSpawnPosition.y);
        }
        /*
        while (dragObject.childCount > 0)
        {
            dragObject.GetChild(0).SetParent(ObjectPooler.instance.Minions.transform);
        }
        
        var _visualization = ClientWorld.Instance.GetOrCreateSystem<MinionGameObjectInitializationSystem>();
        for (int i = 0; i < spawnList.Length; i++)
        {
            var mib = spawnObjectsList[i].GetComponent<MinionInitBehaviour>();
            mib.SetupWaitPrefab();
            _visualization.Spawned(spawnList[i], spawnObjectsList[i]);
            ObjectPooler.instance.MinionBack(spawnObjectsList[i]);

            mib.MakeGray();
        }*/
    }

    private void UseSkill(Vector3 position)
    {
        var pos = new float2(position.x, position.z);

        if (CustomSpawnPosition != Vector2.zero)
        {
            pos = CustomSpawnPosition;
            CustomSpawnPosition = Vector2.zero;
        }

        ClientWorld.Instance.ActionPlaySkill(
            (byte)skillIndex,
            pos
        );

        HideDragSkillPrefab();

        TutorialMessageBehaviour.MakeTapEvent();
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

        rectTransform.localScale = startScale * scale;        

        //Если карта заскейлилась в ноль
        if (scale == 0)
        {
            //Показываем Драг объект
            ShowDragSkillPrefab();
            SetDragObjectPosition(pointerPosition);
            isPointerOnField = true;
        }
        else
        {
            //Прячем Драг объект
            isPointerOnField = false;
            HideDragSkillPrefab();
        }
    }

    private void StartDrag(Vector2 localClickPosition)
    {
        isDrag = true;
        //Устанавливаем пивот в точку где игрок взял карту
        var newPivot = (localClickPosition + rectTransform.pivot * rectTransform.rect.size) / rectTransform.rect.size;

        SetPivot(rectTransform, newPivot);
        startDragPosition = rectTransform.localPosition;

        rectTransform.DOKill();
    }

    private void Deselect(float delay = 0)
    {
        isSelected = false;
        PutOutHighlightZone();
        BackToStartPos(delay);
    }

    private void Select(Vector2 pointerPosition)
    {
        touchStartPosition = pointerPosition;

        if (isSelected)
            return;

        isSelected = true;
        HighlightZone();
        AnimateSelected();
    }

    private void StopDrag(Vector2 pointerPosition)
    {
        isDrag = false;

        SetPivot(rectTransform, startPivot);

        if (isPointerOnField)
        {
            TryUseSkill(pointerPosition);
        }
        else
        {
            Deselect();
        }

        isPointerOnField = false;
    }

    private void AnimateSelected()
    {
        if (isSandbox)
            return;

        var timeToMove = 0.2f;
        var moveToY = selectedPosition.y;

        rectTransform.DOLocalMoveY(moveToY, timeToMove).SetEase(Ease.InQuad);
    }

    private void BackToStartPos(float delay)
    {
        rectTransform.localPosition = startPosition;
        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(delay);
        rectTransform.DOLocalMove(startPosition, 0.2f).SetEase(Ease.OutQuad);
        sequence.Append(rectTransform.DOScale(startScale * 1.05f, 0.2f)).SetEase(Ease.InQuad);
        sequence.Append(rectTransform.DOScale(startScale, 0.1f)).SetEase(Ease.OutQuad);
    }

    private void SetDragObjectPosition(Vector2 pointerPosition)
    {
        var ray = BattleInstanceInterface.instance.MainCamera.ScreenPointToRay(pointerPosition);

        groundPlane.Raycast(ray, out float distance);

        var pos = ray.GetPoint(distance);
        pos.y = 0.1f;

        if (CustomSpawnPosition != Vector2.zero)
        {
            if (pos.z > CustomSpawnPosition.y)
                pos.z = CustomSpawnPosition.y;

            if (pos.x < CustomSpawnPosition.x)
                pos.x = CustomSpawnPosition.x;
        }

        dragObject.position = pos;
        //ArenaZonesBehaviour.instance.SetRegularPosition(pos);
    }

    private bool IsClickOnMe(out Vector2 localClickPosition)
    {
        var ray = BattleInstanceInterface.instance.UICamera.ScreenPointToRay(Input.mousePosition);

        localClickPosition = Vector2.zero;
        var hits = Physics.RaycastAll(ray, 30f);

        //if (hits.Length != 1)
        //    return false;

        //if (hits[0].transform.parent.GetComponent<BattleSkillDragBehaviour>() == null)
        //    return false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, BattleInstanceInterface.instance.UICamera, out localClickPosition);
        return rectTransform.rect.Contains(localClickPosition);
    }

    private bool IsTouchOnMe(Touch touch, out Vector2 localClickPosition)
    {
        var ray = BattleInstanceInterface.instance.UICamera.ScreenPointToRay(touch.position);

        localClickPosition = Vector2.zero;
        var hits = Physics.RaycastAll(ray, 30f);

        //if (hits.Length != 1)
        //    return false;

        //if (hits[0].transform.parent.GetComponent<BattleSkillDragBehaviour>() == null)
        //    return false;

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
        dragObject.gameObject.SetActive(false);
        //SetupSquad(binarySkill.entities);
    }
    /*
    private void SetupSquad(List<ushort> DBEntities)
    {
        uint count = (uint)DBEntities.Count;
        spawnList = new ushort[count];
        spawnObjectsList = new GameObject[count];

        if (Entities.Instance.Get(DBEntities[0], out BinaryEntity entity))
        {
            for (int i = 0; i < count; i++)
            {
                ushort eDBID = DBEntities[i];
                spawnList[i] = eDBID;
                var j = i;
                CreateEntityPrefab(eDBID, (obj) => {
                    spawnObjectsList[j] = obj;
                    obj.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                    float2 inSquadPos = (GetSquareSquadUnitPosition(count, (uint)i) * entity.collider * 2) * (-1);
                    obj.transform.localPosition = new Vector3((float)inSquadPos.x, 0, (float)inSquadPos.y);
                    obj.SetActive(true);
                    obj.GetComponent<MinionInitBehaviour>().DoMinionVisible();
                });
            }
        }
    }*/

    /*
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

            ObjectPooler.instance.GetMinion(entity, (obj) => {
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
                obj.GetComponent<StartEffect>().Flying = entity.type == MinionLayerType.Fly;
                callback(obj);
            });
        }
    }*/

    /// <summary>
    /// Устанавливает пивот и корректирует положение, что бы объект не двигался.
    /// </summary>
    public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {/*
        if (rectTransform == null) return;

        var scale = rectTransform.localScale;
        var size = rectTransform.rect.size;
        var deltaPivot = rectTransform.pivot - pivot;

        var deltaPosition = new Vector3(
            deltaPivot.x * size.x * scale.x,
            deltaPivot.y * size.y * scale.y);

        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;*/
    }

    private void HideDragSkillPrefab(bool forceHide = false)
    {
        if (dragSkillParticle)
        {
            var main = dragSkillParticle.main;
            main.loop = false;
            HideAllParticlesAndMeshes(false);
            if (forceHide)
                dragSkillParticle.gameObject.SetActive(false);
        }
    }

    private void ShowDragSkillPrefab()
    {
        if (dragSkillParticle)
        {
            if (!dragSkillParticle.gameObject.activeSelf)
            {
                dragSkillParticle.gameObject.SetActive(true);
                HideAllParticlesAndMeshes(true);
                if (dragSkillParticle.particleCount == 0)
                {
                    dragSkillParticle.Play();
                }
                var main = dragSkillParticle.main;
                main.loop = true;
            }
        }
    }

    private float CheckEffects(BinaryEffect binaryEffect)
    {
        for (byte j = 0; j < binaryEffect.components.Count; j++)
        {
            if (Components.Instance.GetLink(binaryEffect.components[j], out string name))
            {
                if (name == typeof(Legacy.Database.EffectRadius).Name)
                {
                    var effects = Components.Instance.Get<Legacy.Database.EffectRadius>();
                    if (effects.TryGetValue(binaryEffect.index, out Legacy.Database.EffectRadius radiusData))
                    {
                        return radiusData.radius;
                    }
                }
            }
        }
        return 0f;
    }

    // Мы показываем большое сообщение и хотим прекратить перетаскивание карты и прекратить задрежанное выставление юнита
    public void TutorialStopUseSkill()
    {
        if (delayedUseCardCoroutine != null)
        {
            StopCoroutine(delayedUseCardCoroutine);
            delayedUseCardCoroutine = null;
        }

        if (isDrag)
        {
            isDrag = false;
            currentTouchId = int.MaxValue;
            HideDragSkillPrefab();
            Deselect();
            SetPivot(rectTransform, startPivot);
        }
    }

    private void HideAllParticlesAndMeshes(bool flag)
    {

        foreach (var ps in pss)
        {
            var main = ps.main;
            main.loop = flag;
        }
        foreach (var mr in mrs)
        {
            mr.enabled = flag;
            mr.gameObject.SetActive(flag);
        }
        foreach (var smr in smrs)
        {
            smr.enabled = flag;
            dragSkillParticle.gameObject.SetActive(flag);
        }
    }

    protected void HighlightZone()
    {
        zoneHighlight = ClientWorld.Instance.EntityManager.CreateEntity();
        ClientWorld.Instance.EntityManager.AddComponentData(zoneHighlight, new ArenaZoneHighlight { allZone = true });
    }

    protected void PutOutHighlightZone()
    {
        if (zoneHighlight != Entity.Null)
            ClientWorld.Instance.EntityManager.DestroyEntity(zoneHighlight);
    }
    private void PlayClip(AudioClip clip)
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }
}
