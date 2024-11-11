//using Unity.Entities;
//using Unity.Collections;
//using Legacy.Database;
//using Unity.Jobs;
//using UnityEngine;

//namespace Legacy.Server
//{
//	[UpdateInGroup(typeof(ServerInitializationSystemGroup))]
	
//	public class SandboxSkillsInitiazliationSystem : JobComponentSystem
//	{
//		private EntityQuery _query_events;
//		private EntityQuery _query_minions;
//		private EndInitializationEntityCommandBufferSystem _barrier;

//		unsafe protected override void OnCreate()
//		{
//			_query_events = GetEntityQuery(
//				ComponentType.ReadOnly<SystemTriggerData>()
//			);

//			_query_minions = GetEntityQuery(
//				ComponentType.ReadOnly<MinionData>(),
//				ComponentType.ReadOnly<MinionSkills>(),
//				ComponentType.ReadOnly<EntityDatabase>(),
//				ComponentType.Exclude<StateAlive>(),
//				ComponentType.Exclude<PauseState>(),
//				ComponentType.Exclude<MinionHeroTag>()
//			);

//			var _query_battle = GetEntityQuery(
//				ComponentType.ReadOnly<BattleInstance>(),
//				ComponentType.ReadOnly<SandboxInstance>()
//			);

//			_barrier = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();

//			RequireForUpdate(_query_events);
//			RequireForUpdate(_query_minions);
//			RequireForUpdate(_query_battle);
//		}

//		protected override JobHandle OnUpdate(JobHandle inputDeps)
//		{
//			var _buffer = _barrier.CreateCommandBuffer();
//			_buffer.DestroyEntity(_query_events);

//			inputDeps = new InitMinionsSkillsJob
//			{
//				triggers = _query_events.ToComponentDataArray<SystemTriggerData>(Allocator.TempJob),
//				buffer = _buffer.AsParallelWriter(),
//				heroes = GetComponentDataFromEntity<MinionHeroTag>(true)
//			}.Schedule(_query_minions, inputDeps);

//			_barrier.AddJobHandleForProducer(inputDeps);

//			return inputDeps;
//		}

//		struct InitMinionsSkillsJob : IJobForEachWithEntity<MinionData, MinionSkills, EntityDatabase>
//		{
//			[ReadOnly] public ComponentDataFromEntity<MinionHeroTag> heroes;
//			[ReadOnly, DeallocateOnJobCompletion] public NativeArray<SystemTriggerData> triggers;
//			public EntityCommandBuffer.ParallelWriter buffer;

//			public void Execute(
//				Entity entity, 
//				int index, 
//				[ReadOnly] ref MinionData minion,
//				[ReadOnly] ref MinionSkills skills,
//				[ReadOnly] ref EntityDatabase database
//			)
//			{
//				for (int i = 0; i < triggers.Length; ++i)
//				{
//					var _trigger = triggers[i];

//					if (_trigger.battle != database.battle) continue;
//					if (_trigger.minion != database.index) continue;
//					if (_trigger.source != entity) continue;

//					for (byte k = 0; k < MinionSkills.Count; ++k)
//					{
//						if (skills.Get(k, out ushort skill))
//						{
//							if (Database.Skills.Instance.Get(skill, out BinarySkill bskill))
//							{
//								var _skill_level = minion.level;

//								#region find hero skills
//								if (heroes.HasComponent(entity))
//								{
//									var _hero_skills = heroes[entity];
//									if (_hero_skills.skill1.index == skill)
//									{
//										_skill_level = _hero_skills.skill1.level;
//									}

//									if (_hero_skills.skill2.index == skill)
//									{
//										_skill_level = _hero_skills.skill2.level;
//									}
//								}
//								#endregion

//								var _cooldown = (ushort)bskill.cooldown._value(_skill_level);
//								var _skill_entity = buffer.CreateEntity(index);
//								buffer.AddComponent(index, _skill_entity, new SkillData
//								{
//									db = skill,
//									index = k,
//									minion = entity,
//									level = _skill_level,
//									side = minion.side,

//									battle = database.battle,
//									source = database.index,

//									type = bskill.type,
//									cooldown = _cooldown,
//									duration = bskill.duration,
//									timer = _cooldown
//								});

//								if(_cooldown > 0)
//								{
//									buffer.AddComponent<SkillCDTag>(index, _skill_entity);
//								}

//								if (bskill.component > 0)
//								{
//									Components.Instance.Attach(bskill.component, skill, index, buffer, _skill_entity);
//								}
//							}
//						}
//					}
//				}
//			}
//		}

//	}
//}