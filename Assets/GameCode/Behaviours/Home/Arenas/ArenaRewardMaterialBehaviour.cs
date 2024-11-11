using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy.Client
{
    public class ArenaRewardMaterialBehaviour : ArenaBasicRewardBehaviour
    {
        [SerializeField] 
        private List<Image> imagesForGrayOut;
        [SerializeField] 
        private List<GameObject> objectsForTurnOff;
        
        public void Init(ushort count)
        {
            SetAmount(count.ToString());
        }

        public override void SetActiveState()
        {
            base.SetActiveState();
            
            MakeGray(false);
            TurnOn(true);
        }
        
        public void MakeGray(bool toggle)
        {
            if (imagesForGrayOut == null) return;

            for (int i = 0; i < imagesForGrayOut.Count; i++)
            {
                imagesForGrayOut[i].material = toggle ? VisualContent.Instance.GrayScaleMaterial : null;
            }
        }

        public void TurnOn(bool state)
        {
            if(objectsForTurnOff == null) return;

            for (int i = 0; i < objectsForTurnOff.Count; i++)
            {
                objectsForTurnOff[i].SetActive(state);
            }
        }

        public override void SetCompleteState()
        {
            base.SetCompleteState();
            
            MakeGray(false);
            TurnOn(true);
        }

        public override void SetLockedState()
        {
            base.SetLockedState();
            
            //MakeGray(true);
            TurnOn(false);
        }
    }
}