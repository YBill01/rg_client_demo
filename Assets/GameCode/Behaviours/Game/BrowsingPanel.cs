using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

namespace Legacy.Client
{

    public class BrowsingPanel : MonoBehaviour
	{

		static public BrowsingPanel Instance;
		public Button BattleButton;
		void Start()
		{
			Instance = this;
			BattleButton.onClick.AddListener(OnBattleClick);
		}

		private void OnBattleClick()
		{
			BattleButton.interactable = false;
			BattleButton.GetComponentInChildren<Text>().text = "search opponent";
			NetworkMessageHelper.Battle1x1();
			//StartCoroutine("SendMessageBattle");			
		}

   //     IEnumerator SendMessageBattle()
   //     {
			//if(faderCanvas)
			//	faderCanvas.Loader.FadeOut();
   //         yield return new WaitForSeconds(0.5f);
   //         //NetworkMessage.Battle1x1(ClientWorld.Instance.EntityManager);
   //     }

	}
}
