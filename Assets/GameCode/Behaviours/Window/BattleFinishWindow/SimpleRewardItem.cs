using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleRewardItem : MonoBehaviour
{
    public int id;
    public int count;
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private List<Sprite> sprites;
    void Start()
    {
    }

    public void ResetData(int id, int count)
    {
        this.id = id;
        this.count = count;
        image.sprite = sprites[id];
        image.SetNativeSize();
        text.text = "+" + count.ToString();
    }
}
