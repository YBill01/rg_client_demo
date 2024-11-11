using Legacy.Client;
using Legacy.Database;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class MinionBubble : MonoBehaviour
{
	[SerializeField]
	private TutorialMessageBehaviour tutorialMessage;
	public ushort minionID;
	[SerializeField]
	private BattlePlayerSide minionSide = BattlePlayerSide.Left;
	public ushort bubbleTime;

	[SerializeField]
	private GameObject heroMessagePrefab;
	private GameObject heroMessageInsance;
	private GameObject heroUnit;

	public void ShowMinionBubble()
	{
		var manager = ClientWorld.Instance.EntityManager;

		var _minions_query = manager.CreateEntityQuery(
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<MinionData>(),
			ComponentType.ReadOnly<EntityDatabase>()
		);

		var _minions = _minions_query.ToEntityArray(Allocator.TempJob);
		var _transforms = _minions_query.ToComponentArray<Transform>();
		for (int i = 0; i < _minions.Length; ++i)
		{
			var _db = manager.GetComponentData<EntityDatabase>(_minions[i]);
			var _minion = manager.GetComponentData<MinionData>(_minions[i]);
			if (_db.db == minionID && (minionSide == BattlePlayerSide.None || minionSide == _minion.side) )
			{
				heroUnit = _transforms[i].gameObject;
			}
		}

		_minions.Dispose();
		
		heroMessageInsance = GameObject.Instantiate(heroMessagePrefab, tutorialMessage.transform.parent);
		var ftr = heroMessageInsance.GetComponent<FollowTargetRect>();
		ftr.target = heroUnit;
		var blt = heroMessageInsance.GetComponent<BubbleLifetime>();
		blt.Lifetime = bubbleTime;
		blt.InitLifetime();
	}
}
