using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NotDraggableScrollRect : ScrollRect
{
	public override void OnDrag(PointerEventData eventData)
	{ }
}
