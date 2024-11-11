using Legacy.Database;

using System.Diagnostics;

using Unity.Entities;

namespace Legacy.Client
{
    /*[DisableAutoCreation]
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
	public class ClientInitializationSystemGroup : ComponentSystemGroup
	{
        private Stopwatch _timer;
        public long CurrentTime { get { return _timer.ElapsedMilliseconds; } }

        protected override void OnCreate()
        {
            BinaryDatabase.Instance.Read(AppInitSettings.Instance.UseLocalBinary);
            _timer = new Stopwatch();
            _timer.Start();
        }

        protected override void OnDestroy()
        {
            BinaryDatabase.Instance.Dispose();
            base.OnDestroy();
        }
    }

    [DisableAutoCreation]
    [AlwaysUpdateSystem]
	[UpdateInGroup(typeof(SimulationSystemGroup))]
    public class ClientSimulationSystemGroup : ComponentSystemGroup
    {
    }

    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ClientPresentationSystemGroup : ComponentSystemGroup
    {
    }*/
}



