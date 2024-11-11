using System;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

namespace Legacy.Client
{
	class ObserverConnection
	{
        private static ObserverConnection _instance = null;
        public static ObserverConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObserverConnection();
                }
                return _instance;
            }
        }

        private NetworkDriver _driver;
		public NetworkDriver Driver => _driver;

		private NetworkPipeline _reliable_peline;
		public NetworkPipeline ReliablePeline => _reliable_peline;

		private NetworkPipeline _unreliable_pipeline;
        public NetworkPipeline UnreliablePeline => _unreliable_pipeline;

        private NetworkEndPoint _network_point;

        public ObserverConnection()
        {
            Create();
        }

		private void Create()
		{
            var reliabilityParams = new ReliableUtility.Parameters { WindowSize = 32 };
            _driver = NetworkDriver.Create(reliabilityParams);

            _unreliable_pipeline = _driver.CreatePipeline(typeof(NullPipelineStage));
            _reliable_peline = _driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
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
