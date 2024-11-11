using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

public class FinishBattleBehaviour : MonoBehaviour
{
	public FinishBattleBehaviour()
	{
		ChangeDataEvent = new UnityEvent();
	}

	public int result;
	public int leftStars;
	public int rightStars;

	public int books = 0;
	public int points = 0;
	public int coins = 0;

	public BattleResultView draw;
	public BattleResultView win;
	public BattleResultView loose;

	public Animator leftStarsAnimator;
	public Animator righttarsAnimator;

	public Animator drawStarsAnimator;
	public Animator winStarsAnimator;
	public Animator looseStarsAnimator;

	public Animator drawAnimator;
	public Animator winAnimator;
	public Animator looseAnimator;

	public void SetupState(BattlePlayerSide winnerSide, BattlePlayerSide mySide)
	{
		if (winnerSide != BattlePlayerSide.None)
		{
			result = (int)(winnerSide == mySide ? 1 : 2);
		}
		StateSet = true;
		ChangeDataEvent.Invoke();
	}

	public bool StateSet { get; private set; }
	public bool RewardSet { get; private set; }
	public UnityEvent ChangeDataEvent { get; private set; }
	public void SetupReward(BattleRatingResultReward reward)
	{
		Debug.Log("RWD is " + reward.rating.ToString() + "  " + reward.soft.ToString());
		points = (int)reward.rating;
		coins = reward.soft;
		RewardSet = true;
		ChangeDataEvent.Invoke();
	}

	public void SetupData()
	{
		leftStarsAnimator.SetInteger("StarsCount", leftStars);
		righttarsAnimator.SetInteger("StarsCount", rightStars);

		switch (result)
		{
			case 0:
				draw.books = books;
				draw.points = points;
				draw.coins = coins;
				draw.gameObject.SetActive(true);
				drawStarsAnimator.SetInteger("StarsCount", leftStars);
				break;
			case 1:
				win.books = books;
				win.points = points;
				win.coins = coins;
				win.gameObject.SetActive(true);
				winStarsAnimator.SetInteger("StarsCount", leftStars);
				break;
			case 2:
				loose.books = books;
				loose.points = points;
				loose.coins = coins;
				loose.gameObject.SetActive(true);
				looseStarsAnimator.SetInteger("StarsCount", leftStars);
				break;
		}
	}

	private StateMachineSystem stateMachineSystem;
	public void GoHome()
	{
		stateMachineSystem = ClientWorld.Instance.GetExistingSystem<StateMachineSystem>();
		stateMachineSystem.ForceExitBattle();
	}
}
