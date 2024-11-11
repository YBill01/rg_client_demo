using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    public class CurrenciesBehaviour : MonoBehaviour
    {
        //[SerializeField]
        //private ChangeValueBehaviour Shards;
        //[SerializeField]
        //private GameObject ShardsPlusButton;
        [SerializeField]
        private ChangeValueBehaviour Soft;
        [SerializeField]
        private GameObject SoftPlusButton;
        [SerializeField]
        private ChangeValueBehaviour Hard;
        [SerializeField]
        private GameObject HardPlusButton;
        private ProfileInstance profile;
        public void SetShards(int v)
        {
            //Shards.SetValue(v);
        }
        public void UpdateValue()
        {
            if (profile == null)
                profile = ClientWorld.Instance.Profile;
            if (profile != null)
            {
                Hard.SetValue((int)profile.Stock.getItem(CurrencyType.Hard).Count);
                Soft.SetValue((int)profile.Stock.getItem(CurrencyType.Soft).Count);
            }
        }
        public void SetSoft(int v)
        {
            Soft.SetValue(v);
        }

        public void SetHard(int v)
        {
            Hard.SetValue(v);
        }

        public void EnableHard(bool v)
        {
            Hard.GetComponent<FadeElementBehaviour>().Enable(v);
        }
        
        public void EnableSoft(bool v)
        {
            Soft.GetComponent<FadeElementBehaviour>().Enable(v);
        }

        public void EnableShards(bool v)
        {
            //Shards.GetComponent<FadeElementBehaviour>().Enable(v);
        }

        public void EnablePlusButtons(bool v)
        {
            //ShardsPlusButton.SetActive(v);
            SoftPlusButton.SetActive(v);
            HardPlusButton.SetActive(v);
        }

        public void OnHardPlusButtonClick()
        {
            if (ClientWorld.Instance.Profile.IsBattleTutorial)
            {
                PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
            }
            else
                WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankGems);
        }

        public void OnSoftPlusButtonClick()
        {
            if (ClientWorld.Instance.Profile.IsBattleTutorial)
            {
                PopupAlertBehaviour.ShowHomePopupAlert(Input.mousePosition, Locales.Get("locale:1483"));
            }else
            WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankCoins);
        }

        public void OnShardsPlusButtonClick()
        {
            WindowManager.Instance.MainWindow.OpenShopWithSection(RedirectMenuSection.BankLoots);
        }
    }
}
