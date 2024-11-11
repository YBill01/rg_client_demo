using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Legacy.Client
{
	public class LegacyEditor
	{
		[MenuItem("GameObject/UI/LegacyButton", false, 0)]
		static void CreateLegacyButton(MenuCommand menuCommand)
		{
			GameObject ButtonContainer = new GameObject("LegacyButton");
			ButtonContainer.AddComponent<RectTransform>();
			ButtonContainer.AddComponent<LegacyButton>();

			ButtonContainer.AddComponent<ButtonPunch>();
			var bp = ButtonContainer.GetComponent<ButtonPunch>();
			bp.PunchPower = 0.1f;
			bp.PunchType = PunchType.Size;

			GameObject ButtonText = new GameObject("Text");

			ButtonText.AddComponent<TextMeshProUGUI>();
			var tmp = ButtonText.GetComponent<TextMeshProUGUI>();
			tmp.text = "LegacyButton";


			tmp.alignment = TextAlignmentOptions.Center;
			tmp.alignment = TextAlignmentOptions.CenterGeoAligned;

			tmp.raycastTarget = false;

			ButtonContainer.AddComponent<Image>();
			var img = ButtonContainer.GetComponent<Image>();

			ButtonContainer.GetComponent<LegacyButton>().targetGraphic = img;

			var parent = menuCommand.context as GameObject;

			ButtonText.GetComponent<RectTransform>().sizeDelta = ButtonContainer.GetComponent<RectTransform>().sizeDelta;

			GameObjectUtility.SetParentAndAlign(ButtonContainer, parent);
			GameObjectUtility.SetParentAndAlign(ButtonText, ButtonContainer);
			Undo.RegisterCreatedObjectUndo(ButtonContainer, "Create " + ButtonContainer.name);
			Selection.activeObject = ButtonContainer;
		}
	}
	
}

