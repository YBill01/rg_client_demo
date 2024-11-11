using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class DeckTabPanel : MonoBehaviour
    {
        [SerializeField] private TabButtonBehaviour[] Tabs;
        private ProfileInstance Profile;
        public void Init() 
        {
            Profile = ClientWorld.Instance.Profile;
            for (byte i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].Init((byte)Profile.DecksCollection.Active_set_id);
            }
        }

        public void CloseInTutor()
        {
            for (byte i = 0; i < Tabs.Length; i++)
            {
               Tabs[i].gameObject.SetActive(Profile.DecksCollection.In_collection.Length!=0);
            }
        }
    }
}