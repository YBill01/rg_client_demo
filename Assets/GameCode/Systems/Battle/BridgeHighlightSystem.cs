using Unity.Entities;
using UnityEngine;
using Legacy.Game;
using Legacy.Database;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]
    [UpdateAfter(typeof(MinionTransformSystem))]

    
    public class BridgeHighlightSystem : ComponentSystem
    {
        public static BattlePlayerSide currentSide1 = BattlePlayerSide.None;
        public static BattlePlayerSide currentSide2 = BattlePlayerSide.None;

        public static bool bridge1 = false;
        public static bool bridge2 = false;

        public static byte BridgeBoost {
            get
            {
                return (byte)((bridge1 ? 1 : 0) + (bridge2 ? 1 : 0));
            }
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BattleInstance>();
        }



        protected override void OnUpdate()
        {
			var battle_instance = GetSingleton<BattleInstance>();
			if (battle_instance.status == BattleInstanceStatus.Playing)
			{
				var battle_player = battle_instance.players[battle_instance.players.player];

				if (currentSide1 != battle_instance.bridges.top)
				{

                    var firstTime1 = false;
					if (currentSide1 == BattlePlayerSide.None)
					{
                        firstTime1 = true;
					}

                    bridge1 = battle_player.side == currentSide1;
                    currentSide1 = battle_instance.bridges.top;

                    var valueForBriedge = currentSide1;
                    if (battle_player.side == BattlePlayerSide.Right && valueForBriedge > BattlePlayerSide.None)
                        valueForBriedge = valueForBriedge == BattlePlayerSide.Right ? BattlePlayerSide.Left : BattlePlayerSide.Right;

                    //BattleInstanceInterface.instance.bridge1.GetComponent<Animator>().Play(animationName);
                    SetBridge(StaticColliders.instance.Bridge1, valueForBriedge, BridgeSide.Top, firstTime1);
                    
				}

				if (currentSide2 != battle_instance.bridges.down)
				{
                    var firstTime2 = false;
                    if (currentSide2 == BattlePlayerSide.None)
					{
                        firstTime2 = true;
					}

                    bridge2 = battle_player.side == currentSide2;
                    currentSide2 = battle_instance.bridges.down;

                    var valueForBriedge = currentSide2;
                    if (battle_player.side == BattlePlayerSide.Right && valueForBriedge > BattlePlayerSide.None)
                        valueForBriedge = currentSide2 == BattlePlayerSide.Right ? BattlePlayerSide.Left : BattlePlayerSide.Right;

                    
                    SetBridge(StaticColliders.instance.Bridge2, valueForBriedge, BridgeSide.Bottom, firstTime2);

                }
			}
		}

        private void SetBridge(GameObject bridge, BattlePlayerSide side, BridgeSide bridgeSide, bool firstTime)
        {            
            bridge.GetComponent<BridgeBehaviour>().SetSide(side, bridgeSide, firstTime);
        }
    }
}

