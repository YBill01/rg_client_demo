using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Legacy.Client { 

    public class DropDownHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Dropdown tmp_dropdown;
        [SerializeField] private SettingsWindowBehaviour settings; // временный вариант, пока нет рабочего расскрывающегося списка
        public void OnPointerClick(PointerEventData eventData)
        {
            //   StartCoroutine(Show());
            settings.OnSelectLangue();
        }

        IEnumerator Show()
        { 

            bool ch = true;
            while (ch) {
                if (tmp_dropdown.IsExpanded)
                {
                    ch = false;
                }    // else
                {
                    yield return new WaitForSeconds(0.05f); // защита от зацикливания.
                }
            }
         //   yield return new WaitForSeconds(1f);
            Canvas parentCanvas = GetComponent<Canvas>();
            foreach (Transform child in transform)
            {
               if(child.name == "Dropdown List")
                {
                    Canvas childCanvas = child.GetComponent<Canvas>();
                    if (childCanvas && parentCanvas)
                    {
                        childCanvas.overrideSorting = false;
                    }
                   break;
               }   
            }
     
            yield return null;
         }
    }
}