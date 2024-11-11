using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialCardMagnetState : IState
{
	private TutorialCard tutorialCard;
	public TutorialCardMagnetState(TutorialCard tutorialCard)
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
		tutorialCard.animator.SetTrigger(TutorialCardState.Magnet.ToString());
		ClearEvents();
		tutorialCard.continueDragEvent.AddListener(onDrag);
		tutorialCard.endDragEvent.AddListener(onDragEnd);
		tutorialCard.untouchEvent.AddListener(OnUntouch);
		var p = tutorialCard.planeHit.point;
		var ps = tutorialCard.magnetTarget.transform.position;
		ps.y = 0.2f;
		tutorialCard.dragPrefab.transform.position = ps;
	}

	private void ClearEvents()
	{
		tutorialCard.continueDragEvent.RemoveListener(onDrag);
		tutorialCard.endDragEvent.RemoveListener(onDragEnd);
		tutorialCard.untouchEvent.RemoveListener(OnUntouch);
	}

	private void OnUntouch(PointerEventData eventData)
	{
		ClearEvents();
		tutorialCard.DoMinionPlace();
	}

	private void onDrag(PointerEventData eventData)
	{
		var p = tutorialCard.planeHit.point;
		if (Vector3.Distance(p, tutorialCard.magnetTarget.transform.position) > 1.5f)
		{
			ClearEvents();
			tutorialCard.SetState(TutorialCardState.Drag);
			return;
		}
	}

	private void onDragEnd(PointerEventData eventData)
	{
		ClearEvents();
		tutorialCard.DoMinionPlace();
	}

	public void OnLeave()
	{
		ClearEvents();
	}

	public void OnUpdate()
	{
	}
}
