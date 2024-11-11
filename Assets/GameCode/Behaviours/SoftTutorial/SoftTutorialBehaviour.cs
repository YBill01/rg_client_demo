using Legacy.Database;
using UnityEngine;

namespace Legacy.Client
{
	abstract class SoftTutorialBehaviour : MonoBehaviour
	{
		protected enum MainWindowPriority
		{ 
			EnterName = 0,
			UnlockLootboxFor8Cards = 2,
			OpenTutorialLootbox = 3,			
			UpgradeCard = 5,
			UpgradeHero = 6,
			UnlockTutorialLootbox = 10,
			StartBattle = 100
		}

		/// <summary>
		/// Какой туториал засчитать игроку после прохождения этого туториала.  
		/// Ожидаемыое значение (ushort)SoftTutorial.SoftTutorialState.{enum member}
		/// </summary>
		public virtual ushort TutorialState => 0;

		/// <summary>
		/// Приоритет в рамках одного окна (приоритеты тутора для окна карты и для окна героя не пересекаются). 
		/// Меньше значение - важнее туториал
		/// </summary>
		abstract public int Priority { get; }

		/// <summary>
		/// При очередной проверке возможности запуска софт туториала, может оказаться что запуститься может только туториал который и так работает
		/// В таком случае его запуск игнорируется
		/// Но если эта переменная возвращает True - такой софттуториал нужно перезапустить
		/// </summary>
		public virtual bool CanBeRestarted => false;

		/// <summary>
		/// Готов ли этот туориал к показу, выполнены ли нужные требования для его показа
		/// </summary>
		/// <returns> Прошел ли туториал свои проверки - будем ли мы его показывать </returns>
		abstract public bool CanStartTutorial();

		/// <summary>
		/// Запускает показ туториала
		/// </summary>
		abstract public void StartTutorial();

		protected ProfileInstance profile;

		private void Awake()
		{
			profile = ClientWorld.Instance.Profile;
		}

		/// <summary>
		/// Метод вызывается когда начинает показываться новый туториал, а этот еще  не завершен
		/// </summary>
		public virtual void StopTutorial()
		{
		}
	}
}
