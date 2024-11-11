//using Unity.Jobs;
//using Unity.Entities;
//using Unity.Collections;
//using Unity.Mathematics;
//using Legacy.Database;

//namespace Legacy.Server
//{
//    [UpdateInGroup(typeof(ServerSimulationSystemGroup))]
//    public class SandboxMinionsSkillsSystem : JobComponentSystem
//	{
//		private BeginSimulationEntityCommandBufferSystem _barrier;
//		private NativeHashMap<Entity, byte> _damages;
//		private NativeQueue<EffectTriggerData> _events;
//		private EntityQuery _query_skills;
//		private EntityQuery _query_damages;

//		unsafe protected override void OnCreate()
//		{
//			_barrier = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

//			_damages = new NativeHashMap<Entity, byte>(32, Allocator.Persistent);
//			_events = new NativeQueue<EffectTriggerData>(Allocator.Persistent);

//			_query_skills = GetEntityQuery(
//				ComponentType.ReadWrite<MinionData>(),
//				ComponentType.ReadOnly<StateAlive>(),
//				ComponentType.ReadOnly<EntityDatabase>(),
//				ComponentType.ReadWrite<StateSkill>(),
//				ComponentType.Exclude<PauseState>()
//			);

//			_query_damages = GetEntityQuery(
//				ComponentType.ReadWrite<EffectData>(),
//				ComponentType.ReadWrite<EffectServerData>(),
//				ComponentType.ReadOnly<EffectDamage>(),
//				ComponentType.Exclude<EntityDatabase>(),
//				ComponentType.Exclude<PauseState>()
//			);

//			var _query_battle = GetEntityQuery(
//				ComponentType.ReadOnly<BattleInstance>(),
//				ComponentType.ReadOnly<SandboxInstance>()
//			);

//			RequireForUpdate(_query_battle);
//			RequireForUpdate(_query_skills);			
//		}

//		protected override void OnDestroy()
//		{
//			_damages.Dispose();
//			_events.Dispose();
//		}

//		protected override JobHandle OnUpdate(JobHandle inputDeps)
//		{
//			inputDeps = new SkillsJob
//			{
//				buffer = _barrier.CreateCommandBuffer().AsParallelWriter(),
//				time = ServerInitializationSystemGroup.ElapsedMilliseconds,
//				created = _damages.AsParallelWriter(),
//				events = _events.AsParallelWriter()
//			}.Schedule(_query_skills, inputDeps);

//			// deny all damage prepare
//			if (!_query_damages.IsEmptyIgnoreFilter)
//			{
//				inputDeps = new DenyEffectJob
//				{
//					damages = _damages
//				}.Schedule(_query_damages, inputDeps);
//			}

//			inputDeps = new CompleteJob
//			{
//				buffer = _barrier.CreateCommandBuffer(),
//				damages = _damages,
//				events = _events
//			}.Schedule(inputDeps);

//			_barrier.AddJobHandleForProducer(inputDeps);

//			return inputDeps;
//		}

//		struct CompleteJob : IJob
//		{
//			public EntityCommandBuffer buffer;

//			internal NativeHashMap<Entity, byte> damages;
//			internal NativeQueue<EffectTriggerData> events;

//			public void Execute()
//			{
//				damages.Clear();

//				if (events.Count > 0)
//				{
//					while (events.TryDequeue(out EffectTriggerData trigger))
//					{
//						var _entity = buffer.CreateEntity();
//						buffer.AddComponent(_entity, trigger);
//					}
//				}
//			}
//		}

//		[Unity.Burst.BurstCompile]
//		struct DenyEffectJob : IJobForEachWithEntity<EffectData, EffectServerData>
//		{
//			[ReadOnly] public NativeHashMap<Entity, byte> damages;

//			public void Execute(
//				Entity entity,
//				int index,
//				ref EffectData effect,
//				ref EffectServerData system
//			)
//			{
//				if(effect.state < EffectState.Active)
//				{
//					if (damages.TryGetValue(system.source, out byte _index))
//					{
//						if (_index == effect.source)
//						{
//							effect.state = EffectState.Expire;
//							system.timestate = 0;
//						}
//					}
//				}				
//			}
//		}

//		//[Unity.Burst.BurstCompile]
//		struct SkillsJob : IJobForEachWithEntity<MinionData, StateSkill, EntityDatabase>
//		{
//			internal long time;
//			public EntityCommandBuffer.ParallelWriter buffer;
//			internal NativeHashMap<Entity, byte>.ParallelWriter created;
//			internal NativeQueue<EffectTriggerData>.ParallelWriter events;

//			public void Execute(
//				Entity entity,
//				int index, 
//				ref MinionData minion,
//				ref StateSkill state,
//				[ReadOnly] ref EntityDatabase database
//			)
//			{
//				switch (state.status)
//				{
//					case StateSkillStatus.Create:
//						created.TryAdd(entity, database.index); // deny damage
//						state.status = StateSkillStatus.Spawned;
//						if (!state.position.Equals(float2.zero))
//						{
//						var _forward = math.normalize(state.position);
//							minion.mrotation = (short)UnityEngine.Quaternion.LookRotation(
//								new float3(_forward.x, 0f, _forward.y),
//								new float3(0f, 1f, 0f)
//							).eulerAngles.y;
//						}
//						break;

//					case StateSkillStatus.Spawned:
//						minion.state = MinionState.SkillPoint + state.index + 1;
//						state.status = StateSkillStatus.Timer;
//						events.Enqueue(new EffectTriggerData
//						{
//							type = TriggerEvent.OnSkillStart,
//							battle = database.battle,
//							esource = entity,
//							etarget = entity,
//							index = byte.MaxValue,
//							level = minion.level,
//							position = minion.mposition,
//							side = minion.side,
//							source = database.index,
//							target = database.index
//						});
//						break;

//					case StateSkillStatus.Timer:
//						if (time > state.expire)
//						{
//							state.status = StateSkillStatus.Expire;
//						}
//						break;

//					case StateSkillStatus.Expire:
//						minion.state = MinionState.Undefined;
//						buffer.RemoveComponent<StateSkill>(index, entity);
//						events.Enqueue(new EffectTriggerData
//						{
//							type = TriggerEvent.OnSkillStop,
//							battle = database.battle,
//							esource = entity,
//							etarget = entity,
//							index = byte.MaxValue,
//							level = minion.level,
//							position = minion.mposition,
//							side = minion.side,
//							source = database.index,
//							target = database.index
//						});
//						break;
//				}
//			}
//		}

//	}
//}