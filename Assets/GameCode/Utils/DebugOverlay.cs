using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Legacy.Client
{

	public class DebugOverlay : MonoBehaviour
	{
		public static DebugOverlay instance;
		public Text Console;
		public GameObject View;
		private bool _active = false;
		public ScrollRect Scroll;

		void Start()
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				_active = !_active;
				View.SetActive(_active);
			}
		}

		static public void Write(string msg)
		{
			if (instance)
			{
				instance.WriteMessage(msg);
			}
		}

		private void WriteMessage(string msg)
		{
			Console.text += "\n" + msg;
			StartCoroutine(IFocusOn(Scroll));
		}

		IEnumerator IFocusOn(ScrollRect a)
		{
			yield return new WaitForEndOfFrame();
			a.verticalNormalizedPosition = -0.1f;
		}


	}

}