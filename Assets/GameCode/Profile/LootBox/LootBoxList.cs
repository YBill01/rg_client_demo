using Legacy.Database;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
/*
using Legacy.Client;

public class LootBoxModelEvent : UnityEvent<LootBoxModel> { };
public class LootBoxModelTimeEvent : UnityEvent<LootBoxModel, TimeSpan> { };
public class LootBoxModel
{
	private byte _id;
	private List<LootBoxModel> _queue = new List<LootBoxModel>();
	private BinaryLootType _type;
	private DateTime _startOpenTime;
	private LootBoxSlotState _state;
	private bool _isBlocked;
	private LootBoxModelEvent _receiveBoxEvent = new LootBoxModelEvent();
	private LootBoxModelTimeEvent _skipEvent = new LootBoxModelTimeEvent();
	private LootBoxModelEvent _userAction = new LootBoxModelEvent();
	private LootBoxModelEvent _openEvent = new LootBoxModelEvent();
	private LootBoxModelEvent _queueEvent = new LootBoxModelEvent();
	private LootBoxModelEvent _blockEvent = new LootBoxModelEvent();
	private LootBoxModelEvent _unblockEvent = new LootBoxModelEvent();
	private UnityEvent _timerEvent = new UnityEvent();
	private LootBoxModelEvent _readyEvent = new LootBoxModelEvent();
	private LootBoxModelEvent _timerStartEvent = new LootBoxModelEvent();
	private LootBoxModelTimeEvent _timeUpdateEvent = new LootBoxModelTimeEvent();

	private StateMachine<LootBoxSlotState> _lootStateMachine = new StateMachine<LootBoxSlotState>();
	public LootBoxModel(byte id, BinaryLootType type, DateTime startOpenTime)
	{
		this._id = id;
		this._type = type;
		this._startOpenTime = startOpenTime;

		_lootStateMachine.Add(LootBoxSlotState.Empty, new LootBoxEmptyState(this));
		_lootStateMachine.Add(LootBoxSlotState.Default, new LootBoxDefaultState(this));
		_lootStateMachine.Add(LootBoxSlotState.WaitQueue, new LootBoxQueueState(this));
		_lootStateMachine.Add(LootBoxSlotState.Working, new LootBoxWorkingState(this));
		_lootStateMachine.Add(LootBoxSlotState.Ready, new LootBoxReadyState(this));

		if(type != BinaryLootType.None)
		{
			_lootStateMachine.SwitchTo(LootBoxSlotState.Default);
		}
		else
		{
			_lootStateMachine.SwitchTo(LootBoxSlotState.Empty);
		}

		updateOpenTime();
	}

	public void SetQueue(List<LootBoxModel> queue)
	{
		_queue = queue;
	}

	public void SetBox(BinaryLootType type)
	{
		this._type = type;
		//_receiveBoxEvent.Invoke(this);
		//_state = LootBoxSlotState.Default;
		_lootStateMachine.SwitchTo(LootBoxSlotState.Default);
		_receiveBoxEvent.Invoke(this);
		updateOpenTime();

	}

	private void updateOpenTime()
	{
		OpenTime = new TimeSpan(0, 0, 60);
		if (Loots.Instance.Get(_type, out BinaryLoot binary))
		{
			OpenTime = new TimeSpan(0, 0, binary.time);
		}
	}


	public void Block()
	{
		if(_isBlocked)
			return;
		_blockEvent.Invoke(this);
	}

	public void UnBlock()
	{
		if (!_isBlocked)
			return;
		_unblockEvent.Invoke(this);
	}

	public void SwitchState(LootBoxSlotState state)
	{
		_lootStateMachine.SwitchTo(state);
	}

	public TimeSpan OpenTime { get; private set; }

	public byte Id { get => _id; }
	public BinaryLootType Sid { get => _type; }
	public UnityEvent TimerEvent { get => _timerEvent; }
	//public LootBoxSlotState State { get => _state; }
	public LootBoxSlotState State { get => _lootStateMachine.CurrentState; }
	public LootBoxModelTimeEvent TimeUpdateEvent { get => _timeUpdateEvent; }
	public bool IsBlocked { get => _isBlocked; }
	public BinaryLootType Type { get => _type; }
	public LootBoxModelEvent BlockEvent { get => _blockEvent; }
	public LootBoxModelEvent UnblockEvent { get => _unblockEvent; }
	public LootBoxModelEvent ReadyEvent { get => _readyEvent; }
	public LootBoxModelEvent TimerStartEvent { get => _timerStartEvent; }
	public LootBoxModelEvent OpenEvent { get => _openEvent; }
	public LootBoxModelEvent QueueEvent { get => _queueEvent; }
	public LootBoxModelEvent ReceiveBoxEvent { get => _receiveBoxEvent; }
	public LootBoxModelTimeEvent SkipEvent { get => _skipEvent; }
	public LootBoxModelEvent UserAction { get => _userAction; }
	public UnityEvent SwitchEvent { get => _lootStateMachine.SwitchEvent; }
	public LootBoxSlotState CurrentState { get => _lootStateMachine.CurrentState; }
	public DateTime StartOpenTime { get => _startOpenTime; }
	public List<LootBoxModel> Queue { get => _queue; }

	public void ChangeStartTime(TimeSpan time)
	{
		_startOpenTime = _startOpenTime.AddTicks(time.Ticks);
	}

	public void SetStartTime(DateTime dateTime)
	{
		_startOpenTime = dateTime;
	}
}

public enum LootBoxSlotState
{
	Working,
	Ready,
	WaitQueue,
	Default,
	Empty,
}

public class LootBoxList
{
	private List<LootBoxModel> _lootBoxes = new List<LootBoxModel>();
	private byte maxQueue;
	private DateTime _startOpenTime;
	private List<byte> queue = new List<byte>();
	public LootBoxList(PlayerProfileLoots loot)
	{
		Rebuild(loot);
	}

	private void OnBoxReady(LootBoxModel lootBoxModel)
	{
		lootBoxModel.SkipEvent.RemoveListener(OnBoxSkip);
		lootBoxModel.ReadyEvent.RemoveListener(OnBoxReady);
		lootBoxModel.OpenEvent.AddListener(OnBoxOpen);

		lootBoxModel.SwitchState(LootBoxSlotState.Ready);

		_startOpenTime = _startOpenTime.AddTicks(lootBoxModel.OpenTime.Ticks);
		//_startOpenTime = DateTime.Now;

		queue.Remove(lootBoxModel.Id);

		UnblockExcess();
		MoveQueue();
		ResetQueue();
		ResetTime();
	}

	private void ResetQueue()
	{
		var qList = new List<LootBoxModel>();
		foreach(var m in _lootBoxes)
		{
			foreach(var qID in queue)
			{
				if(m.Id == qID)
				{
					qList.Add(m);
					break;
				}
			}
		}
		foreach (var lb in _lootBoxes)
		{
			lb.SetQueue(qList);
		}
	}

	private void UnblockExcess()
	{
		foreach (var lootBox in _lootBoxes)
		{
			if (lootBox.State == LootBoxSlotState.Default)
				lootBox.UnBlock();
		}
	}

	private void OnBoxOpen(LootBoxModel lootBoxModel)
	{
		lootBoxModel.OpenEvent.RemoveListener(OnBoxOpen);
		lootBoxModel.SwitchState(LootBoxSlotState.Empty);
		if(queue.Contains(lootBoxModel.Id))
		{
			queue.Remove(lootBoxModel.Id);
			_startOpenTime = _startOpenTime.AddTicks(lootBoxModel.OpenTime.Ticks);
		}
		ResetQueue();
		ResetTime();
	}

	private void OnBoxSkip(LootBoxModel lootBoxModel, TimeSpan leftTime)
	{
		//if (Loots.Instance.Get(lootBoxModel.Sid, out BinaryLoot binary))
		//{
		//	//binary.time
		//	TimeSpan openTime = 
		//	long lostTicks = leftTim.Ticks;
		//}
		//long lostTicks = lootBoxModel.OpenTime.Subtract(leftTime).Ticks;
		long lostTicks = leftTime.Ticks;
		Debug.Log("Skipped time " + LegacyHelpers.BeautifullTimeText(leftTime));
		Debug.Log("Passed lootbox time " + LegacyHelpers.BeautifullTimeText(lootBoxModel.OpenTime - leftTime));
		lootBoxModel.SwitchState(LootBoxSlotState.Empty);

		lootBoxModel.SkipEvent.RemoveListener(OnBoxSkip);
		lootBoxModel.ReadyEvent.RemoveListener(OnBoxReady);
		lootBoxModel.OpenEvent.AddListener(OnBoxOpen);

		if (queue.Contains(lootBoxModel.Id))
		{
			queue.Remove(lootBoxModel.Id);
			//_startOpenTime = _startOpenTime.AddTicks(-lostTicks);
			_startOpenTime = _startOpenTime.AddTicks((lootBoxModel.OpenTime - leftTime).Ticks);
		}

		UnblockExcess();
		MoveQueue();
		ResetQueue();
		ResetTime();
	}

	private void MoveQueue()
	{
		if (queue.Count == 0) return;

		var id = queue[0];
		var lb = _lootBoxes[id];
		lb.SwitchState(LootBoxSlotState.Working);
		lb.SkipEvent.AddListener(OnBoxSkip);
		lb.ReadyEvent.AddListener(OnBoxReady);
	}

	public void AddBox(byte id, BinaryLootType type)
	{
		_lootBoxes[id].SetBox(type);
	}

	public void UpdateTime()
	{
		foreach(var lb in _lootBoxes)
		{
			lb.TimerEvent.Invoke();
		}
	}

	public void Rebuild(PlayerProfileLoots loot)
	{
		/*maxQueue = PlayerProfileLoots.max_queue;
		//if (loot.boxes.Count != 0)
		//{
		//	loot.boxes = new List<BinaryLootType>() { BinaryLootType.Normal, BinaryLootType.None, BinaryLootType.Expensive, BinaryLootType.Normal };
		//	queue = new List<byte>() { 0, 2 };
		//}
		//_startOpenTime = loot.timer + HomeSystems.ObserverTimeDelta;

		//_startOpenTime = loot.timer + HomeSystems.ObserverTimeDelta;
		queue = loot.queue;
		if (queue.Count == 0)
		{
			_startOpenTime = DateTime.Now;
		}
		else
		{
			_startOpenTime = loot.timer.ToLocalTime();
		}
		for (int i = 0; i < loot.boxes.Count; i++)
		{
			var lb = new LootBoxModel((byte)i, loot.boxes[i], _startOpenTime);
			_lootBoxes.Add(lb);
		}
		for (int i = 0; i < queue.Count; i++)
		{
			var id = queue[i];
			var lb = _lootBoxes[id];
			if (i == 0)
			{
				lb.SwitchState(LootBoxSlotState.Working);
				lb.ReadyEvent.AddListener(OnBoxReady);
				lb.SkipEvent.AddListener(OnBoxSkip);
				continue;
			}
			lb.SwitchState(LootBoxSlotState.WaitQueue);
		}
		for (int i = 0; i < loot.boxes.Count; i++)
		{
			var lb = _lootBoxes[i];
			if (lb.State == LootBoxSlotState.Default)
			{
				lb.QueueEvent.AddListener(OnQueue);
			}
		}
		ResetQueue();
		ResetTime();
		if (loot.queue.Count < maxQueue) return;

		foreach (var lb in _lootBoxes)
		{
			if (lb.State == LootBoxSlotState.Default)
				lb.Block();
		}
	}

	private void OnQueue(LootBoxModel lootBoxModel)
	{
		lootBoxModel.QueueEvent.RemoveListener(OnQueue);
		if (queue.Count == 0)
		{
			_startOpenTime = DateTime.Now;
			ResetTime();
		}
		queue.Add(lootBoxModel.Id);

		ResetQueue();

		foreach(var lb in _lootBoxes)
		{
			if (lb == lootBoxModel) continue;
			if(lb.State == LootBoxSlotState.Working)
			{
				lootBoxModel.SwitchState(LootBoxSlotState.WaitQueue);
				return;
			}
		}
		lootBoxModel.SwitchState(LootBoxSlotState.Working);
		lootBoxModel.SkipEvent.AddListener(OnBoxSkip);
		lootBoxModel.ReadyEvent.AddListener(OnBoxReady);
	}

	private void ResetTime()
	{
		foreach (var lbm in _lootBoxes)
		{
			lbm.SetStartTime(_startOpenTime);
		}
	}

	public List<LootBoxModel> LootBoxes { get => _lootBoxes; }
	public DateTime StartOpenTime { get => _startOpenTime; }
}
*/
