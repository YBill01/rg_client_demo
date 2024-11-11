using Legacy.Database;
using System.Diagnostics;
using Unity.Entities;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(BattleSystems))]
	public class BattleInitialization : ComponentSystemGroup
	{

	}

	[UpdateInGroup(typeof(BattleSystems))]
	[UpdateAfter(typeof(BattleInitialization))]
	public class BattleSimulation : ComponentSystemGroup
	{

	}
	[UpdateInGroup(typeof(BattleSystems))]
	[UpdateAfter(typeof(BattleSimulation))]
	public class BattlePresentation : ComponentSystemGroup
	{

	}

	[UpdateInGroup(typeof(SimulationSystemGroup))]
	public class BattleSystems : ComponentSystemGroup
	{
		private BattleInitialization _initialization;
		private BattleSimulation _simulation;
		private BattlePresentation _presentation;

        private Stopwatch _timer;
        public long CurrentTime { get { return _timer.ElapsedMilliseconds; } }
        public float DeltaTime { get; private set; }

        private long _last_time;
        private InputSystem _input;
		//private EntityQuery _errors;
		private bool _active = false;
		private bool error_state;
		public static byte versionResult;

		protected override void OnCreate()
		{
#if UNITY_EDITOR
			var domain = AppInitSettings.Instance.GetBinaryDatabaseHost();
			versionResult = BinaryDatabase.Instance.Read(AppInitSettings.Instance.UseLocalBinary, domain);
			//AnalyticsManager.Instance.BinaryDomainRead(domain);
#else
			var dom = AppInitSettings.Instance.GetBinaryDatabaseHost();
			versionResult = BinaryDatabase.Instance.Read(false, dom);
			//AnalyticsManager.Instance.BinaryDomainRead(dom);
#endif

			_initialization = World.GetOrCreateSystem<BattleInitialization>();
			_simulation = World.GetOrCreateSystem<BattleSimulation>();
			_presentation = World.GetOrCreateSystem<BattlePresentation>();

            _timer = new Stopwatch();
            _timer.Start();
			base.OnCreate();
        }

		public void PauseTimer()
		{
			_timer.Stop();
		}

		public void ResumeTimer()
		{
			_timer.Start();
		}

		protected override void OnUpdate()
        {
            DeltaTime = (_timer.ElapsedMilliseconds - _last_time) * 0.001f;
            _last_time = _timer.ElapsedMilliseconds;
            base.OnUpdate();
        }

        protected override void OnDestroy()
        {
            BinaryDatabase.Instance.Dispose();
        }

		public void Initialization(bool value)
		{
			EntityManager.CompleteAllJobs();
			if (value)
			{
				AddSystemToUpdateList(_initialization);
			}
			else
			{
				RemoveSystemFromUpdateList(_initialization);
			}
		}

		public void Simulation(bool value)
		{
			EntityManager.CompleteAllJobs();
			_active = value;
			if (value)
			{
				AddSystemToUpdateList(_simulation);
			}
			else
			{
				RemoveSystemFromUpdateList(_simulation);
			}
		}

		public void Presentation(bool value)
		{
			EntityManager.CompleteAllJobs();
			if (value)
			{
				AddSystemToUpdateList(_presentation);
			}
			else
			{
				RemoveSystemFromUpdateList(_presentation);
			}
		}

	}
}
