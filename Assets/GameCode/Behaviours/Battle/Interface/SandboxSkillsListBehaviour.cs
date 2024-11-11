using UnityEngine;
using Legacy.Database;
using Legacy.Client;
using Legacy.Server;
using Unity.Entities;
using System.Collections.Generic;

public class SandboxSkillsListBehaviour : MonoBehaviour
{
	[SerializeField]
	GameObject SandboxSkillPrefab;

	private List<BattleSkillDragBehaviour> skillBehaviours;

	void Start()
	{
		var root = transform.root;

		List<ushort> heroSkills = new List<ushort>();

		Entity playerHero = GetPlayerHeroEntity();

		foreach (var minion in Entities.Instance.List)
		{
			if (minion.type != MinionLayerType.Hero)
				continue;

			if (!Components.Instance.Get<MinionSkills>().TryGetValue(minion.index, out MinionSkills skills))
				continue;

			for (int j = 0; j < MinionSkills.Count; ++j)
			{
				skills.Get((byte)j, out ushort skill);
				if (skill != 0 && !heroSkills.Contains(skill))
					heroSkills.Add(skill);
			}
		}

		skillBehaviours = new List<BattleSkillDragBehaviour>(heroSkills.Count);

		byte i = 0;
		foreach (var skill in heroSkills)
		{
			if (!Skills.Instance.Get(skill, out BinarySkill binarySkill))
				continue;

			var skillObject = Instantiate(SandboxSkillPrefab, transform);
			var skillBehaviour = skillObject.GetComponentInChildren<BattleSkillDragBehaviour>();
			skillBehaviours.Add(skillBehaviour);

			var dragObject = new GameObject($"DragSkill {skill}");
			dragObject.transform.SetParent(root);
			dragObject.SetActive(false);

			skillBehaviour.dragObject = dragObject.transform;

			skillBehaviour.InitSkillView(binarySkill);
			skillBehaviour.SkillIndex = i;

			PrepareSkillData(binarySkill, i, playerHero);
			
			i++;
		}

		var rect = GetComponent<RectTransform>();
		var size = rect.sizeDelta;
		size.x = SandboxSkillPrefab.GetComponent<RectTransform>().sizeDelta.x * heroSkills.Count;
		rect.sizeDelta = size;
	}

	private void Update()
	{
		foreach (var skillBehaviour in skillBehaviours)
		{
			skillBehaviour.UpdateSkill(0, 0, 1,BattleInstanceStatus.Playing);
		}
	}

	private Entity GetPlayerHeroEntity()
	{
		var em = ClientWorld.Instance.EntityManager;

		var query = em.CreateEntityQuery(
			ComponentType.ReadOnly<MinionData>(),
			ComponentType.ReadOnly<MinionHeroTag>()
		);

		var heroes = query.ToEntityArray(Unity.Collections.Allocator.TempJob);

		foreach (var hero in heroes)
		{
			var minion = em.GetComponentData<MinionData>(hero);
			if (minion.side == BattlePlayerSide.Left)
			{
				heroes.Dispose();
				return hero;
			}
		}

		Debug.LogError("Cant find player hero");
		heroes.Dispose();
		return default;
	}

	private void PrepareSkillData(BinarySkill bskill, byte index, Entity playerHero)
	{
		var _cooldown = (ushort)bskill.cooldown._value(1);
		var em = ClientWorld.Instance.EntityManager;

		var database = default(EntityDatabase);

		//var _skill_entity = em.CreateEntity();
		//em.AddComponentData(_skill_entity, new SkillData
		//{
		//	db = bskill.index,
		//	index = index,
		//	minion = playerHero,
		//	level = 1,
		//	side = BattlePlayerSide.Left,
		//	battle = database.battle,
		//	source = database.index,
		//	type = bskill.type,
		//	cooldown = _cooldown,
		//	duration = bskill.duration,
		//	timer = 0
		//});

		//em.AddComponent<HeroSkill>(_skill_entity);
		/*
		if (_cooldown > 0)
		{
			buffer.AddComponent<SkillCDTag>(index, _skill_entity);
		}*/

		/*if (bskill.component > 0)
		{
			Components.Instance.Attach(bskill.component, bskill.index, index, buffer, _skill_entity);
		}*/
	}
}
