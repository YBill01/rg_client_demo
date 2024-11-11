using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialCardDefaultState : IState
{
	private TutorialCard tutorialCard;
	public TutorialCardDefaultState(TutorialCard tutorialCard)
	{
		this.tutorialCard = tutorialCard;
	}

	public void MonoThreadEnter()
	{
	}

	public void MonoThreadLeave()
	{
	}

	public void MonoThreadUpdate()
	{
	}

	public void OnEnter()
	{
		tutorialCard.animator.SetTrigger(TutorialCardState.Default.ToString());
		tutorialCard.touchEvent.AddListener(OnTouchStart);
	}

	private void OnTouchStart(PointerEventData eventData)
	{
		tutorialCard.touchEvent.RemoveListener(OnTouchStart);
		tutorialCard.SetState(TutorialCardState.Touched);
	}

	public void OnLeave()
	{
		tutorialCard.touchEvent.RemoveListener(OnTouchStart);
	}

	public void OnUpdate()
	{
	}
}
