using Legacy.Client;
using Legacy.Database;
using Legacy.Game;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BattleCardBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ushort hand_index;
    public ushort db_index;

    [SerializeField]
    private CardTextDataBehaviour text;
    [SerializeField]
    private CardViewBehaviour view;

    private bool inited;

    private bool isActive = true;

    public bool Selected = false;
	public static bool isAnySelected;

    public bool isNext;

    public BinaryCard DBCardData;
    public HandBehaviour hand;

    public class BattleCardDoubleClick : UnityEvent<ushort> { };
    public BattleCardDoubleClick onDoubleClick = new BattleCardDoubleClick();

    public CardViewBehaviour View { get => view; }

    public void Select(Touch touch)
    {
        GetComponent<SelectedCardBehaviour>().SetTouch(touch);
        //isAnySelected = true;
        if (!Selected)
        {
            view.Glow(true);
            transform.SetAsLastSibling();
            ManaUpdateSystem.ManaToUse += DBCardData.manaCost;
            GetComponent<SelectedCardBehaviour>().ResetAddParams();
            GetComponent<SelectedCardBehaviour>().SetStartPosition();
            Selected = true;
            isAnySelected = true;
        }
        else
        {
            if (onDoubleClick != null)
                onDoubleClick.Invoke(db_index);
        }
    }

    internal void Active(bool NonActive)
    {
        if (isActive != NonActive || isNext) return;
        else isActive = !NonActive;
        if (!NonActive && !Selected)
        {
            //TODO: ManaComes
        }
        view.MakeGray(NonActive);
    }

    public void Deselect(bool isUnitSpawn = true)
    {
        transform.SetAsFirstSibling();
        view.SetRaycastTarget(true);        
        view.Glow(false);
        Selected = false;
		isAnySelected = false;

        if (!isUnitSpawn)
		    ManaUpdateSystem.ManaToUse -= DBCardData.manaCost;
    }

    public void UpdateCardData(ushort db_id)
	{
        inited = false;
        db_index = db_id;

        if (Cards.Instance.Get(db_index, out BinaryCard card))
		{
			DBCardData = card;
			InitCard();
		}
	}

    public void InitCard()
    {
        if (inited) return;
        view.Init(DBCardData);
        isNext = hand_index == BattlePlayerHand.next;
        view.SetRaycastTarget(!isNext);
        if (!isNext)
            UpdateManaViewOn();

        inited = true;
       
    }    

    internal void UpdateManaViewOn(byte add = 0)
    {
        byte cost = (byte)(DBCardData.manaCost + add);
        text.SetManaCost(cost);
    }

	public void OnEndDrag(PointerEventData eventData)
	{
		isAnySelected = false;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		isAnySelected = true;
	}
}
