using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocaleBehaviour : MonoBehaviour
{
    void Start()
    {
        var text = GetComponent<TMP_Text>();
        if (text == null)
            return;

        string temp = text.text;
        if (temp.Length > 0)
        {
            text.text = Locales.Get(temp);
        }
    }
}
