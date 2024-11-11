using UnityEngine;
using System.Collections;
using Legacy.Database;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using TMPro;
using Legacy.Client;
using static Legacy.Client.CardProgressBarBehaviour;

public class BattleCardViewBehaviour : MonoBehaviour
{
	[SerializeField]
	private Image GlowImage;

    [SerializeField]
    private Image Frame;

    [SerializeField]
    private RaritySprites RarityFrameSprites;

    [SerializeField]
    private Image Icon;

    [SerializeField]
    private TextMeshProUGUI manaText;
    [SerializeField]  private AudioClip cardReady;

    //[Foldout("Frames", true)]
    [SerializeField] public Image cardIconMask;
    [SerializeField] public Sprite simpleMask;
    [SerializeField] public Sprite legendMask;
    [Space]
    [SerializeField] public Image outerFrame;
    [SerializeField] public Sprite simpleOuterFrame;
    [SerializeField] public Sprite legendOuterFrame;
    [Space]
    [SerializeField] public Image innerFrame;
    [SerializeField] public Sprite simpleInnerFrame;
    [SerializeField] public Sprite legendInnerFrame;
    [Space]
    [SerializeField] private Sprite outerFrameSpriteCommon;
    [SerializeField] private Sprite outerFrameSpriteRare;
    [SerializeField] private Sprite outerFrameSpriteEpic;
    [SerializeField] private Sprite outerFrameSpriteLegendary;

    private ushort cardIndex;
    public ushort CardIndex => cardIndex;
    private bool isGray;

    internal void Init(BinaryCard binaryCard)
    {
        cardIndex = binaryCard.index;
        SetRarity(binaryCard.rarity);
        manaText.text = binaryCard.manaCost.ToString();

        Icon.sprite = VisualContent.Instance.CardIconsAtlas.GetSprite(binaryCard.icon);
    }

    void SetRarity(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common:
                outerFrame.sprite = outerFrameSpriteCommon;
                SetUpRegularCardFrames();
                break;
            case CardRarity.Rare:
                outerFrame.sprite = outerFrameSpriteRare;
                SetUpRegularCardFrames();
                break;
            case CardRarity.Epic:
                outerFrame.sprite = outerFrameSpriteEpic;
                SetUpRegularCardFrames();
                break;
            case CardRarity.Legendary:
                SetUpLegendaryCardFrames();
                break;
        }
    }

    public void SetUpLegendaryCardFrames()
    {
        outerFrame.type = Image.Type.Simple;
        innerFrame.type = Image.Type.Simple;
        cardIconMask.type = Image.Type.Simple;

        var rect = outerFrame.GetComponent<RectTransform>().rect;
        rect.height = legendOuterFrame.bounds.size.y;
        outerFrame.sprite = legendOuterFrame;

        rect = innerFrame.GetComponent<RectTransform>().rect;
        rect.height = legendInnerFrame.bounds.size.y;
        innerFrame.sprite = legendInnerFrame;

        rect = cardIconMask.GetComponent<RectTransform>().rect;
        rect.height = legendMask.bounds.size.y;
        cardIconMask.sprite = legendMask;
    }

    public void SetUpRegularCardFrames()
    {
        outerFrame.type = Image.Type.Sliced;
        innerFrame.type = Image.Type.Sliced;
        cardIconMask.type = Image.Type.Sliced;

        var rect = outerFrame.GetComponent<RectTransform>().rect;
        rect.height = simpleOuterFrame.bounds.size.y;
        outerFrame.sprite = simpleOuterFrame;

        rect = innerFrame.GetComponent<RectTransform>().rect;
        rect.height = simpleInnerFrame.bounds.size.y;
        innerFrame.sprite = simpleInnerFrame;

        rect = cardIconMask.GetComponent<RectTransform>().rect;
        rect.height = simpleMask.bounds.size.y;
        cardIconMask.sprite = simpleMask;
    }

    internal void Glow(bool glow)
    {
        //GlowImage.gameObject.SetActive(glow);
    }

    public void SetGray(bool gray)
    {
        if (gray == isGray)
            return;

        isGray = gray;
        if (!gray)
            PlayClip(cardReady);
        Icon.material = gray ? VisualContent.Instance.GrayScaleMaterial : null;
        outerFrame.material = gray ? VisualContent.Instance.GrayScaleMaterial : null;
    }
    private void PlayClip(AudioClip clip)
    {
        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }
    }
}
