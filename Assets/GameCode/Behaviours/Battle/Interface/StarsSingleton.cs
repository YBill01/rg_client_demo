using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsSingleton
{
    public int PlayerStarsCount
    {
        get
        {
            return playerStarsCount;
        }
        set
        {
            playerStarsCount = value;
            if (value < 0)
                playerStarsCount = 0;
        }
    }

    public bool IsVictory => PlayerStarsCount > EnemyStarsCount;
    public int EnemyStarsCount
    {
        get
        {
            return enemyStarsCount;
        }
        set
        {
            enemyStarsCount = value;
            if (value < 0)
                enemyStarsCount = 0;
        }
    }
    private static StarsSingleton _instance;
    private static int enemyStarsCount;
    private static int playerStarsCount;


    public static StarsSingleton Instance
    {
        get
        {
            if (_instance == null)
                _instance = new StarsSingleton();
            return _instance;

        }
        set
        {
            _instance = value;
        }
    }

    public static int GetStars(bool isEnemy)
    {
        if (!isEnemy) return enemyStarsCount;
        else return playerStarsCount;
    }

    public void SetStarsAndFlags(int starsCount, bool isEnemy)
    {
        if (isEnemy) EnemyStarsCount = starsCount;
        else PlayerStarsCount = starsCount;


        var flags = StaticColliders.instance.GetFlags(!isEnemy);
        flags.DisableFlag(starsCount);

        var starView = isEnemy ? BattleInstanceInterface.instance.EnemyStars : BattleInstanceInterface.instance.PlayerStars;
        starView.SetStars(starsCount, isEnemy);

    }

}
