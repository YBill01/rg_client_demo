using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
    public class ConnectedUnitBehaviour : MonoBehaviour
    {
        public MinionInitBehaviour initBehaviour;
        public string PrefabToConnect;
        public Animator connectedAnimator;
        public Transform connectedTransform;

        void Update()
        {
            if (connectedTransform == null) return;
            connectedTransform.position = transform.position;
            connectedTransform.localRotation = transform.localRotation;
        }

        private bool isConnectin;
        public void SetupConnectedUnit(BinaryEntity entity)
        {
            if (isConnectin) return;
            isConnectin = true;
            ObjectPooler.instance.GetMinion(entity, ConnectIt);
        }

        public void UnsetConnectedUnit()
        {
            isConnectin = false;
            if (connectedTransform == null) return;
            connectedTransform.gameObject.SetActive(false);
            ReleaseConnectedUnit();
        }

        private void OnDisable()
        {
            isConnectin = false;
            ReleaseConnectedUnit();
        }

        public void ReleaseConnectedUnit()
        {
            isConnectin = false;
            if (connectedTransform == null) return;
            connectedTransform.parent = transform.parent;
            connectedTransform = null;
            connectedAnimator = null;
        }

        private void ConnectIt(GameObject gameObject)
        {
            if (!isConnectin) return;
            connectedTransform = gameObject.transform;
            connectedTransform.parent = transform.parent;
            connectedAnimator = gameObject.GetComponent<Animator>();

            gameObject.SetActive(true);
        }
    }
}
