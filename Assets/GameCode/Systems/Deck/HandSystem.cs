using Legacy.Database;
using Legacy.Game;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattlePresentation))]

    public class HandSystem : ComponentSystem
    {

        private EntityQuery _battle;

        protected override void OnCreate()
        {
            _battle = GetEntityQuery(
                ComponentType.ReadOnly<BattleInstance>()
            );

        }

        protected override void OnUpdate()
        {
			var _battle = GetSingleton<BattleInstance>();

            if (_battle.isSandbox)
                return;

            switch (_battle.status)
			{
				case BattleInstanceStatus.Prepare:
                    BattleInstanceInterface.instance.hand.UpdateHand(_battle.players[_battle.players.player].hand, true);
                    break;
                case BattleInstanceStatus.Playing:
                case BattleInstanceStatus.Pause:
					BattleInstanceInterface.instance.hand.UpdateHand(_battle.players[_battle.players.player].hand);
					break;
				default:
					break;
			}
        } 
    }
}
