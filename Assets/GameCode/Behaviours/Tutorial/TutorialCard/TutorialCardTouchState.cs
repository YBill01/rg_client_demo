using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialCardTouchState : IState
{
	private TutorialCard tutorialCard;
	public TutorialCardTouchState(TutorialCard tutorialCard)
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
		tutorialCard.animator.SetTrigger(TutorialCardState.Touched.ToString());
		tutorialCard.continueDragEvent.RemoveListener(onDrag);
		tutorialCard.endDragEvent.RemoveListener(onDragEnd);
		tutorialCard.untouchEvent.RemoveListener(onUntouch);
		tutorialCard.continueDragEvent.AddListener(onDrag);
		tutorialCard.endDragEvent.AddListener(onDragEnd);
		tutorialCard.untouchEvent.AddListener(onUntouch);
	}

	private void onUntouch(PointerEventData eventData)
	{
		GoToDefault();
	}
	private void onDragEnd(PointerEventData eventData)
	{
		GoToDefault();
	}

	private void GoToDefault()
	{
		tutorialCard.continueDragEvent.RemoveListener(onDrag);
		tutorialCard.endDragEvent.RemoveListener(onDragEnd);
		tutorialCard.untouchEvent.RemoveListener(onDragEnd);

		tutorialCard.SetState(TutorialCardState.Default);
	}

	private void onDrag(PointerEventData eventData)
	{
		if(tutorialCard.startDragPos.y < eventData.position.y)
		{
			tutorialCard.SetState(TutorialCardState.Small);
		}
	}

	public void OnLeave()
	{
		tutorialCard.continueDragEvent.RemoveListener(onDrag);
		tutorialCard.endDragEvent.RemoveListener(onDragEnd);
	}

	public void OnUpdate()
	{
	}
}
