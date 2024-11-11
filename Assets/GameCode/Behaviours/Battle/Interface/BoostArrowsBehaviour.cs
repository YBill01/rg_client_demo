using Legacy.Client;
using UnityEngine;
using UnityEngine.UI;

public class BoostArrowsBehaviour : MonoBehaviour
{
    public Sprite OneBoostArrow;
    public Sprite TwoBoostArrows;

    public ParticleSystem BoomParticles;
    public ParticleSystem BoomRedParticles;

    public void UpdateState(bool isYellow)
    {
        if (BridgeHighlightSystem.BridgeBoost > 0)
        {
            {
                if (BridgeHighlightSystem.BridgeBoost > 1)
                {
                    GetComponent<Image>().sprite = TwoBoostArrows;
                }
                else
                {
                    GetComponent<Image>().sprite = OneBoostArrow;
                }
            }
        }
        BoomParticles.gameObject.SetActive(!isYellow);
        BoomRedParticles.gameObject.SetActive(isYellow);
    }
}
