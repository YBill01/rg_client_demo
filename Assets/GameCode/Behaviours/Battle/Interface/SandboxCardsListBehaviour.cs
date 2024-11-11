using UnityEngine;
using Legacy.Database;
using System.Collections;

namespace Legacy.Client {
	public class SandboxCardsListBehaviour : MonoBehaviour
	{
		public static SandboxCardsListBehaviour Instance;

		public BattleCardDragBehaviour[] cardsList;
		[SerializeField]
		GameObject SandboxCardPrefab;

		[SerializeField]
		SandboxUnitEditorBehaviour unitEditor;

		void Awake()
		{
			Instance = this;
			cardsList = new BattleCardDragBehaviour[Cards.Instance.List.Length];
			var root = transform.root;
			byte i = 0;

			var rect = GetComponent<RectTransform>();

			foreach (var card in Cards.Instance.List)
			{
				var cardObject = Instantiate(SandboxCardPrefab, transform);
				var cardBehaviour = cardObject.GetComponentInChildren<BattleCardDragBehaviour>();

				var cardRect = cardObject.GetComponent<RectTransform>();
				cardRect.localPosition = new Vector2(cardRect.rect.size.x / 2f + i * 200, -cardRect.rect.size.y / 5);

				cardBehaviour.nextCard = rect;
				cardBehaviour.IndexInHand = i;
				cardBehaviour.Init();
				cardBehaviour.UpdateCardData(card.index);
				cardBehaviour.Unhide();

				cardObject.GetComponent<BattleCardViewBehaviour>().SetGray(false);

				cardBehaviour.onRightClick.AddListener(unitEditor.OpenUnitStats);

				cardsList[i] = cardBehaviour;
				i++;
			}

			var size = rect.sizeDelta;
			size.x = SandboxCardPrefab.GetComponent<RectTransform>().sizeDelta.x * Cards.Instance.List.Length + 1000;
			rect.sizeDelta = size;
		}

        private void Start()
        {
			StartCoroutine(DelayedHidePanels());
        }
		private IEnumerator DelayedHidePanels()
        {
			yield return new WaitForSeconds(3);
		//	GameObject.Find("BattleInterface").GetComponent<BattleInstanceInterface>().HidePanels();
		}
    }
}
