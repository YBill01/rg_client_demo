/*using Legacy.Database;
using Unity.Entities;


namespace Legacy.Server
{
	[UpdateInGroup(typeof(ServerInitializationSystemGroup))]

	public class RageTriggerSystem : ComponentSystem
	{

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<BattleInstance>();
			RequireSingletonForUpdate<TutorialInstance>();
		}

		protected override void OnUpdate()
		{
			var battle = GetSingleton<BattleInstance>();
			if (battle.status != BattleInstanceStatus.Playing && battle.status != BattleInstanceStatus.Prepare)
				return;

			var settings = Settings.Instance.Get<HeroRageSettings>();

			var player = battle.players[battle.players.player];
			if (player.skillTimer < settings.cooldown * 0.1f && player.skillTimer > settings.cooldown * 0.05f)
			{
				var entity = EntityManager.CreateEntity();
				EntityManager.AddComponentData(entity, new EventInstance { trigger = TutorialEventTrigger.OnRageAlmostReady });
			}
			else if (player.skillTimer == 0)
			{
				var entity = EntityManager.CreateEntity();
				EntityManager.AddComponentData(entity, new EventInstance { trigger = TutorialEventTrigger.OnRageReady });
			}

		}
	}
}
*/

