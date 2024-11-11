namespace Legacy.Client
{
    /*[UpdateInGroup(typeof(BattleInitialization))]

	public class DespawnSystem : JobComponentSystem
	{
		
        private EntityQuery _query_replicated;

		private NetworkSystems _network;
		private NativeQueue<Entity> _dispose;

		protected override void OnCreate()
		{
			_network = World.GetOrCreateSystem<NetworkSystems>();

			_query_replicated = GetEntityQuery(
				ComponentType.ReadOnly<Transform>(),
				ComponentType.ReadOnly<NetworkReplicated>(),
				ComponentType.Exclude<BattleInstance>(),
				ComponentType.Exclude<NetworkDispose>()
			);
			_dispose = new NativeQueue<Entity>(Allocator.Persistent);
		}

		protected override void OnDestroy()
		{
			_dispose.Dispose();
			base.OnDestroy();
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
            if (!_query_replicated.IsEmptyIgnoreFilter)
            {
                inputDeps = new CollectReplicated
                {
                    dispose = _dispose.AsParallelWriter(),
                    time = _network.CurrentTime
                }.Schedule(_query_replicated, inputDeps);

                inputDeps.Complete();

                while (_dispose.TryDequeue(out Entity entity))
                {
                    var _tranform = EntityManager.GetComponentObject<Transform>(entity);
                    //var _childs = _tranform.GetComponentsInChildren<ParticleSystem>();
                    //for (int i = 0; i < _childs.Length; ++i)
                    //{
                        //var _particle = _childs[i];
                        //if (_particle.isPlaying)
                        //{
                            //_particle.Stop();
                            //var main = _particle.main;
                            //main.stopAction = ParticleSystemStopAction.Disable;
                        //}
                    //}
                    var deathEffect = _tranform.GetComponent<DeathEffect>();
                    if (deathEffect != null)
                    {
                        deathEffect.Die();
                    }
                    var proxy = _tranform.GetComponent<EntityProxyBehaviour>();
                    if(proxy != null)
                    {
                        proxy.SetDefaults();
                    }

                    _tranform.gameObject.SetActive(false);
                    
                    EntityManager.DestroyEntity(entity);
                }
            }		

			return inputDeps;
		}

		[Unity.Burst.BurstCompile]
		[ExcludeComponent(typeof(Transform))]
		struct CollectReplicated : IJobForEachWithEntity<NetworkReplicated>
		{
			public NativeQueue<Entity>.ParallelWriter dispose;
			public long time;

			public void Execute(Entity entity, int index, [ReadOnly] ref NetworkReplicated repl)
			{
				if (time - repl.snapshottime > 2000u)
				{
					dispose.Enqueue(entity);
				}
			}
		}

	}*/
}

