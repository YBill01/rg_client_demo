//using Unity.Entities;
//using Legacy.Database;
//using Unity.Collections;

//namespace Legacy.Server
//{
//    [UpdateInGroup(typeof(ServerInitializationSystemGroup))]
//    public class TutorialSkillHardcodeSystem : ComponentSystem
//    {
//        EntityQuery skillsQuery;

//        public struct SkillUpdated : IComponentData
//        { 
//        }

//        protected override void OnCreate()
//        {
//            skillsQuery = GetEntityQuery(
//               ComponentType.ReadOnly<SkillData>(),
//               ComponentType.ReadOnly<HeroSkill>(),

//               ComponentType.Exclude<SkillUpdated>()
//            );

//            RequireSingletonForUpdate<TutorialInstance>();
//            RequireSingletonForUpdate<BattleInstance>();
//            RequireForUpdate(skillsQuery);
//        }

//        protected override void OnUpdate()
//        {
//            var skills = skillsQuery.ToEntityArray(Allocator.TempJob);

//            for (int i = 0; i < skills.Length; i++)
//            {
//                var skill = skills[i];
//                var skillData = EntityManager.GetComponentData<SkillData>(skill);

//                EntityManager.AddComponentData(skill, new SkillUpdated());

//                var tutorial = GetSingleton<TutorialInstance>();
//                if (tutorial.index != 2)
//                    continue;

//                if (skillData.side != BattlePlayerSide.Left)
//                    continue;

//                if (skillData.index != 0)
//                    continue;

//                skillData.cooldown = 17000;
//                skillData.timer = 17000;

//                EntityManager.SetComponentData(skill, skillData);                
//            }

//            skills.Dispose();
//        }
//	}
//}

