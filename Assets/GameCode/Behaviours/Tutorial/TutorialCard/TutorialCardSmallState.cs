using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialCardSmallState : IState
{

	private TutorialCard tutorialCard;
	public TutorialCardSmallState(TutorialCard tutorialCard)
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
		tutorialCard.animator.SetTrigger(TutorialCardState.Small.ToString());
		tutorialCard.continueDragEvent.AddListener(onDrag);
		tutorialCard.endDragEvent.AddListener(onDragEnd);
	}

	private void onDrag(PointerEventData eventData)
	{
		var lp = Mathf.Abs(tutorialCard.planeHit.point.z);
		if (lp > tutorialCard.sceneOverBorderDrag)
		{
			tutorialCard.SetState(TutorialCardState.Touched);
			tutorialCard.icon.transform.localScale = Vector3.one;
			return;
		}
		if (lp < tutorialCard.sceneOverBorderDrag &&
			lp > tutorialCard.sceneDragBorder)
		{
			var diff = (tutorialCard.sceneOverBorderDrag - tutorialCard.sceneDragBorder);
			var rel = (lp - tutorialCard.sceneDragBorder) / diff;
			tutorialCard.ViewContainer.localScale = Vector3.one * rel;
			//var tlp = tutorialCard.ViewContainer.anchoredPosition;
			//tlp.y = 250 * (1 - rel);
			//tutorialCard.ViewContainer.anchoredPosition = tlp;
			return;
		}
		tutorialCard.SetState(TutorialCardState.Drag);
	}

	private void onDragEnd(PointerEventData eventData)
	{
		tutorialCard.continueDragEvent.RemoveListener(onDrag);
		tutorialCard.endDragEvent.RemoveListener(onDragEnd);

		tutorialCard.SetState(TutorialCardState.Default);
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
