using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialCardPlaneDragState : IState
{
	private TutorialCard tutorialCard;
	public TutorialCardPlaneDragState(TutorialCard tutorialCard)
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
		tutorialCard.animator.SetTrigger(TutorialCardState.Drag.ToString());
		ClearEvents();
		tutorialCard.continueDragEvent.AddListener(onDrag);
		tutorialCard.endDragEvent.AddListener(onDragEnd);
		tutorialCard.untouchEvent.AddListener(OnUntouch);
		tutorialCard.dragPrefab.SetActive(true);
		tutorialCard.icon.enabled = false;
		UpdateDragUnit();
	}

	private void OnUntouch(PointerEventData eventData)
	{
		ClearEvents();

		if(!tutorialCard.placeAnywhere)
		{
			var ps = tutorialCard.magnetTarget.transform.position;
			ps.y = 0.2f;
			tutorialCard.dragPrefab.transform.position = ps;
		}
		tutorialCard.DoMinionPlace();
		//tutorialCard.SetState(TutorialCardState.Default);
		//tutorialCard.dragPrefab.SetActive(false);
		//tutorialCard.icon.enabled = true;
	}

	private void ClearEvents()
	{
		tutorialCard.continueDragEvent.RemoveListener(onDrag);
		tutorialCard.endDragEvent.RemoveListener(onDragEnd);
		tutorialCard.untouchEvent.RemoveListener(OnUntouch);
	}

	private void onDrag(PointerEventData eventData)
	{
		UpdateDragUnit();
	}

	private void UpdateDragUnit()
	{
		var p = tutorialCard.planeHit.point;

		p.x = Mathf.Clamp(p.x, tutorialCard.clampPosX.x, tutorialCard.clampPosX.y);
		p.z = Mathf.Clamp(p.z, tutorialCard.clampPosZ.x, tutorialCard.clampPosZ.y);
		p.y = 0.2f;
		tutorialCard.dragPrefab.transform.localPosition = p;
		/*if (Vector3.Distance(tutorialCard.dragPrefab.transform.localPosition, tutorialCard.target.transform.localPosition) < tutorialCard.target.transform.localScale.x)
		if(!tutorialCard.placeAnywhere)
		{
			if (Vector3.Distance(tutorialCard.dragPrefab.transform.position, tutorialCard.magnetTarget.transform.position) < 1.5f)
			{
				ClearEvents();
				//tutorialCard.SetState(TutorialCardState.Magnet);
				return;
			}
		}*/
		tutorialCard.ViewContainer.localScale = Vector3.Lerp(tutorialCard.ViewContainer.localScale, Vector3.zero, 0.5f);
		if (p.z > 0)
		{
			return;
		}
		var lp = Mathf.Abs(p.z);
		if (lp > tutorialCard.sceneDragBorder)
		{
			ClearEvents();
			tutorialCard.SetState(TutorialCardState.Small);
			tutorialCard.dragPrefab.SetActive(false);
			tutorialCard.icon.enabled = true;
		}
	}

	private void onDragEnd(PointerEventData eventData)
	{
		ClearEvents();
		tutorialCard.SetState(TutorialCardState.Default);
		tutorialCard.dragPrefab.SetActive(false);
		tutorialCard.icon.enabled = true;
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
