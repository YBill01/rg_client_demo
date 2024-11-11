using UnityEngine;
using System.Collections;

namespace Legacy.Client
{
	public class SandboxCardsDisableBehaviour : MonoBehaviour
	{
		Transform center;
		Collider boxCollider;
		public bool invert;

		private void Start()
		{
			center = BattleInstanceInterface.instance.canvas.transform;
			boxCollider = GetComponent<BoxCollider>();
		}


		void Update()
		{
			boxCollider.enabled = !invert ? transform.position.x < center.position.x : transform.position.x > center.position.x;
		}
	}
}