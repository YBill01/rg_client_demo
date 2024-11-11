using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Legacy.Client
{
    [DisableAutoCreation]
	//[AlwaysUpdateSystem]
	[UpdateInGroup(typeof(BattleSimulation))]

	public class DeckSystem : ComponentSystem
	{
		private GameObject DeckContainer;
		private EntityQuery _non_spawned_cards_query;
		private EntityQuery _spawned_cards_query;
		protected override void OnCreate()
		{
			_non_spawned_cards_query = GetEntityQuery(
				ComponentType.ReadOnly<CardDataComponent>(),
				//ComponentType.ReadOnly<PropertyOfMe>(),
				ComponentType.Exclude<RectTransform>()
			);
			_spawned_cards_query = GetEntityQuery(
				ComponentType.ReadOnly<CardDataComponent>(),
				//ComponentType.ReadOnly<PropertyOfMe>(),
				ComponentType.ReadOnly<BattleCardBehaviour>(),
				ComponentType.ReadOnly<RectTransform>()
			);
		}

		protected override void OnUpdate()
		{
			GetDeckContainer();
			if (DeckContainer == null) return;
			SpawnCards();
			//UpdateCards();
		}

		private void GetDeckContainer()
		{
			if (DeckContainer != null) return;
			DeckContainer = GameObject.Find("MinionsPanel");
		}

		private void SpawnCards()
		{
			if (_non_spawned_cards_query.IsEmptyIgnoreFilter) return;
			NativeArray<Entity> entities = _non_spawned_cards_query.ToEntityArray(Allocator.TempJob);
			for (int i = 0; i < entities.Length; i++)
			{
				Entity currentEntity = entities[i];

                var address = Addressables.InstantiateAsync("Prefabs/Canvas/Deck/Card.prefab", DeckContainer.transform);
                address.Completed += (AsyncOperationHandle<GameObject> async) =>
               {
                   GameObjectEntity.AddToEntity(EntityManager, async.Result.gameObject, currentEntity);
               };
			}
			entities.Dispose();
		}

		private void UpdateCards()
		{
			if (_spawned_cards_query.IsEmptyIgnoreFilter) return;
			NativeArray<Entity> entities = _spawned_cards_query.ToEntityArray(Allocator.TempJob);
			NativeArray<CardDataComponent> deckCardComponents = _spawned_cards_query.ToComponentDataArray<CardDataComponent>(Allocator.TempJob);
			for (int i = 0; i < entities.Length; i++)
			{
				Entity currentEntity = entities[i];
				CardDataComponent deckCardComponent = deckCardComponents[i];
				EntityManager.GetComponentObject<BattleCardBehaviour>(currentEntity).UpdateCardData(deckCardComponent.DBID);
			}
			deckCardComponents.Dispose();
			entities.Dispose();
		}
	}
}
