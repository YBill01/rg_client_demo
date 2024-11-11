/*using UnityEngine;
using Legacy.Client;
using Unity.Entities;
using Legacy.Database;

public abstract class BaseMenuTutorialExtention : MonoBehaviour
{
	// Возвращает true если сообщение полностью обработано и дальнейших действий не нужно
	public abstract bool ProcessMessage(string message, ref RectTransform buttonForPointer);
	
	protected virtual void StopOtherTutorials()
	{
		//var em = ClientWorld.Instance.EntityManager;
		//var queryTutorial = em.CreateEntityQuery(ComponentType.ReadOnly<MenuTutorialInstance>());
		//if (!queryTutorial.IsEmptyIgnoreFilter)
		//{
		//	em.DestroyEntity(queryTutorial.GetSingletonEntity());
		//}
	}
}
*/