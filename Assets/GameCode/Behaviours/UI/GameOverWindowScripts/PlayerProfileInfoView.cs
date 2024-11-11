using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image playerAvatar;
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerClanName;
    [SerializeField] private Image playerClanAvatar;

    private uint profileId;

    public void SetDataToProfile(string name)
    {
        playerName.text = name;
    }


}
