using Legacy.Client;
using Legacy.Database;
using System.Linq;
using UnityEngine;

public class DeckAlertBoxBehaviour : MonoBehaviour
{
    [SerializeField]
    AlertBoxBehaviour alertBox;

    private void Start()
    {
        var profile = ClientWorld.Instance.Profile;
        profile.PlayerProfileUpdated.AddListener(UpdateAlertBox);
        profile.Inventory.AllertUpdated.AddListener(UpdateAlertBox);
        UpdateAlertBox();
    }

	private void OnDestroy()
	{
        var profile = ClientWorld.Instance.Profile;
        profile.PlayerProfileUpdated.RemoveListener(UpdateAlertBox);
        profile.Inventory.AllertUpdated.RemoveListener(UpdateAlertBox);
    }

    private void UpdateAlertBox()
    {
        var profile = ClientWorld.Instance.Profile;
        var cards = profile.Inventory.AvailableCards;

        int countToUpgrade = 0;
        int countToUpgradeInDeck = 0;
        int countOfNew = 0;

        foreach (var card in cards)
        {
            if (card.CanUpgrade) 
            {
                if (card.SoftToUpgrade > profile.Stock.GetCount(Legacy.Database.CurrencyType.Soft))
                    continue;

                if (profile.DecksCollection.ActiveSet.Cards.Any(x => x == card.index))
                    countToUpgradeInDeck++;
                else
                    countToUpgrade++;
            }

            if (card.isNew)
            {
                countOfNew++;
            }
        }

        alertBox.HideAll();

        if (countToUpgrade + countToUpgradeInDeck > 0 && !profile.HasSoftTutorialState(SoftTutorial.SoftTutorialState.UpgradeCard) )
            alertBox.ShowArrowAlert((countToUpgrade + countToUpgradeInDeck).ToString(), countToUpgradeInDeck > 0);
        else if (countOfNew != 0)
            alertBox.ShowRedAlert(countOfNew.ToString());
        else if (countToUpgrade + countToUpgradeInDeck > 0)
            alertBox.ShowArrowAlert((countToUpgrade + countToUpgradeInDeck).ToString(), countToUpgradeInDeck > 0);

    }
}
