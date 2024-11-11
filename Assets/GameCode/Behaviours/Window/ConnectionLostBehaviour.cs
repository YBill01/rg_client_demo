using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConnectionLostBehaviour : MonoBehaviour
{
	[SerializeField]
	private Image icon;

	private static GameObject instance;

	public static void ShowLostConnection(bool active)
	{
        if (instance == null) return;
		//if (active == instance.activeSelf)
		//	return;

		instance.SetActive(active);
	}

	private void Start()
	{
		if (instance != null) 
		{
			Destroy(gameObject);
			return;
		}

		instance = gameObject;

		DontDestroyOnLoad(this);
		gameObject.SetActive(false);

		var sequence = DOTween.Sequence();

		sequence.Append(icon.DOFade(1, 0.66f));
		sequence.Append(icon.DOFade(0, 0.66f));
		sequence.Append(icon.DOFade(1, 0.66f));
		sequence.SetLoops(-1);
	}


}