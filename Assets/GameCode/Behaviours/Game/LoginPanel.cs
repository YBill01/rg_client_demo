using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Legacy.Game;

namespace Legacy.Client
{
    public class LoginPanel : MonoBehaviour
	{

		static public LoginPanel Instance;

		public InputField Email;
		//public Text PassText;
		//public Dropdown Player;
		public Button LoginButton;
		public Text Status;

        public Button BattleButtonNew;

		private StateMachineSystem homeSystems;

		private void OnBattleClick()
		{
			NetworkMessageHelper.Battle1x1();
			//BattleButtonNew.interactable = false;
			//BattleButtonNew.GetComponentInChildren<Text>().text = "search opponent";
			//coroutine2 = StartCoroutine(SendMessageBattle());
		}
   //     }

   //     IEnumerator SendMessageBattle()
   //     {
			//if (faderCanvas)
			//	faderCanvas.Loader.FadeOut();
			////GameObject.Find("FaderCanvas").GetComponent<FaderCanvas>().Loader.FadeOut();
			////yield return new WaitForSeconds(0.5f);
   //         NetworkMessage.Battle1x1(ClientWorld.Instance.EntityManager);
   //     }

        void Start()
		{
			homeSystems = ClientWorld.Instance.GetExistingSystem<StateMachineSystem>();

			homeSystems.OpponentSearchStartEvent.AddListener(OnSearch);

			Instance = this;
			LoginButton.gameObject.SetActive(true);
			LoginButton.onClick.AddListener(OnLoginClick);
			//Email.text = SystemInfo.deviceName;
            //BattleButtonNew.onClick.AddListener(OnBattleClick);

#if UNITY_EDITOR
#else
			Email.text += "STANDALONE";
#endif

            //var _request = ClientWorld.Instance.EntityManager.CreateEntity();
			//ClientWorld.Instance.EntityManager.AddComponentData(_request, new NetworkAuthorization
			//{
				//login = new FixedString64(Email.text)
			//});
		}

		private void OnSearch()
		{
			homeSystems.OpponentSearchStartEvent.RemoveListener(OnSearch);
			homeSystems.BattleCancelEvent.AddListener(OnCancelSearch);
			LoginButton.interactable = false;
		}

		private void OnCancelSearch()
		{
			homeSystems.BattleCancelEvent.RemoveListener(OnCancelSearch);
			homeSystems.OpponentSearchStartEvent.AddListener(OnSearch);
			LoginButton.interactable = true;
		}

		private Coroutine coroutine1;
		private Coroutine coroutine2;
		private void OnLoginClick()
		{
			//coroutine1 = StartCoroutine("StartBattle");
			NetworkMessageHelper.Battle1x1();
		}

  //      IEnumerator StartBattle()
  //      {
		//	//GameObject.Find("FaderCanvas").GetComponent<FaderCanvas>().Loader.FadeOut();
		//	//if (faderCanvas)
		//	//	faderCanvas.Loader.FadeOut();
		//	//yield return new WaitForSeconds(0.5f);

		//	NetworkMessage.Battle1x1(ClientWorld.Instance.EntityManager);

		//	//var _state_machine = 
		//	//StateMachineSystem sms = ClientWorld.Instance.GetExistingSystem<StateMachineSystem>();
		//	//sms.

		//	//var _request = ClientWorld.Instance.EntityManager.CreateEntity();
		//	//ClientWorld.Instance.EntityManager.AddComponentData(_request, new NetworkAuthorization
		//	//{
		//	//    Login = new FixedString64(Email.text)
		//	//});

		//	/*var _message = default(NetworkMessageRaw);
		//	_message.WriteUShort(7777);
		//	_message.size = 0;

		//	var _request = ClientWorld.Instance.EntityManager.CreateEntity();
		//	ClientWorld.Instance.EntityManager.AddComponentData(_request, new NetworkMessage
		//	{
		//		data = _message,
		//		protocol = (byte)ObserverPlayerMessage.EnterBattle
		//	});*/

		//	//LoginButton.interactable = false;
		//	//Status.text = "Connecting...";
		//}

		internal void Connecting()
		{
			LoginButton.interactable = false;
		}

		private Coroutine coroutine;

		internal void Disconnect(string warning)
		{
			coroutine = StartCoroutine(ButtonDelay(warning));			
		}

		IEnumerator ButtonDelay(string warning)
		{
			yield return new WaitForSeconds(3);
			LoginButton.interactable = true;
			//Status.text = warning;
		}

		private void OnDestroy()
		{
			if (coroutine != null)
				StopCoroutine(coroutine);
			if (coroutine1 != null)
				StopCoroutine(coroutine1);
			if (coroutine2 != null)
				StopCoroutine(coroutine2);
			homeSystems.BattleCancelEvent.RemoveListener(OnCancelSearch);
			homeSystems.OpponentSearchStartEvent.RemoveListener(OnSearch);
		}
	}
}
