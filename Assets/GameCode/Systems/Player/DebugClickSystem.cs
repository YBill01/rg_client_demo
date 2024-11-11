using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Legacy.Client
{
#if UNITY_EDITOR
    //[DisableAutoCreation]
    [AlwaysUpdateSystem]
	[UpdateInGroup(typeof(BattleSimulation))]

	public class DebugClickSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
		
		}

		protected override void OnUpdate()
		{
			/*if (Input.GetMouseButtonDown(0))
			{
				var _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(_ray, out RaycastHit hit))
				{
					if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Grid"))
					{
						NetworkMessage.ActionDebugCard(EntityManager, 22, new float2(hit.point.x, hit.point.z));
					}
				}
			}*/

		}
	}
#endif
}
