using UnityEngine;
using UnityEditor;
using TMPro;
using Legacy.Client;
using DG.Tweening;

public class SpendManaBehaviour : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI manaText;

	private RectTransform rectTransform;
	private Camera mainCamera;
	private RectTransform canvas;

	private Vector3 targetPosition;
	private byte manaCost;

	private float startTime;
	
	private const float flyTime = 1.3f;
	private const float flyDistance = 70;

	

	private Rect limits = new Rect()
	{
		xMin = 50,
		xMax = 100,
		yMin = 100,
		yMax = 50
	};

	public void SetInitValues(Vector3 targetPosition, byte manaCost)
	{
		this.targetPosition = targetPosition;
		this.manaCost = manaCost;
	}
	
	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		mainCamera = Camera.main;
		canvas = BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>();
		
		startTime = Time.time;

		manaText.text = $"-{manaCost}";

		transform.DOScale(Vector3.one * 1.3f, flyTime).SetEase(Ease.InOutFlash, 4);

		Destroy(gameObject, flyTime);
	}

	private void Update()
	{
		SetPosition();
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

		var shift = 90f + (Time.time - startTime) / flyTime * flyDistance;

		rectTransform.localPosition = result + new Vector2(0f, shift);
	}
}