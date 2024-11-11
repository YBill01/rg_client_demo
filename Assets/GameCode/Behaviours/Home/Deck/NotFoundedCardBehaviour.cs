using Legacy.Database;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{

    public class NotFoundedCardBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI ArenaText;

        //[SerializeField]
        //private GameObject Lock;

        [SerializeField]
        private LegacyButton button;

        //[SerializeField]
        //private GameObject OpenedText;

        internal void Opened(byte arenaNumber, BinaryCard card)
        {
            //Lock.SetActive(false);
            //OpenedText.SetActive(false);
            ArenaText.text = Locales.Get("locale:1333");

            button.interactable = false;
            button.isLocked = true;
            button.localeAlert = Locales.Get("locale:1357");
            ComingSoon(arenaNumber, card);
        }
        private void ComingSoon(byte arenaNumber, BinaryCard card)
        {
            if (/*arenaNumber > ArenaTemporarySettings.Instance.RealArenasCount &&*/ card.coming_soon)
            {
                //Lock.SetActive(true);
                //OpenedText.SetActive(false);
                ArenaText.text = Locales.Get("locale:1291");

                button.interactable = false;
                button.isLocked = true;
                button.localeAlert = Locales.Get("locale:1291");
            }
        }

        internal void Locked(byte arenaNumber, BinaryCard card)
        {
            //Lock.SetActive(true);
            //OpenedText.SetActive(true);
            ArenaText.text = Locales.Get("locale:712") + " " + arenaNumber.ToString();

            button.interactable = false;
            button.isLocked = true;
            var NumbeArenaText = Locales.Get("locale:712") + " " + arenaNumber.ToString();
            button.localeAlert = Locales.Get("locale:1336", NumbeArenaText);
            ComingSoon(arenaNumber, card);
        }
    }
}