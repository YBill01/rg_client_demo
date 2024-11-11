using Legacy.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class BattleOutcomeBehaviour : MonoBehaviour
    {
        [InspectorName("InspectorObjects")]
        [SerializeField] private Image substrateResult;
        [SerializeField] private TextMeshProUGUI textResult;
        [SerializeField] private Image substrateGlow;
        [SerializeField] private Image taptoContinueBack;
        [SerializeField] private GameObject particlesResultContainer;
        [InspectorName("particles fireworks")]
        [SerializeField] private GameObject victoryConfettiFirst;
        [SerializeField] private GameObject victoryConfettiSecond;
        [SerializeField] private GameObject victoryConfettiShower;
        [InspectorName("result substrates")]
        [SerializeField] private Sprite drawImage;
        [SerializeField] private Sprite victoryImage;
        [SerializeField] private Sprite defeatImage;
        [InspectorName("result colors")]
        [SerializeField] private Color victoryTextColor;
        [SerializeField] private Color tapToContinueVictoryDrawBackColor;
        [SerializeField] private Color tapToContinueEdefatColorBackColor;
        [InspectorName("Music")]
        [SerializeField] private AudioSource audioSourceEndBattle;
        [SerializeField] private AudioClip victoryClip;
        [SerializeField] private AudioClip defeatClip;

        private AudioClip clip;
        public void SetMainView(bool result)
        {
            Sprite sprite = drawImage;
            Color color = Color.white;
            Color tapBack = Color.white;
            clip = defeatClip;
            switch (result)
            {
                case true:
                    sprite = victoryImage;
                    clip = victoryClip;
                    color = victoryTextColor;
                    tapBack = tapToContinueVictoryDrawBackColor;
                    break;
                case false:
                    sprite = defeatImage;
                    clip = defeatClip;
                    tapBack = tapToContinueEdefatColorBackColor;
                    foreach (Transform child in particlesResultContainer.transform)
                    {
                        var ps = child.GetComponent<ParticleSystem>();
                        var main = ps.main;
                        main.startColor = tapToContinueEdefatColorBackColor;
                    }
                    break;
                default:
                    break;
            }

            substrateResult.sprite = (sprite);
            textResult.text = ResultToString(result);
            textResult.color = color;
            taptoContinueBack.color = tapBack;
        }

        private string ResultToString(bool result)
        {
            switch (result)
            {
                case true:
                    return Locales.Get("locale:1174");
                case false:
                    return Locales.Get("locale:1177");
            }
            return "";
        }

        public void PlayMusic()
        {
            audioSourceEndBattle.clip = clip;
            audioSourceEndBattle.Play();
        }
        public void ActiveParticles(bool flag)
        {
            particlesResultContainer.SetActive(true);
            victoryConfettiFirst.SetActive(flag);
            victoryConfettiSecond.SetActive(flag);
            victoryConfettiShower.SetActive(flag);
        }

    }
}
