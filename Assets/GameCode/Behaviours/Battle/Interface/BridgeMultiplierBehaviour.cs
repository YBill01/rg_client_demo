using Legacy.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeMultiplierBehaviour : MonoBehaviour
{

   private bool yellow;
   public void EnemyBuff()
   {
        yellow = true;
        //ArrowsBoom();
        yellow = false;
   }

    public void ArrowsBoom()
    {
        //BattleInstanceInterface.instance.HiglightBridgeBoosters(yellow);
    }
}
