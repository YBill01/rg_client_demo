using Legacy.Client;
using Legacy.Database;
using System;
using Unity.Entities;
using UnityEngine;
using System.Collections;
using System;

public class HandBehaviour : MonoBehaviour
{
    public BattleCardDragBehaviour[] handObjects;
    public BattleCardViewBehaviour nextCard;

    const float delayToPrepareNewCard = 0.15f;

    [HideInInspector] public bool CanContinue;

    private bool _setHandPrepearStarted = false;
    private Coroutine _cardsDeliverRout;

    public void UpdateHand(BattlePlayerHand hand, bool isPrepare = false)
    {
        if (isPrepare && _setHandPrepearStarted || !CanContinue)
            return;

        if (isPrepare || !ClientWorld.Instance.GetOrCreateSystem<StateMachineSystem>().IsConnectedTooExistedBattle && !_setHandPrepearStarted)
            SetHandAtStart(hand);
        else if (_cardsDeliverRout == null)
            SetHandInBattle(hand);
    }

	private void SetHandInBattle(BattlePlayerHand hand)
	{
        for (int i = 0; i < BattlePlayerHand.next; ++i)
        {
            var cardBehaviour = handObjects[i];
            if (!cardBehaviour.IsHidden)
                continue;

            if (cardBehaviour.BinaryCard.index == hand[i].index)
                continue;

            if (!cardBehaviour.IsInited)
            {
                cardBehaviour.Init();
            }

            cardBehaviour.UpdateCardData(hand[i].index);
            cardBehaviour.Unhide();

            Cards.Instance.Get(hand.Next.index, out BinaryCard binaryCard);
            nextCard.Init(binaryCard);
            return;
        }
    }

	private void SetHandAtStart(BattlePlayerHand hand)
	{
        _setHandPrepearStarted = true;
        _cardsDeliverRout = StartCoroutine(SetHandAtStartRoutine(hand));
    }

    private IEnumerator SetHandAtStartRoutine(BattlePlayerHand hand)
    {
        var delay = new WaitForSeconds(delayToPrepareNewCard);

        Cards.Instance.Get(hand[0].index, out BinaryCard binaryCard);
        nextCard.Init(binaryCard);
        BattleInstanceInterface.instance.ShowNextCard();

        yield return delay;
        yield return delay;

        for (int i = 0; i < BattlePlayerHand.next; i++)
        {
            var cardBehaviour = handObjects[i];
            if (!cardBehaviour.IsHidden)
                continue;

            cardBehaviour.Init();
            cardBehaviour.UpdateCardData(hand[i].index);
            cardBehaviour.Unhide();

            Cards.Instance.Get(hand[i + 1].index, out BinaryCard binary);
            nextCard.Init(binary);

            yield return delay;
        }
        if(_cardsDeliverRout!= null)
        {
            StopCoroutine(_cardsDeliverRout);
            _cardsDeliverRout = null;
        }
    }
}
