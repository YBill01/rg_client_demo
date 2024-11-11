using Legacy.Client;
using Legacy.Database;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrossSceneContainer : MonoBehaviour
{
	[SerializeField] GameObject LunarPrefab;
	public static CrossSceneContainer instance;
	private bool show = true;
	void Awake()
	{
		if (!instance) {
			DontDestroyOnLoad(this);
			instance = this;
#if !PRODUCTION
			Instantiate(LunarPrefab, transform);
#endif
		}
		else
		{
			Destroy(this);
			show = false;
		}

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			if (BattleInstanceInterface.instance != null)
			{
				if (BattleInstanceInterface.instance.transform.GetComponentInChildren<TutorialMessageBehaviour>())
				{
					var tutormessages = BattleInstanceInterface.instance.transform.GetComponentsInChildren<TutorialMessageBehaviour>();
					foreach (var message in tutormessages)
					{
						GameObject.Destroy(message.gameObject);
					}
				}
				ClientWorld.Instance.EntityManager.DestroyEntity(ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<TutorialInstance>()));
				ClientWorld.Instance.EntityManager.DestroyEntity(ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<TutorialSnapshot>()));
			}
		}
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!show) return;
		var gs = scene.GetRootGameObjects();
		foreach (var g in gs)
		{
			if (g.GetComponent<CrossSceneContainer>())
			{
				DestroyImmediate(g);
			}
		}
		if (scene.name == "Home")
		{
			GetComponent<Canvas>().worldCamera = Camera.main;
		}
	}
}
