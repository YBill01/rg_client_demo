using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Legacy.Database;

namespace Legacy.Client
{
    [AlwaysUpdateSystem]
	[UpdateInGroup(typeof(BattleSimulation))]

	public class SpawnMessageSystem : ComponentSystem
	{
		private EntityQuery _player_query;

		protected override void OnCreate()
		{
			_player_query = GetEntityQuery(
				ComponentType.ReadOnly<BattlePlayer>(),
				ComponentType.ReadOnly<BattlePlayerOwner>()
			);
		}

		public bool PlayCard(byte CardID, Vector3 Position)
		{
			if (Acted) return false;
			Acted = true;
			Act = new NextAction
			{
                isSkill = false,
				CardID = CardID,
				Position = new float2(Position.x, Position.z)
			};
			return true;
		}

		private bool Acted;
		private NextAction Act;

		private struct NextAction
		{
            public bool isSkill;
			public byte CardID;
			public float2 Position;
		}
		protected override void OnUpdate()
		{
			if (!_player_query.IsEmptyIgnoreFilter)
			{
				
				if (!Acted) return;
				float2 pos = new float2(Act.Position.x, Act.Position.y);

                if (Act.isSkill)
                {
                    Debug.Log("Playing skill - " + Act.CardID.ToString());
					ClientWorld.Instance.ActionPlay(PlayerGameMessage.ActionSkill, Act.CardID, pos);
                }
                else
                {
                    Debug.Log("Playing card - " + Act.CardID.ToString());
					ClientWorld.Instance.ActionPlay(PlayerGameMessage.ActionCard, Act.CardID, pos);
                    //NetworkMessage.ActionPlayCard(EntityManager, Act.CardID, pos);
                }
                Acted = false;
			}
		}

        internal bool PlaySkill(byte index, Vector3 position)
        {
            if (Acted) return false;
            Acted = true;
            Act = new NextAction
            {
                isSkill = true,
                CardID = index,
                Position = new float2(position.x, position.z)
            };
            return true;
        }
    }
}
