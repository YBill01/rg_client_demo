using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Entities;
using Legacy.Database;
using Legacy.Client;
using System;
using DG.Tweening;

public class BattleCardManaBehaviour : MonoBehaviour
{
	[SerializeField]
	private Image Fader;

	[SerializeField]
	private GameObject ManaIcon;

	private int manaCost;
	private bool shaking;
	private bool inited;

	private RectTransform rect;
	private Sequence shaker;
	private BattleCardDragBehaviour dragBehaviour;
	private BattleCardViewBehaviour viewBehaviour;
	private Vector3 startCardScale;

    private void OnEnable()
    {
		rect = GetComponent<RectTransform>();
		startCardScale = rect.localScale;
	}
    public void Init(int manaCost)
	{
		inited = true;
		this.manaCost = manaCost;

		dragBehaviour = GetComponent<BattleCardDragBehaviour>();
		viewBehaviour = GetComponent<BattleCardViewBehaviour>();

		if (!ClientWorld.Instance.isSandbox)
			UpdateState();
	}

	void Update()
	{
		if (!inited)
			return;

		UpdateState();
	}

	private void UpdateState()
	{
		float fill = CalculateFill();

		if (Fader.fillAmount != 0 && fill == 0)
		{
			var sequence = DOTween.Sequence();

			sequence.Append(rect.DOScale(startCardScale * 1.025f, 0.1f));
			sequence.Append(rect.DOScale(startCardScale, 0.1f));
		}

		if (!dragBehaviour.IsGoLikeNewCard)
			viewBehaviour.SetGray(fill != 0 || dragBehaviour.IsBlockedByTutorial);


		Fader.fillAmount = fill;

		if (ManaUpdateSystem.PlayerMana == 10)
		{
			if (!shaking)
			{
				//shaker = rect.DOShakeRotation(1, 1.5f).SetLoops(-1);
				shaker = DOTween.Sequence();

				var period = 0.09f + ((float)dragBehaviour.IndexInHand) * 0.005f;

				shaker.Append(rect.DORotate(new Vector3(0, 0, 0.5f), period));
				shaker.Append(rect.DORotate(new Vector3(0, 0, -0.5f), period));
				shaker.SetLoops(-1);
				shaker.onKill = () => shaking = false;
				shaking = true;
			}
		}
		else
		{
			shaking = false;
			shaker.Kill();
		}
	}

	private float CalculateFill()
	{
		return 1 - Mathf.Clamp01(((float)ManaUpdateSystem.PlayerMana) / manaCost);
	}

	public void ShowManaCostAndFader(bool show)
	{
		ManaIcon.SetActive(show);
		Fader.gameObject.SetActive(show);
	}
}
