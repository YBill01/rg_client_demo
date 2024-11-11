using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class LootBoxesBehaviour : MonoBehaviour
    {
        [SerializeField]
        private List<LootBoxBehaviour> LootBoxes;
        
        public void InitBoxes(MainWindowBehaviour mainWindowBehaviour)
        {
            var playerLoots = ClientWorld.Instance.Profile.loot;
            byte i = 0;
            bool potentialOpenening = false, isOpenening = false;
            foreach (PlayerProfileLootBox box in playerLoots.boxes)
            {
                byte boxNumber = (byte)(i + 1);
                LootBoxes[i].Init(playerLoots, mainWindowBehaviour, boxNumber);

				if (!isOpenening)
				{
                    isOpenening = box.started;
				}
				else if (!potentialOpenening)
				{
                    potentialOpenening = box.index > 0;
                }

                i++;
                if (i == LootBoxes.Count) break;
            }

            /*PushNotifications.Instance.ChestReminderLocalNotificationCancel();
            if (potentialOpenening && !isOpenening)
			{
                // Отправка push-notification
                PushNotifications.Instance.ChestReminderLocalNotificationStart();
            }*/
        }

        public List<LootBoxBehaviour> GetBoxes()
        {
            return LootBoxes;
        }

        public void IfBoxArived()
        {
            foreach (var  box in LootBoxes)
            {
                LootBoxBehaviour lb = box.GetComponent<LootBoxBehaviour>();
                if (lb && lb.isJump)
                {
                    lb.Arrived();
                }
                Debug.Log("IfBoxArived");
            }
        }
    }
}
