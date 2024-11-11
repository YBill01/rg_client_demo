using UnityEngine;
using System.Collections;
using Legacy.Client;
using System;

public class OnHeroMessageClickBehaviour : MonoBehaviour
{
	void Update()
	{
		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
		{
			if (WindowManager.Instance.CurrentWindow is ArenaWindowBehaviour)
			{
				var window = WindowManager.Instance.CurrentWindow as ArenaWindowBehaviour;
				window.gameObject.GetComponent<ArenaListBehaviour>().StartScrolling();
				window.StartScroll();
			}

			SoftTutorialManager.Instance.MenuTutorialPointer.HideHeroMessage();
			//MenuTutorialPointerBehaviour.CreateOnTapEntity();
		}
	}
}
