using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileUpdatedValueDelayBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    //просто отображение в эдиторе
    [SerializeField] private bool canUpdate = true;
    [SerializeField] private string value;
    [SerializeField] private string previousValue;

    private void OnEnable()
    {
        GetPreviousValue();
        canUpdate = true;
        SetValue();
    }

    public void GetPreviousValue()
    {
        previousValue = valueText.text;
    }

    public void SendValue(string value)
    {
        this.value = value;
    }

    private void SetValue()
    {
        valueText.text = value;
    }

    public void SetCanUpdate(bool canUpdate)
    {
        this.canUpdate = canUpdate;
        SetValue();

    }

    private void Update()
    {
    }
}
