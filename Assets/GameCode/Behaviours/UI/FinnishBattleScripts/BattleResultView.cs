using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultView : MonoBehaviour
{
	public int books;
	public int points;
	public int coins;

	[SerializeField]
	private RewardPointsViewBehaviour booksGO;
	[SerializeField]
	private RewardPointsViewBehaviour pointsGO;
	[SerializeField]
	private RewardPointsViewBehaviour coinsGO;

	[SerializeField]
	private RewardPointsViewBehaviour topParticles1;
	[SerializeField]
	private RewardPointsViewBehaviour topParticles2;
	[SerializeField]
	private RewardPointsViewBehaviour BottomParticles;

	public void NextReward()
	{
		if(books!= 0 && !booksGO.gameObject.active)
		{
			booksGO.value = books;
			booksGO.gameObject.SetActive(true);
			return;
		}
		if (points != 0 && !pointsGO.gameObject.active)
		{
			pointsGO.value = points;
			pointsGO.gameObject.SetActive(true);
			return;
		}
		if (coins != 0 && !coinsGO.gameObject.active)
		{
			coinsGO.value = coins;
			coinsGO.gameObject.SetActive(true);
			return;
		}
	}
}
