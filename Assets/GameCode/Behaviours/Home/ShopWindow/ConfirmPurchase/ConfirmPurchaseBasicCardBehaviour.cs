using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class ConfirmPurchaseBasicCardBehaviour : MonoBehaviour
    {
        protected PlayerDailyDealsItem data;
        protected BinaryBank bankData;

        public virtual void InitData(PlayerDailyDealsItem item)
        {
            data = item;
            UpdateData();
        }

        public virtual void InitData(BinaryBank item)
        {
            bankData = item;
            UpdateData();
        }        

        protected virtual void UpdateData() { }

    }
}
