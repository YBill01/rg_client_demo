using Legacy.Database;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SkillViewBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image Icon;

    [SerializeField]
    private Image Frame;

    public BinarySkill binaryData;

    public Image IconContent { get => Icon; }

    internal void Init(BinarySkill binarySkill)
    {
        binaryData = binarySkill;

        Icon.sprite = VisualContent.Instance.SkillsIconsAtlas.GetSprite(binaryData.icon);            

    }

    internal void MakeGray(bool toggle = false)
    {
        Icon.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
        Frame.material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
    }
}
