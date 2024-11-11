using Legacy.Database;
using Unity.Entities;


namespace Legacy.Client
{
	[UpdateInGroup(typeof(BattlePresentation))]
	[UpdateAfter(typeof(MinionTransformSystem))]

	public class ManaUpdateSystem : ComponentSystem
	{

		protected override void OnCreate()
		{
			RequireSingletonForUpdate<BattleInstance>();         
        }

		/*private TextMeshProUGUI ManaCountText;
		private Slider Fillin;
		private Slider ManaSlider;
		private ManaSliderBehaviour Modified;*/

        public static float PlayerMana;
        public static float ManaToUse;
		public static float ManaSelected;
		public static bool setImmediatelly;

        protected override void OnUpdate()
		{
			var _battle_instance = GetSingleton<BattleInstance>();
			if (_battle_instance.status == BattleInstanceStatus.Playing || _battle_instance.status == BattleInstanceStatus.Prepare || _battle_instance.status ==  BattleInstanceStatus.Pause)
			{
				PlayerMana = _battle_instance.players[_battle_instance.players.player].mana;

				//if (PlayerMana == 10)
				//{
				//	var em = ClientWorld.Instance.EntityManager;
				//	var entity = em.CreateEntity();
				//	em.AddComponentData<EventInstance>(entity, new EventInstance { trigger = TutorialEventTrigger.OnFullMana });
				//}
			}
			setImmediatelly = false;
		}
	}
}


