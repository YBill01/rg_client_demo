using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

namespace Legacy.Client
{
	class ServerConnection
	{
        private static ServerConnection _instance = null;
        public static ServerConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServerConnection();
                }
                return _instance;
            }
        }

        private NetworkDriver _driver;
        private NetworkDriver.Concurrent _driver_concurrent;

        public NetworkDriver Driver => _driver;
        public NetworkDriver.Concurrent DriverConcurrent => _driver_concurrent;

        private NetworkPipeline _reliable_pipeline;
        public NetworkPipeline ReliablePipeline => _reliable_pipeline;

        private NetworkPipeline _unreliable_pipeline;
        public NetworkPipeline UnreliablePipeline => _unreliable_pipeline;

        public ServerConnection()
        {
            Create();
        }

        public void Create()
        {
            var reliabilityParams = new ReliableUtility.Parameters { WindowSize = 32 };
            _driver = NetworkDriver.Create(reliabilityParams);            

            _unreliable_pipeline = _driver.CreatePipeline(typeof(NullPipelineStage));
            _reliable_pipeline = _driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

            _driver_concurrent = _driver.ToConcurrent();
        }

        public void Dispose()
        {
            if (_driver.IsCreated)
            {
                _driver.Dispose();
            }
        }
    }
}
