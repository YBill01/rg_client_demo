//using Unity.Jobs;
//using Unity.Entities;
//using Unity.Collections;
//using Legacy.Database;

//namespace Legacy.Server
//{
//	[UpdateInGroup(typeof(ServerInitializationSystemGroup))]
//	public class SandboxHeroSkillsCooldownSystem : JobComponentSystem
//	{
//		EntityQuery _query_skills;

//		protected override void OnCreate()
//		{
//			_query_skills = GetEntityQuery(
//				ComponentType.ReadWrite<SkillData>(),
//				ComponentType.ReadOnly<HeroSkill>()
//			);

//			var _query_sandbox = GetEntityQuery(
//				ComponentType.ReadOnly<SandboxInstance>()
//			);

//			RequireForUpdate(_query_sandbox);
//		}

//		protected override JobHandle OnUpdate(JobHandle inputDeps)
//		{
//			inputDeps = new UpdateSkillsJob
//			{
//			}.Schedule(_query_skills, inputDeps);

//			return inputDeps;
//		}

//		struct UpdateSkillsJob : IJobForEach<SkillData>
//		{
//			public void Execute(ref SkillData skill)
//			{
//				skill.timer = 0;
//			}
//		}
//	}
//}
