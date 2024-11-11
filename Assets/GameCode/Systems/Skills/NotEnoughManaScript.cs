using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughManaScript : MonoBehaviour
{
    
	public BattleCardBehaviour CardBehaviour;
	public Image Fader;
	private ManaUpdateSystem manaSystem;
    public float amount;

    private EntityQuery battleInstanceQuery;

	private void Start()
	{
        battleInstanceQuery = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());

    }

	// Update is called once per frame
	void Update()
    {
		if (!battleInstanceQuery.IsEmptyIgnoreFilter)
		{
			var _battle = battleInstanceQuery.GetSingleton<BattleInstance>();

            if (_battle.status == BattleInstanceStatus.Prepare)
            {
                CardBehaviour.Active(false);
                return;
            }
		}


        /*if (!CardBehaviour.isNext && CardBehaviour.hand.inited)
        {
            if (CardBehaviour.DBCardData.manaCost > 0)
            {
                amount = 1 - Mathf.Clamp01((float)ManaUpdateSystem.PlayerMana / CardBehaviour.DBCardData.manaCost);
            }
            else
            {
                amount = 1;
            }

            CardBehaviour.Active(amount > 0);
            Fader.fillAmount = amount;
        }*/
    }
}
