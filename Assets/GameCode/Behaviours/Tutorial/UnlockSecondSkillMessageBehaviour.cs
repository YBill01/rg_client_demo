using UnityEngine;

namespace Legacy.Client
{
	public class UnlockSecondSkillMessageBehaviour : MonoBehaviour
	{
		private void Start()
		{
			BattleInstanceInterface.instance.Skill2.IsBlockedByTutorial = false;
		}
	}
}