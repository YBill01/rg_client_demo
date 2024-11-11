using UnityEngine;
using Unity.Entities;
using Legacy.Database;
using Unity.Collections;
using Unity.Jobs;

namespace Legacy.Client
{
	[UpdateInGroup(typeof(BattlePresentation))]
	public class HeroRageVisualizationSystem : ComponentSystem
	{
		private bool leftIsOn;
		private bool rightIsOn;

		private EntityQuery rageEffectQuery;
		private EntityQuery battleQuery;
		private EntityQuery minionsQuery;

		protected override void OnCreate()
		{		
			rageEffectQuery = GetEntityQuery(
				ComponentType.ReadOnly<EffectHeroRage>(),
				ComponentType.ReadOnly<EffectData>(),

				ComponentType.Exclude<StateDeath>());

			battleQuery = GetEntityQuery(
				ComponentType.ReadOnly<BattleInstance>()
			);

			minionsQuery = GetEntityQuery(
				ComponentType.ReadOnly<MinionData>(),
				ComponentType.ReadWrite<MinionInitBehaviour>()
			);

			RequireForUpdate(battleQuery);
		}

		protected override void OnUpdate()
		{
			var battle = battleQuery.GetSingleton<BattleInstance>();
			var player = battle.players[battle.players.player];

			var rages = rageEffectQuery.ToComponentDataArray<EffectData>(Allocator.TempJob);

			var leftIsOn = false;
			var rightIsOn = false;

			foreach (var rage in rages)
			{
				var isIamInRage = player.side == rage.side;

				if (isIamInRage)
					leftIsOn = true;
				else
					rightIsOn = true;
			}

			RageSetActive(leftIsOn, true);
			RageSetActive(rightIsOn, false);

			rages.Dispose();

			foreach (var minion in MinionInitBehaviour.MinionsList)
			{
				minion.atRage = minion.atBattle && (minion.IsEnemy && rightIsOn || !minion.IsEnemy && leftIsOn);
			}
		}

		private void RageSetActive(bool active, bool isLeft)
		{
			if (isLeft ? active == leftIsOn : active == rightIsOn)
				return;

			leftIsOn = isLeft ? active : leftIsOn;
			rightIsOn = isLeft ? rightIsOn : active;

			var particles = isLeft ? StaticColliders.instance.LeftHeroRageParticles : StaticColliders.instance.RightHeroRageParticles;

			foreach (var ps in particles)
			{
				if (active)
				{
					ps.gameObject.SetActive(true);
					var psMain = ps.main;
					psMain.loop = true;

					ps.Play();
				}
				else
				{
					var psMain = ps.main;
					psMain.loop = false;
				}
			}
		}
	}
}