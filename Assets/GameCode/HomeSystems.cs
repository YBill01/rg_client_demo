using Legacy.Database;
using System;
using System.Timers;
using Unity.Entities;

namespace Legacy.Client
{
    [UpdateInGroup(typeof(HomeSystems))]
	[UpdateBefore(typeof(MenuSystemsGroup))]
	public class TechnicalSystemGroup : ComponentSystemGroup { }

	[UpdateInGroup(typeof(HomeSystems))]
	public class MenuSystemsGroup : ComponentSystemGroup { }

	[UpdateAfter(typeof(MenuSystemsGroup))]
	[UpdateInGroup(typeof(HomeSystems))]
	public class GameStockSystemsGroup : ComponentSystemGroup { }

	[UpdateAfter(typeof(MenuSystemsGroup))]
	[UpdateInGroup(typeof(HomeSystems))]
	public class CardsCollectionSystemsGroup : ComponentSystemGroup { }


	[AlwaysUpdateSystem]
	public class HomeSystems : ComponentSystemGroup
	{
		private InputSystem _input;
		private bool _active = false;
		//private EntityQuery _errors;
		private Timer updateTimer;

		protected override void OnCreate()
		{
			base.OnCreate();
			/*_errors = GetEntityQuery(
					ComponentType.ReadOnly<UserErrorData>()
				);*/
			updateTimer = new Timer();
			updateTimer.Interval = 1000;
			updateTimer.Start();
		}

		private static TimeSpan _observerTimeDelta;
		private static DateTime _observerTime;
		public static DateTime ObserverTime { get => _observerTime; }
		public static TimeSpan ObserverTimeDelta { get => _observerTimeDelta; }
		public static void SetupOBserverTime(long ticks)
		{
			_observerTime = new DateTime().AddTicks(ticks);
			_observerTimeDelta = new TimeSpan(ticks - DateTime.Now.Ticks);
		}

		private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			//_userProfile.LootBoxList.UpdateTime();
		}

		private bool error_state;
		protected override void OnUpdate()
		{
			if (_active)
			{
				/*if(!_errors.IsEmptyIgnoreFilter && !error_state)
				{
					error_state = true;
					SetupErrorInput();
				}
				if(_errors.IsEmptyIgnoreFilter && error_state)
				{
					SetupActiveInput();
					error_state = false;
				}*/
				base.OnUpdate();
				_input.Update();
			}
		}

		public void Initialization(bool value)
		{
			_active = value;
			EntityManager.CompleteAllJobs();
			if (value)
			{
				SetInputEnabled(true);
				updateTimer.Elapsed += UpdateTimer_Elapsed;
				updateTimer.Start();
			}
			else
			{
				SetInputEnabled(false);
				updateTimer.Elapsed -= UpdateTimer_Elapsed;
				updateTimer.Stop();
			}
		}

		public void Simulation(bool value)
		{
			_active = value;
			EntityManager.CompleteAllJobs();
			
			SetInputEnabled(value);
			SetupProfile(value);
		}

		private void SetInputEnabled(bool State)
		{
			if (State)
			{
				SetupActiveInput();
			}
			else
			{
				if(_input != null)
				{
					_input.Enabled = false;
					_input = null;
				}
			}
		}

		private void SetupActiveInput()
		{
			_input = World.GetOrCreateSystem<InputSystem>();
			_input.UITag = "HomeUI";
			_input.UIName = "ProfileInterface";

			_input.Clear();

			//SwipeMenuExecutor SME = new SwipeMenuExecutor();
			//CollectionCardTouchExecutor CCTE = new CollectionCardTouchExecutor();
			//SME.Swipe_target_name = "ContentContainer";
			//SME.MenuID = 1;
			//_input.AddInteractor(SME, 0);
			//_input.AddInteractor<ItemRotateExecutor>(0);
			//_input.AddInteractor<MenuBlockExecutor>(0);
			//_input.AddInteractor(CCTE, 0);
			//_input.AddInteractor<AcceleratedSwipeExecutor>(0).orientation = Orientation.Horizontal;
			//_input.AddInteractor<AcceleratedSwipeExecutor>(0);
			//_input.AddInteractor<MenuClickExecutor>(0);
			//_input.AddInteractor<CollectionCardClickExecutor>(0);
			//_input.AddInteractor<CollectionChosenTouchExecutor>(0);
			//_input.AddInteractor<CollectionCardPretouchExecutor>(1);
			_input.Enabled = true;
		}

		private void SetupErrorInput()
		{
			_input = World.GetOrCreateSystem<InputSystem>();
			_input.UITag = "HomeUI";
			_input.UIName = "ProfileInterface";

			_input.Enabled = true;
			_input.Clear();
		}

		private ProfileInstance _userProfile;
		private PlayerProfileInstance _profile;
		public PlayerProfileInstance DefaultProfile
		{
			get
			{
				if (_profile == null)
				{
					_profile = new PlayerProfileInstance();
					_profile.SetDefault();
				}
				return _profile;
			}
			set
			{
				_profile = value;
			}
		}

		public ProfileInstance UserProfile { get => _userProfile; set => _userProfile = value; }

		private void SetupProfile(bool State)
		{
			if (_userProfile != null) return;
			_userProfile = new ProfileInstance()
			{ 
				playerSettings = new PlayerProfileSettings()
			};
		}
	}
}
