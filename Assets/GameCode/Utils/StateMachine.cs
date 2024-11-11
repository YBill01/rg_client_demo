using Legacy.Game;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Legacy.Client
{
    public class StateMachine<T>
	{
		public delegate void StateFunc();

		private UnityEvent _switchEvent = new UnityEvent();
		private UnityEvent _leaveEvent = new UnityEvent();

		public void Add(T id, StateFunc enter, StateFunc update, StateFunc leave)
		{
			m_States.Add(id, new State(id, enter, update, leave, null, null, null));
		}

		public void Add(T id, StateFunc enter, StateFunc update, StateFunc leave, StateFunc threadEnter, StateFunc threadUpdate, StateFunc threadLeave)
		{
			m_States.Add(id, new State(id, enter, update, leave, threadEnter, threadUpdate, threadLeave));
		}

		public void Add(T id, IState state)
		{
			m_States.Add(id, new State(id, state.OnEnter, state.OnUpdate, state.OnLeave, state.MonoThreadEnter, state.MonoThreadUpdate, state.MonoThreadLeave));
		}

		public T CurrentState { get => m_CurrentState.Id; }

		public bool StateInited { get => m_CurrentState != null; }
		public bool ConnectedToExistedBattle { get; set; }
		public UnityEvent SwitchEvent { get => _switchEvent; }
		public UnityEvent LeaveEvent { get => _leaveEvent; }

		public void Update()
		{
			m_CurrentState.Update();
		}

		public void ThreadEnter()
		{
			m_CurrentState.ThreadEnter();
		}

		public void ThreadUpdate()
		{
			m_CurrentState.ThreadUpdate();
		}

		public void ThreadLeave()
		{
			m_CurrentState.ThreadLeave();
		}

		public void Shutdown()
		{
			_leaveEvent.Invoke();
			if (m_CurrentState != null && m_CurrentState.Leave != null)
			{
				m_CurrentState.Leave();
			}
			m_CurrentState = null;
		}

		public void SwitchTo(T state)
		{
			GameDebug.Assert(m_States.ContainsKey(state), "Trying to switch to unknown state " + state.ToString());
			if(m_CurrentState == null || !m_CurrentState.Id.Equals(state))
			{
				GameDebug.Log("Catchin same state");
			}

			

			var newState = m_States[state];
			GameDebug.Log("Switching state: " + (m_CurrentState != null ? m_CurrentState.Id.ToString() : "null") + " -> " + state.ToString());

			if (m_CurrentState != null)
			{
				_leaveEvent.Invoke();
				if(m_CurrentState.Leave != null)
					m_CurrentState.Leave();
			}
			if (newState.Enter != null)
				newState.Enter();
			m_CurrentState = newState;
			_switchEvent.Invoke();
		}

		class State
		{
			public State(T id, StateFunc enter, StateFunc update, StateFunc leave, StateFunc threadEnter, StateFunc threadUpdate, StateFunc threadLeave)
			{
				Id = id;
				Enter = enter;
				Update = update;
				Leave = leave;
				ThreadEnter = threadEnter;
				ThreadUpdate = threadUpdate;
				ThreadLeave = threadLeave;
			}
			public T Id;
			public StateFunc Enter;
			public StateFunc Update;
			public StateFunc Leave;

			public StateFunc ThreadEnter;
			public StateFunc ThreadUpdate;
			public StateFunc ThreadLeave;
		}

		State m_CurrentState = null;
		Dictionary<T, State> m_States = new Dictionary<T, State>();
	}
}