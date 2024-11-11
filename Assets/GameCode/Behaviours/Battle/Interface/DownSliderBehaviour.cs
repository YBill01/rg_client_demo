using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownSliderBehaviour : MonoBehaviour
{
    public Image _100;
    public Image _75;
    public Image _50;
    public Image _100_Lerp;
    public Image _75_Lerp;
    public Image _50_Lerp;
    private float value100 = 1;
    private float value75 = 1;
    private float value50 = 1;

    public float LerpSpeed = 0.2f;

    public void SetValue(float value)
    {        
        if (value > 0.75f)
        {
            float delta100 = value - 0.75f;
            value100 = delta100 / 0.25f;
            value75 = 1f;
            value50 = 1f;
        } else if (value > 0.5f && value <= 0.75f) {
            value100 = 0f;
            float delta75 = value - 0.5f;
            value75 = delta75 / 0.25f;
            value50 = 1f;
        } else if (value <= 0.5f)
        {
            value100 = 0f;
            value75 = 0f;
            value50 = value / 0.5f;
        }        
    }

    void Update()
    {
        _100.fillAmount = value100;
        _75.fillAmount = value75;
        _50.fillAmount = value50;
        _100_Lerp.fillAmount = Mathf.Lerp(_100_Lerp.fillAmount, value100, LerpSpeed);
        _75_Lerp.fillAmount = Mathf.Lerp(_75_Lerp.fillAmount, value75, LerpSpeed);
        _50_Lerp.fillAmount = Mathf.Lerp(_50_Lerp.fillAmount, value50, LerpSpeed);
    }
}
