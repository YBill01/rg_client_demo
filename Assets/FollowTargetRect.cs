using Legacy.Client;
using UnityEngine;

public class FollowTargetRect : MonoBehaviour
{
	public GameObject target;
	private RectTransform rectTransform;
	void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	void Update()
	{
		if (!target) return;
		var screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);
		Vector2 result;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), screenPoint, BattleInstanceInterface.instance.UICamera, out result);

		rectTransform.anchoredPosition = result;
	}
}
