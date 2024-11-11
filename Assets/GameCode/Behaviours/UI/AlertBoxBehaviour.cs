using TMPro;
using UnityEngine;

public class AlertBoxBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject RedAlert;
    [SerializeField]
    private GameObject GreenAlert;
    [SerializeField]
    private TextMeshProUGUI GreenAlertText;
    [SerializeField]
    private Animator GreenAlertAnimator;
    [SerializeField]
    private GameObject YellowAlert;
    [SerializeField]
    private GameObject ArrowAlert;
    [SerializeField]
    private Animator ArrowAlertAnimator;

    [SerializeField]
    private TextMeshProUGUI AlertText;

    [SerializeField]
    private GameObject FreeAlert;

    public void ShowRedAlert(string text)
    {
        RedAlert.SetActive(true);
        AlertText.gameObject.SetActive(true);

        AlertText.text = text;
    }

    public void ShowGreenAlert(string text, bool animate)
    {
        GreenAlert.SetActive(true);
        GreenAlertAnimator.enabled = animate;
        GreenAlertText.text = text;
    }

    public void ShowYellowAlert(string text)
    {
        YellowAlert.SetActive(true);
        AlertText.gameObject.SetActive(true);

        AlertText.text = text;
    }

    public void ShowFreeAlert()
    {
        FreeAlert.SetActive(true);
    }

    public void ShowArrowAlert(string text = "", bool animate = true)
    {
        ArrowAlert.SetActive(true);
        AlertText.gameObject.SetActive(true);
        AlertText.text = text;
        ArrowAlertAnimator.enabled = animate;
    }

    public void HideAll()
    {
        RedAlert.SetActive(false);
        GreenAlert.SetActive(false);
        YellowAlert.SetActive(false);
        FreeAlert.SetActive(false);
        ArrowAlert.SetActive(false);
        AlertText.gameObject.SetActive(false);
    }
}
