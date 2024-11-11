using UnityEngine;
using System.Collections;

public class ShowPopupBehaviour : MonoBehaviour
{
	[SerializeField]
	string messageText;
	
	void Start()
	{
		Vector2 messagePos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
		PopupAlertBehaviour.ShowBattlePopupAlert(messagePos, messageText);
	}

}
