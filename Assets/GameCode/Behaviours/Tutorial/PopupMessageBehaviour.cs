using Legacy.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupMessageBehaviour : MonoBehaviour
{
	[SerializeField]
	private RectTransform TopArrow;

	[SerializeField]
	private RectTransform BottomArrow;

	[SerializeField]
	private RectTransform RightDownArrow;

	[SerializeField]
	private RectTransform RightArrow;

	[SerializeField]
	private TextMeshProUGUI Text;

	private RectTransform rect;

	private RectTransform target;
	private Func<Vector3> positionShiftMethod;
	private RectTransform currentArrow;

	[SerializeField]private AnimationAutoDestroy animationAutoDestroy;

	private float timeToHide;

	public void ShowTextAtBottomFrom(string text, RectTransform from, float delay = 0)
	{
		gameObject.SetActive(true);
		TopArrow.gameObject.SetActive(true);
		
		Text.text = text;
		target = from;
		currentArrow = TopArrow;
		positionShiftMethod = AtBottomShift;

		SetPosition();
	}

	public void ShowTextAtTopFrom(string text, RectTransform from, float delay = 0)
	{
		gameObject.SetActive(true);
		BottomArrow.gameObject.SetActive(true);
		
		Text.text = text;
		target = from;
		currentArrow = BottomArrow;
		positionShiftMethod = AtTopShift;

		SetPosition();
	}

	public void ShowTextAtLeftTopFrom(string text, RectTransform from, float delay = 0)
	{
		gameObject.SetActive(true);
		RightDownArrow.gameObject.SetActive(true);

		Text.text = text;
		target = from;
		currentArrow = RightDownArrow;
		positionShiftMethod = AtLeftTopShift;

		SetPosition();
	}

	public void ShowTextAtLeftFrom(string text, RectTransform from, float delay = 0)
	{
		Text.text = text;
		target = from;
		currentArrow = RightArrow;
		positionShiftMethod = AtLeftShift;

		RightArrow.gameObject.SetActive(true);
		gameObject.SetActive(true);
		SetPosition();
	}

	private void SceduleTimer(float delay)
	{
		if (delay == 0)
			timeToHide = 0;

		timeToHide = Time.time + delay;
	}

	private void SetPosition()
	{
		var arrowDelta = currentArrow.position - rect.position;
		var newPos = target.position;
		if(positionShiftMethod != null)
			newPos +=	positionShiftMethod();
		newPos -= arrowDelta;
		rect.position = newPos;

	}

	public void Hide()
	{
		gameObject.SetActive(false);
		TopArrow.gameObject.SetActive(false);
		BottomArrow.gameObject.SetActive(false);
		RightDownArrow.gameObject.SetActive(false);
	}

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		SetPosition();

		if (timeToHide != 0 && timeToHide <= Time.time)
		{
			timeToHide = 0;
			Hide();
		}

		if (!target.gameObject.activeInHierarchy || !target.gameObject.activeSelf)
			Hide();

	}

	private Vector3 AtTopShift()
	{
		var k = WindowManager.Instance.MainCanvas.transform.localScale.y;
		return new Vector3(0, target.rect.height / 2f * k, 0);
	}

	private Vector3 AtBottomShift()
	{
		return -AtTopShift();
	}

	private Vector3 AtLeftTopShift()
	{
		var k = WindowManager.Instance.MainCanvas.transform.localScale.y;
		return new Vector3(-target.rect.width / 2f * k, target.rect.height / 2f * k, 0);
	}

	private Vector3 AtLeftShift()
	{
		var k = WindowManager.Instance.MainCanvas.transform.localScale.y;
		return new Vector3(-target.rect.width / 2f * k, 0, 0);
	}
}
