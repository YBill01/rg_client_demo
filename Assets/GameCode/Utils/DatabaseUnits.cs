using System.Collections.Generic;

namespace Legacy.Client
{

    public enum DatabaseEntityType
	{
		Entity		= 5,
		Squad		= 4,
		Effect		= 3,
		Hero		= 2,
		Minion		= 1,

		Other		= 0
	}

	public interface IDatabase
	{
		
	}

	public struct DatabaseEntity
	{
		public uint ID;
		public string Prefab;
		public DatabaseEntityType Type;
		public List<IDatabase> Components;
	}

	public class GameDataBase
	{
        /*public DatabaseUnits()
		{
			_entities = new Dictionary<uint, DatabaseEntity>();

			_entities.Add(1, new DatabaseEntity
			{
				Type = DatabaseEntityType.Entity,
				Prefab = "Prefabs/Interface/BattleInterface",
				Components = new List<IDatabase>()
				{

				}
			});

			_entities.Add(2, new DatabaseEntity
			{
				Prefab = "Prefabs/Minions/m1",
				Type = DatabaseEntityType.Minion,
				Components = new List<IDatabase>()
				{
					new MinionData
					{
						Health = 30,
						Collider = 0.6f
					},
					new AttackData
					{
						Type = AttackType.Mellee,
						AggroRadius = 1f,
						AttackDamage = 9,
						AttackDuration = 2.2f,
						AttackRadius = 0.7f,
						HittingTime = 0.5f,
						AttackCharge = 2.3f
					},
					new MovementData
					{
						Speed = 0.6f
					}
				}
			});

			_entities.Add(3, new DatabaseEntity
			{
				Prefab = "Prefabs/Minions/m2",
				Type = DatabaseEntityType.Minion,
				Components = new List<IDatabase>()
				{
					new MinionData
					{
						Health = 30,
						Collider = 0.6f
					},
					new AttackData
					{
						Type = AttackType.Mellee,
						AggroRadius = 1f,
						AttackDamage = 9,
						AttackDuration = 2.2f,
						AttackCharge = 2.3f,
						AttackRadius = 0.7f,
						HittingTime = 0.5f,
					},
					new MovementData
					{
						Speed = 0.6f
					}
				}
			});

			// hero1
			_entities.Add(4, new DatabaseEntity
			{
				Prefab = "Prefabs/Heroes/hero1",
				Type = DatabaseEntityType.Hero,
				Components = new List<IDatabase>()
				{
					new MinionData
					{
						Health = 100,
						Collider = 1f
					},
					new AttackData
					{
						Type = AttackType.Range,
						AggroRadius = 6,
						AttackDamage = 20,
						AttackDuration = 3f,
						AttackRadius = 6,
						HittingTime = 0.1f,
						BulletSpeed = 20f,
						BulletType = BulletType.Bullet
					}					
				}
			});

			// hero2
			_entities.Add(5, new DatabaseEntity
			{
				Prefab = "Prefabs/Heroes/hero2",
				Type = DatabaseEntityType.Hero,
				Components = new List<IDatabase>()
				{
					new MinionData
					{
						Health = 100,
						Collider = 2f
					},
					new AttackData
					{
						Type = AttackType.Range,
						AggroRadius = 6,
						AttackDamage = 20,
						AttackDuration = 3f,
						AttackRadius = 6,
						HittingTime = 0.1f,
						BulletSpeed = 20f,
						BulletType = BulletType.Bullet
					}
				}
			});
        */
        public static DatabaseEntity GetBattleInstance()
        {
            return new DatabaseEntity
            {
                Type = DatabaseEntityType.Entity,
                Prefab = "Prefabs/Interface/BattleInterface",
                Components = new List<IDatabase>()
                {

                }
            };
        }        
	}


}