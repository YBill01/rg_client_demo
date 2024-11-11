using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using Legacy.Client;

public class UnitTimerBehaviour : MonoBehaviour
{
	[SerializeField] private RectTransform arrow;
	[SerializeField] private Image back;
	[SerializeField] private Image fill;
	[SerializeField] private Image glow;

	private Vector3 targetPosition;
	private RectTransform rectTransform;
	private RectTransform canvas;
	private Camera mainCamera;

	private float startTime;
	private float duration;

	private bool finished;

	private float startScale = 1.3f;
	private float startScaleTime1 = 0.1f;
	private float startScaleTime2 = 0.2f;

	private float scaleTime1 = 0.1f;
	private float scaleTime2 = 0.1f;
	private float scaleTime3 = 0.3f;
	private float finishScale = 1.4f;
	private float glowTime1 = 0.05f;
	private float glowTime2 = 0.1f;
	private float glowTime3 = 0.3f;

	private Rect limits = new Rect()
	{
		xMin = 50,
		xMax = 100,
		yMin = 100,
		yMax = 50
	};

	public void InitTimer(Vector3 targetPosition, float duration, bool isEnemy, bool isFlying)
	{
		rectTransform = GetComponent<RectTransform>();
		mainCamera = Camera.main;
		canvas = BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>();

		if (isFlying)
			targetPosition.z += 1.2f; 

		this.targetPosition = targetPosition;

		startTime = Time.time;
		this.duration = duration;

		fill.fillAmount = 0;
		arrow.localEulerAngles = Vector3.zero;

		var fillColor = isEnemy ? VisualContent.EnemyColor : VisualContent.PlayerColor;
		fillColor.a = fill.color.a;
		fill.color = fillColor;
		var sequence = DOTween.Sequence();
		sequence.Append(transform.DOScale(Vector3.one * startScale, startScaleTime1));
		sequence.Append(transform.DOScale(Vector3.one, startScaleTime2));

		StartCoroutine(LateDestroyGameObject(duration));
		
	}

	private IEnumerator LateDestroyGameObject(float lasting)
    {
		var delay = new WaitForSeconds(lasting);
		yield return delay;
		GetComponent<AudioSource>().Play();
		arrow.gameObject.SetActive(false);
		back.enabled = false;
		fill.enabled = false;
		glow.enabled = false;
		yield return delay;
		Destroy(gameObject);
    }

	void Update()
	{
		SetPosition();

		if (finished)
			return;
			
		var timeDelta = Time.time - startTime;
		var fillAmount = timeDelta / duration*1.0f;

		fill.fillAmount = fillAmount;
		arrow.localEulerAngles = new Vector3(0, 0, -360f * fillAmount);

		if (fillAmount == 1)
			finished = true;
		else
			return;

		var sequence = DOTween.Sequence();
		sequence.Append(transform.DOScale(Vector3.one * finishScale, scaleTime1));
		sequence.Append(transform.DOScale(Vector3.one, scaleTime2));
		sequence.Append(transform.DOScale(Vector3.zero, scaleTime3));

		var sequence2 = DOTween.Sequence();
		sequence2.Append(glow.DOColor(Color.white, glowTime1));
		sequence2.Append(glow.DOColor(Color.white, glowTime2));
		sequence2.Append(glow.DOColor(new Color(1,1,1,0), glowTime3));
	}

	private void SetPosition()
	{
		Vector3 screenPoint = mainCamera.WorldToScreenPoint(targetPosition);
		screenPoint.x = Mathf.Clamp(screenPoint.x, limits.xMin, Camera.main.pixelWidth - limits.xMax);
		screenPoint.y = Mathf.Clamp(screenPoint.y, limits.yMin, Camera.main.pixelHeight - limits.yMax);
		Vector2 result;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvas, 
			screenPoint, 
			BattleInstanceInterface.instance.UICamera, 
			out result);

		rectTransform.localPosition = result + new Vector2(0f, -40f);
	}
}
