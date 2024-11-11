using UnityEngine;
using System.Collections.Generic;
using Legacy.Database;
using DG.Tweening;
using Legacy.Client;

public class SkillsPositionsBehaviour : MonoBehaviour
{
	[SerializeField]
	RectTransform skill1;
	[SerializeField]
	RectTransform skill2;
	[SerializeField]
	BattleSkillDragBehaviour dragSkill1;
	[SerializeField]
	BattleSkillDragBehaviour dragSkill2;
	/*[SerializeField]
	RectTransform rage;*/
	private enum State
	{
		NotReady,
		FirstSkillReady,
		SecondSkillReady,
		BoothReady
	}

	private State state;

	private bool inited;
	private Vector2 startPosSkill1;
	private Vector2 startPosSkill2;
	//private Vector2 startPosRage;

	private const float skillDelta = 15f; // Когда скилл становится скейлом из 0,9 в 1,0 он становиться шире на skillDelta * 2
	private const float rageDelta = 12f; // Когда рейдж становится скейлом из 0,9 в 1,0 он становиться шире на rageDelta * 2

	// Смещения кнопок скилов и рейджа в зависимости от состояния скилов
	private Dictionary<State, float[]> offset = new Dictionary<State, float[]>
	{
		{ State.NotReady, new float[] { 0, 0, 0 } },
		{ State.FirstSkillReady, new float[] { skillDelta, 0, 0 } },
		{ State.SecondSkillReady, new float[] { 2 * skillDelta, skillDelta, 0 } },
		{ State.BoothReady, new float[] { 3 * skillDelta, skillDelta, 0 } }
	};

	// Вызывается в конце анимаций ShowSkills и ShowSkillsWithoutRage. Так мы получаем позиции скилов такие же как и в анимации
	public void Init()
	{
		inited = true;

		startPosSkill1 = skill1.anchoredPosition;
		startPosSkill2 = skill2.anchoredPosition;
		//startPosRage = rage.anchoredPosition;

		//dragSkill1.SetStartPos();
		//dragSkill2.SetStartPos();

		//settings = Settings.Instance.Get<HeroRageSettings>();
	}

	public void UpdateSkillsPositions(int skill1Timer, int skill2Timer)
	{
		if (!inited)
			return;

		State currentState;

		if (skill1Timer == 0 && skill2Timer == 0)
			currentState = State.BoothReady;
		else if (skill1Timer == 0)
			currentState = State.FirstSkillReady;
		else if (skill2Timer == 0)
			currentState = State.SecondSkillReady;
		else
			currentState = State.NotReady;

		if (state == currentState)
			return;

		state = currentState;

		//var newPosSkill1 = startPosSkill1.x - offset[state][0];
		//var newPosSkill2 = startPosSkill2.x - offset[state][1];
		//var newPosRage = startPosRage.x - offset[state][2];

		//if (!dragSkill1.IsDraged)
		//	skill1.DOAnchorPosX(newPosSkill1, 0.3f).SetEase(Ease.InOutBack).SetDelay(0.1f);
		//if (!dragSkill2.IsDraged)
		//	skill2.DOAnchorPosX(newPosSkill2, 0.3f).SetEase(Ease.InOutBack).SetDelay(0.1f);


		//rage.DOAnchorPosX(newPosRage, 0.3f).SetEase(Ease.InOutBack).SetDelay(0.1f);
	}
}