using UnityEngine;
using Legacy.Client;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class PopupAlertBehaviour : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI message;

	[SerializeField]
	Image back;

	private const float duration = 2;
	private static GameObject alert = null;

	public static void ShowHomePopupAlert(string text)
	{
		ShowHomePopupAlert(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), text);
	}

	public static void ShowBattlePopupAlert(string text)
	{
		ShowBattlePopupAlert(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), text);
	}

	public static void ShowBattlePopupAlert(Vector2 screenPosition, string text, float dur = 2)
	{
		var canvas = BattleInstanceInterface.instance.canvas.GetComponent<Canvas>();
		ShowPopupAlert(screenPosition, text, canvas, dur);
	}

	public static void ShowHomePopupAlert(Vector2 screenPosition, string text)
	{
		var canvas = WindowManager.Instance.MainCanvas;
		ShowPopupAlert(screenPosition, text, canvas,duration);
	}

	private static void ShowPopupAlert(Vector2 screenPosition, string text, Canvas canvas, float dur)
	{
		var canvasRect = canvas.GetComponent<RectTransform>();
		
		var prefab = VisualContent.Instance.customVisualData.PopupAlertPrefab;
        
		alert = Instantiate(prefab, canvasRect);

		var rect = alert.GetComponent<RectTransform>();
		
		//Показываем чуть выше пальца
		screenPosition.y = screenPosition.y + Screen.height * 0.06f;

		var pos = screenPosition - canvas.pixelRect.size / 2f;
		pos /= canvas.pixelRect.size / canvasRect.rect.size;

		var limitX = canvasRect.rect.width / 2f - rect.rect.width / 2f;
		var limitY = canvasRect.rect.height / 2f - rect.rect.height / 2f;

		pos.x = Mathf.Clamp(pos.x, -limitX, limitX);
		pos.y = Mathf.Clamp(pos.y, -limitY, limitY - 100f); // -100 даем запас на подняие сообщения

		rect.anchoredPosition = pos;

		var popupBehaviour = alert.GetComponent<PopupAlertBehaviour>();

		popupBehaviour.message.text = text;
		rect.DOAnchorPosY(pos.y + 100f, dur).SetEase(Ease.InSine);

		popupBehaviour.back.DOFade(0, dur * 0.3f).SetEase(Ease.InSine).SetDelay(dur * 0.7f);
		popupBehaviour.message.DOFade(0, dur * 0.3f).SetEase(Ease.InSine).SetDelay(dur * 0.7f);

		popupBehaviour.back.GetComponent<Canvas>().sortingLayerName = "Alerts";
		popupBehaviour.message.GetComponent<Canvas>().sortingLayerName = "Alerts";
		var sequence = DOTween.Sequence();
		
		sequence.Append(rect.DOScale(Vector3.one * 1.05f, 0.1f));
		sequence.Append(rect.DOScale(Vector3.one, 0.05f));

		Destroy(alert, dur);
	}

}