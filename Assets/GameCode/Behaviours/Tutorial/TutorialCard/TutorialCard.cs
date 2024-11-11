using Legacy.Client;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TutorialCardState : int
{
    Default,
    Touched,
    Small,
    Drag,
    Magnet
}

public class TutorialCardChangeStateEvent : UnityEvent<TutorialCardState> { };
public class TutorialCardDragEvent : UnityEvent<PointerEventData> { };

public class TutorialCard : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    private TutorialCardState _state = TutorialCardState.Default;
    public bool placeAnywhere;
    public BoxCollider plane;
    public GameObject dragPrefab;
    public GameObject pointTarget;
    public GameObject magnetTarget;

    public Image icon;
    public TextMeshProUGUI manaText;
    public RectTransform ViewContainer;
    
    public TutorialCardDragEvent touchEvent { get; private set; }
    public TutorialCardDragEvent beginDragEvent { get; private set; }
    public TutorialCardDragEvent continueDragEvent { get; private set; }
    public TutorialCardDragEvent endDragEvent { get; private set; }
    public TutorialCardDragEvent untouchEvent { get; private set; }
    public UnityEvent taskDoneEvent { get; private set; }

    private TutorialCardChangeStateEvent changeStateEvent = new TutorialCardChangeStateEvent();
    private StateMachine<TutorialCardState> _state_machine = new StateMachine<TutorialCardState>();
    public Animator animator { get; private set; }

    public bool simple;
    public TutorialCard()
    {
        touchEvent = new TutorialCardDragEvent();
        beginDragEvent = new TutorialCardDragEvent();
        continueDragEvent = new TutorialCardDragEvent();
        endDragEvent = new TutorialCardDragEvent();
        taskDoneEvent = new UnityEvent();
        untouchEvent = new TutorialCardDragEvent();
    }

    public void CopyCardView(GameObject card)
    {
        var cvb = card.GetComponentsInChildren<CardViewBehaviour>()[0];
        icon.sprite = cvb.IconContent.sprite;
        var bcb = card.GetComponentsInChildren<BattleCardBehaviour>()[0];
        manaText.text = bcb.DBCardData.manaCost.ToString();
    }

    public void CopySkillView(GameObject card)
    {
        var svb = card.GetComponentsInChildren<SkillViewBehaviour>()[0];
        icon.sprite = svb.IconContent.sprite;
    }

    public Vector2 clampPosX = new Vector2(-12f, -1.8f);
    public Vector2 clampPosZ = new Vector2(-6f, 7.3f);

    void Start()
    {
        animator = GetComponent<Animator>();
        if(simple)
        {
            untouchEvent.AddListener(OnTapUntouch);
            return;
        }
        _state_machine.Add(TutorialCardState.Default, new TutorialCardDefaultState(this));
        _state_machine.Add(TutorialCardState.Touched, new TutorialCardTouchState(this));
        _state_machine.Add(TutorialCardState.Small, new TutorialCardSmallState(this));
        _state_machine.Add(TutorialCardState.Drag, new TutorialCardPlaneDragState(this));
        //_state_machine.Add(TutorialCardState.Magnet, new TutorialCardMagnetState(this));
        _state = TutorialCardState.Default;
        _state_machine.SwitchTo(_state);
    }

    private void OnTapUntouch(PointerEventData eventData)
    {
        taskDoneEvent.Invoke();
        ClearAll();
    }

    public void SetState(TutorialCardState newCardState)
    {
        if (_state == newCardState) return;
        Debug.Log("tutorial card state changed from <color=blue>" + _state + "</color> to <color=green>" + newCardState + "</color>");
        this._state = newCardState;
        OnChangeState();
    }

    private void OnChangeState()
    {
        if (_state_machine != null)
        {
            if (!_state_machine.StateInited) return;
            if (_state_machine.CurrentState == _state)
            {
                Debug.Log("Same State is set");
            }
            _state_machine.SwitchTo(_state);
            OnAnyStateEnter();
            changeStateEvent.Invoke(_state);
        }
    }

    private void OnAnyStateEnter()
    {
        _state_machine.ThreadEnter();
    }

    private void OnAnyStateUpdate()
    {
        _state_machine.ThreadUpdate();
    }

    private void OnAnyStateLeave()
    {
        _state_machine.ThreadLeave();
    }

    void Update()
    {
        if (simple) return;
        DoCast();
        _state_machine.Update();
        if(_state == TutorialCardState.Default || _state == TutorialCardState.Touched)
        {
            LerpToDefault();
        }
    }

    private void LerpToDefault()
    {
        if (ViewContainer.transform.localScale == Vector3.one && ViewContainer.localPosition == Vector3.zero)
            return;
        ViewContainer.transform.localScale = Vector3.Lerp(ViewContainer.transform.localScale, Vector3.one, 0.5f);
        if (Vector3.Distance(ViewContainer.transform.localScale, Vector3.one) < 0.05f)
        {
            ViewContainer.transform.localScale = Vector3.one;
        }
        //ViewContainer.anchoredPosition = Vector3.Lerp(ViewContainer.anchoredPosition, Vector3.zero, 0.5f);
        //if (Vector3.Distance(ViewContainer.anchoredPosition, Vector3.zero) < 0.5f)
        //{
        //    ViewContainer.anchoredPosition = Vector3.zero;
        //}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endDragEvent.Invoke(eventData);
    }

    public Vector2 startDragPos { get;private set; }
    public TutorialCardChangeStateEvent ChangeStateEvent { get => changeStateEvent; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startDragPos = eventData.position;
        beginDragEvent.Invoke(eventData);
    }

    public float sceneDragBorder = 4f;
    public float sceneOverBorderDrag = 5f;
    public RaycastHit planeHit;
    public void DoCast()
    {
        Ray _input_ray = Camera.main.ScreenPointToRay((Input.touchCount > 0) ? (new Vector3(Input.touches[0].position.x, Input.touches[0].position.y)) : Input.mousePosition);
        RaycastHit[] _hits = Physics.RaycastAll(_input_ray);

        foreach(var h in _hits)
        {
            if(h.collider == plane)
            {
                planeHit = h;
                return;
            }
        }
    }

    public void DoMinionPlace()
    {
        taskDoneEvent.Invoke();
        ClearAll();
        //SetState(TutorialCardState.Default);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log(eventData.position);
        continueDragEvent.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchEvent.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        untouchEvent.Invoke(eventData);
    }

    public void ClearAll()
    {
        continueDragEvent.RemoveAllListeners();
        touchEvent.RemoveAllListeners();
        endDragEvent.RemoveAllListeners();
        beginDragEvent.RemoveAllListeners();
        taskDoneEvent.RemoveAllListeners();
        untouchEvent.RemoveAllListeners();
        _state_machine.SwitchEvent.RemoveAllListeners();
        _state_machine.LeaveEvent.RemoveAllListeners();
    }
}
