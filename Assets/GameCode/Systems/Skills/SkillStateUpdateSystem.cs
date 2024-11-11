using Legacy.Database;
using Legacy.Game;
using Unity.Entities;

namespace Legacy.Client
{
	[UpdateInGroup(typeof(BattleSimulation))]

	public class SkillStateUpdateSystem : ComponentSystem
	{

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<BattleInstance>();
		}

		protected override void OnUpdate()
		{

			var _battle = GetSingleton<BattleInstance>();
			var _player = _battle.players[_battle.players.player];
			var settings = Settings.Instance.Get<BaseBattleSettings>().bridges;

			if (_battle.status < BattleInstanceStatus.Playing)
				return;

			byte myBridgesCount = (byte)(_battle.bridges.top == _player.side ? 1 : 0);
			myBridgesCount += (byte)(_battle.bridges.down == _player.side ? 1 : 0);
			var skillSpeed = (100 + _battle.bridges.GetSkillBonus(_player.side, settings)) / 100f;

			BattleInstanceInterface.instance.Skill1.UpdateSkill(_player.skill1, myBridgesCount, skillSpeed,_battle.status);
            BattleInstanceInterface.instance.Skill2.UpdateSkill(_player.skill2, myBridgesCount, skillSpeed, _battle.status);
			//BattleInstanceInterface.instance.Rage.UpdateSkill(_player.skillTimer, myBridgesCount, skillSpeed);
			BattleInstanceInterface.instance.skillsPositionsBehaviour.UpdateSkillsPositions(_player.skill1, _player.skill2);
		}	
	}
}
