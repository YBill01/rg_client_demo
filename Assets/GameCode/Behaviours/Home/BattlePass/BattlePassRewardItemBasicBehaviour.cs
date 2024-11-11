using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class BattlePassRewardItemBasicBehaviour : MonoBehaviour
    {
        public virtual void ScaleToCurrentState()
        {
        }
        
        public virtual void ScaleToRegularState()
        {
        }

        public virtual void ScaleToCollectedState()
        {
            ScaleToRegularState();
        }
    }
}