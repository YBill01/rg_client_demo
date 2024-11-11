using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Legacy.Database;
using Legacy.Client;
using Unity.Entities;
using System.Linq;
using System.Collections.Generic;

public class SandboxUnitEditorBehaviour : MonoBehaviour
{
	[SerializeField]
	private Text minionName;

	[SerializeField]
	private InputField level;
	[SerializeField]
	private InputField count;
	[SerializeField]
	private InputField hp;
    [SerializeField]
	private InputField aggro;
	[SerializeField]
	private InputField damage;
    [SerializeField]
    private InputField hit;
    [SerializeField]
    private InputField bulletSpeed;
    [SerializeField]
	private InputField damageDuration;
	[SerializeField]
	private InputField speed;
	[SerializeField]
	private InputField colliderSize;

	private ushort cardIndex;

	public static bool isOpen;

	public void OpenUnitStats(ushort cardIndex)
	{
		if (gameObject.activeSelf)
			return;

		isOpen = true;
		this.cardIndex = cardIndex;

		//LEVEL
		var query = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<SandboxPlayerDeck>());
		var deckComponent = query.GetSingleton<SandboxPlayerDeck>();
		var deck = deckComponent.getAll();
		var playerCard = deck.First(x => x.index == cardIndex);
		level.text = playerCard.level.ToString();

		//COUNT and NAME
		if (!Cards.Instance.Get(cardIndex, out BinaryCard card))
			Debug.LogError($"Cant get card {cardIndex}");
		
		count.text = card.entities.Count.ToString();
		minionName.text = card.title;

		//HP
		var defenceList = Components.Instance.Get<MinionDefence>();
		if (!defenceList.TryGetValue(card.entities[0], out MinionDefence defence))
			Debug.LogError($"Cant get defence for {card.entities[0]}");

		hp.text = defence.health.ToString();

		//DAMAGE
		var offenceList = Components.Instance.Get<MinionOffence>();
		if (!offenceList.TryGetValue(card.entities[0], out MinionOffence offence))
			Debug.LogError($"Cant get offence for {card.entities[0]}");

		damage.text = offence.damage.ToString();
		damageDuration.text = offence.duration.ToString();
        aggro.text = offence.aggro.ToString();
        hit.text = offence.hit.ToString();
        bulletSpeed.text = offence.bulletSpeed.ToString();

		//SPEED
		var movementList = Components.Instance.Get<MinionMovement>();
		if (!movementList.TryGetValue(card.entities[0], out MinionMovement movement))
			Debug.LogError($"Cant get movement for {card.entities[0]}");

		speed.text = movement.speed.ToString();

		//COLLIDER
		if (!Entities.Instance.Get(card.entities[0], out BinaryEntity minion))
			Debug.LogError($"Cant get minion {card.entities[0]}");		

		colliderSize.text = minion.collider.ToString();

		gameObject.SetActive(true);
	}

	public void Cancel()
	{
		gameObject.SetActive(false);
		isOpen = false;
	}

	public void Save()
	{
        if (!ReadByte(level.text, out byte levelValue))
        {
            Cancel(); return;
        }
		if (!ReadByte(count.text, out byte countValue))
        {
            Cancel(); return;
        }
        if (!ReadFloat(hp.text, out float hpValue))
        {
            Cancel(); return;
        }
        if (!ReadFloat(aggro.text, out float aggroValue))
        {
            Cancel(); return;
        }
        if (!ReadUShort(hit.text, out ushort hitValue))
        {
            Cancel(); return;
        }
        if (!ReadUShort(bulletSpeed.text, out ushort bulletSpeedValue))
        {
            Cancel(); return;
        }
        if (!ReadUShort(damage.text, out ushort damageValue))
        {
            Cancel(); return;
        }
        if (!ReadUShort(damageDuration.text, out ushort damageDurationValue))
        {
            Cancel(); return;
        }
        if (!ReadFloat(speed.text, out float speedValue))
        {
            Cancel(); return;
        }
        if (!ReadFloat(colliderSize.text, out float colliderSizeValue))
        {
            Cancel(); return;
        }

        //LEVEL
        var query = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<SandboxPlayerDeck>());
		var deckEntity = query.GetSingletonEntity();
		var deckComponent = query.GetSingleton<SandboxPlayerDeck>();
		var deck = deckComponent.getAll();

		var inDeckIndex = deck.FindIndex(x => x.index == cardIndex);
		var inDeckCard = deck[inDeckIndex];
		inDeckCard.level = levelValue;
		
		deckComponent._set(inDeckCard, (byte)inDeckIndex);
		ClientWorld.Instance.EntityManager.SetComponentData(deckEntity, deckComponent);

		Cards.Instance.Get(cardIndex, out BinaryCard card);
		var minionId = card.entities[0];

        //COUNT
        card.entities = new List<ushort>(countValue);
        for (int i = 0; i < countValue; ++i)
        {
            card.entities.Add(minionId);
        }

        //HP
        var defenceList = Components.Instance.Get<MinionDefence>();
		defenceList.TryGetValue(minionId, out MinionDefence defence);
		defence.health = hpValue;
		defenceList[minionId] = defence;

		Cards.Instance.Set(cardIndex, card);

		//обновили данные и карта тянет нужное количество юнитов
		//так мы избавиись от теней в песочнице
		SandboxCardsListBehaviour.Instance.cardsList[cardIndex - 1].UpdateCardData(cardIndex);

		ClientWorld.Instance.RequestForMinionInSandbox(minionId, hpValue, damageValue, damageDurationValue, aggroValue, hitValue, bulletSpeedValue, speedValue, colliderSizeValue, countValue, levelValue);
		Cancel();
	}

	private bool ReadByte(string text, out byte value)
	{
		if (!byte.TryParse(text, out value) || value == 0)
		{
			Debug.LogError($"Can not parse level {level.text}");
			return false;
		}
		return true;
	}

	private bool ReadUShort(string text, out ushort value)
	{
		if (!ushort.TryParse(text, out value))
		{
			Debug.LogError($"Can not parse {level.text}");
			return false;
		}
		return true;
	}

    private bool ReadFloat(string text, out float value)
	{
		if (!float.TryParse(text, out value) || value == 0)
		{
			Debug.LogError($"Can not parse level {level.text}");
			return false;
		}
		return true;
	}
}
