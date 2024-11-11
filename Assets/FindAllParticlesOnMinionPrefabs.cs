#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Legacy.Client;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine.AddressableAssets;

public class FindAllParticlesOnMinionPrefabs : MonoBehaviour
{
	[MenuItem("Legacy/ClearAddressablesCache")]
	static void ClearAddressablesCache()
	{
		Debug.LogWarning("ClearAddressablesCache");
		Addressables.ClearDependencyCacheAsync("server");
	}

	[MenuItem("Legacy/WorkWithMinions/AddSkill1AndSkill2")]
	static void AddSkill1AndSkill2()
    {
		Debug.LogWarning("Script FindAllParticlesOnMinionPrefabs start work");
        var prefabs = FindAllPrefabPaths();

		foreach (var path in prefabs)
        {			
			var prefab = LoadPrefab(path);
			UpdatePrefab(prefab);
			SavePrefab(prefab, path);
		}
    }

	static void UpdatePrefab(GameObject prefab)
	{
		Debug.LogWarning("Update prefab " + prefab.name);

		var animator = prefab.GetComponent<Animator>();
		if (animator.runtimeAnimatorController == null)
			return;

		var animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(animator.runtimeAnimatorController));
		if (animatorController.parameters.All(x => x.name != "Skill1"))
		{
			Debug.LogWarning("Update prefab Skill1");
			animatorController.AddParameter("Skill1", AnimatorControllerParameterType.Bool);
		}

		if (animatorController.parameters.All(x => x.name != "Skill2"))
		{
			Debug.LogWarning("Update prefab Skill2");
			animatorController.AddParameter("Skill2", AnimatorControllerParameterType.Bool);
		}



		//mib.allParticles = particles;
	}

	static GameObject LoadPrefab(string path)
	{
		Debug.LogWarning("Load prefab " + path);
		return PrefabUtility.LoadPrefabContents(path);
	}

	static void SavePrefab(GameObject prefab, string path)
	{
		Debug.LogWarning("Save prefab " + prefab.name);
		PrefabUtility.SaveAsPrefabAsset(prefab, path);
		PrefabUtility.UnloadPrefabContents(prefab);
	}

	static List<string> FindAllPrefabPaths()
	{
		var result = new List<string>();
		var minionsDir = new DirectoryInfo(Application.dataPath + "/GameResources/Prefabs/NewMinions/");
		var files = minionsDir.GetFiles("*.prefab");

		foreach (var file in files)
		{
			if (excludeList.Any(x => file.Name.Contains(x)))
				continue;

			result.Add("Assets/GameResources/Prefabs/NewMinions/" + file.Name);
		}

		return result;
	}

	private static List<string> excludeList = new List<string>
	{
		"BasicMinionPrefab",
		"Hero_",
		"shadow"
	};
}
#endif