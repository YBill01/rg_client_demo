using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class UpPanelElementBehaviour : MonoBehaviour
    {
        [SerializeField] UpPanelBehaviour UpPanel;
        [SerializeField] UpPanelItem type;

        public bool IsActive()
        {
            return UpPanel.IsItemEnabled(type);
        }
    }
}
