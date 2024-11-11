using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
	//[DisableAutoCreation]
	//[AlwaysUpdateSystem]
	//[UpdateInGroup(typeof(BattleSimulation))]

	//public class SkillsSystem : ComponentSystem
	//{
	//	private GameObject SkillsContainer;
	//	private EntityQuery _non_spawned_cards_query;
	//	private EntityQuery _spawned_cards_query;
	//	protected override void OnCreate()
	//	{
	//		_non_spawned_cards_query = GetEntityQuery(
	//			ComponentType.ReadOnly<SkillComponent>(),
	//			ComponentType.ReadOnly<PropertyOfMe>(),
	//			ComponentType.Exclude<SkillBehaviourScript>(),
	//			ComponentType.Exclude<RectTransform>()
	//		);
	//		_spawned_cards_query = GetEntityQuery(
	//			ComponentType.ReadOnly<SkillComponent>(),
	//			ComponentType.ReadOnly<PropertyOfMe>(),
	//			ComponentType.ReadOnly<SkillBehaviourScript>(),
	//			ComponentType.ReadOnly<RectTransform>()
	//		);
	//	}

	//	protected override void OnUpdate()
	//	{
	//		GetSkillsContainer();
	//		if (SkillsContainer == null) return;
	//		SpawnCards();
	//		UpdateCards();
	//	}

	//	private void GetSkillsContainer()
	//	{
	//		if (SkillsContainer != null) return;
	//		SkillsContainer = GameObject.FindGameObjectWithTag("SkillContainer");
	//	}

	//	private void SpawnCards()
	//	{
	//		if (_non_spawned_cards_query.IsEmptyIgnoreFilter) return;
	//		NativeArray<Entity> entities = _non_spawned_cards_query.ToEntityArray(Allocator.TempJob);
	//		NativeArray<SkillComponent> skills = _non_spawned_cards_query.ToComponentDataArray<SkillComponent>(Allocator.TempJob);
	//		for (int i = 0; i < entities.Length; i++)
	//		{
	//			Entity currentEntity = entities[i];
	//			SkillComponent skill = skills[i];

	//			uint posID = skill.SkillOrderID + 1;
	//			Transform t = SkillsContainer.transform.Find("SkillPosition" + posID.ToString());
	//			if (t == null) continue;
	//			GameObject SkillItemContainer = t.gameObject;

	//			var _loaded = Resources.Load<GameObject>("Prefabs/Skills/Skill");
	//			var _game_object = UnityEngine.GameObject.Instantiate(_loaded, SkillItemContainer.transform);
	//			_game_object.AddComponent<EntityProxyBehaviour>().Entity = currentEntity;
	//			GameObjectEntity.AddToEntity(EntityManager, _game_object, currentEntity);
	//		}
	//		skills.Dispose();
	//		entities.Dispose();
	//	}

	//	private void UpdateCards()
	//	{
	//		if (_spawned_cards_query.IsEmptyIgnoreFilter) return;
	//		NativeArray<Entity> entities = _spawned_cards_query.ToEntityArray(Allocator.TempJob);
	//		NativeArray<SkillComponent> skills = _spawned_cards_query.ToComponentDataArray<SkillComponent>(Allocator.TempJob);
	//		for (int i = 0; i < entities.Length; i++)
	//		{
	//			Entity currentEntity = entities[i];
	//			SkillComponent skillComponent = skills[i];
	//			//EntityManager.GetComponentObject<SkillBehaviourScript>(currentEntity).UpdateSkillData(skillComponent);
	//		}
	//		skills.Dispose();
	//		entities.Dispose();
	//	}
	//}
}
