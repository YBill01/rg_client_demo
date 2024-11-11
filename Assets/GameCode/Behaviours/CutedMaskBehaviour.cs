using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CutedMaskBehaviour : MonoBehaviour
{
    [Range(0f, 1f)]
    public float Alpha;
    [Range(0f, 1f)]
    public float AlphaTopValue;
    public Image basicFader;
    public Image cutedFader;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetAlpha(cutedFader, (Alpha) * AlphaTopValue);
        SetAlpha(basicFader, (1 - Alpha) * AlphaTopValue);

    }

    private void SetAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            var c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }
}
