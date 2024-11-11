using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Database;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

namespace Legacy.Client
{
    public class CurrencyCardBehaviour : MonoBehaviour
    {
        internal CardViewBehaviour view;

        internal void Init(CurrencyType type, uint count)
        {
            string spriteName = "Soft";
            switch (type)
            {              
                case CurrencyType.Hard:
                    spriteName = "Hard";
                    break;
                //case CurrencyType.Shards:
                    //spriteName = "Shards";
                    //break;
                default:
                    break;
            }
            var sprite = VisualContent.Instance.CardIconsAtlas.GetSprite(spriteName);
            view.SetIconSprite(sprite);
            
        }
    }
}
