using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrefabsEditScript : MonoBehaviour
{

}

#if UNITY_EDITOR
public class PrefabsReassebler
{
    static string sourcePath = "Assets/GameResources/Minions";
    static string basicPrefabsPath = "Assets/GameResources/Prefabs/Minions/BasicMinionPrefab.prefab";
    static string prefabsPath = "Assets/GameResources/Prefabs/Minions";
    static string newPrefabsPath = "Assets/GameResources/Prefabs/NewMinions";

    static string[] failedNames = 
        new string[]
            {
                "Hero_Irva",
                //"Hero_Kayras",
                "Hero_ZacZar",
                "Ran",
                "Satyr",
                "Hero_Blizz",
                "Hero_Grimwald",
            };
    [MenuItem("CreatePrefabs/Execute", false, 100)]
    static void DoIt()
    {
        var directory_info = new DirectoryInfo(prefabsPath);
        string _fullAssetPath = prefabsPath;

        var files_list = directory_info.GetFiles();
        foreach (var f in files_list)
        {
            if (f.Extension != ".prefab") continue;
            //try
            //{
                PrefabRefresh(f);
            //}
            //catch
            //{
            //    var shortName = f.Name.Split('.')[0];
            //    Debug.Log("<color=red>Prefab</color> <color=green>" + shortName + " </color> Reassemble problem");
            //}
        }
    }

    [MenuItem("CreatePrefabs/ChangeUnits", false, 100)]
    static void ChangeUnits()
    {
        var directory_info = new DirectoryInfo(newPrefabsPath);

        var files_list = directory_info.GetFiles();
        int i = 1;
        foreach (var f in files_list)
        {
            if (f.Extension != ".prefab") continue;
            var shortName = f.Name.Split('.')[0];
            if (shortName == "BasicMinionPrefab") continue;
            var prefabToChange = AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(f.FullName));
            if (prefabToChange.name.Length > 3 && prefabToChange.name.Substring(0, 4) == "Hero") continue;
            i++;
            prefabToChange.transform.localScale -= new Vector3(0.15f, 0.15f, 0.15f);
            var shadow = prefabToChange.transform.Find("shadow");
            shadow.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            PrefabUtility.SavePrefabAsset(prefabToChange);
            Debug.Log("Prefab " + i + " updated: " + prefabToChange.name);

        }
    }

    [MenuItem("CreatePrefabs/Clear Excess Options", false, 100)]
    static void ClearExcessOptions()
    {
        var directory_info = new DirectoryInfo(newPrefabsPath);
        string _fullAssetPath = prefabsPath;

        var files_list = directory_info.GetFiles();
        foreach (var f in files_list)
        {
            if (f.Extension != ".prefab") continue;
            ClearOptions(f);
        }
    }

    static void ClearOptions(FileInfo fileInfo)
    {
        var shortName = fileInfo.Name.Split('.')[0];
        if (shortName == "BasicMinionPrefab") return;
        var prefabToChange = AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(fileInfo.FullName));
        var basicPrefabContentInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToChange);
        var sk = basicPrefabContentInstance.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach(var s in sk)
        {
            s.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            s.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            s.allowOcclusionWhenDynamic = false;
            s.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            s.receiveShadows = false;
        }

        var path = newPrefabsPath + "/" + shortName + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(basicPrefabContentInstance, path);
    }

    static void PrefabRefresh(FileInfo fileInfo)
    {
        var shortName = fileInfo.Name.Split('.')[0];
        if (Array.IndexOf(failedNames, shortName) == -1) return;
        if (shortName == "BasicMinionPrefab") return;

        var basicPrefabContent = AssetDatabase.LoadAssetAtPath<GameObject>(basicPrefabsPath);
        var basicPrefabContentInstance = (GameObject)PrefabUtility.InstantiatePrefab(basicPrefabContent);

        var prefabToChange = AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(fileInfo.FullName));
        var prefabToChangeInstance = (GameObject)GameObject.Instantiate(prefabToChange);
        var anim = prefabToChangeInstance.GetComponent<Animator>();
        var prefab_content_source = AssetDatabase.GetAssetPath(anim.runtimeAnimatorController);
        if (prefab_content_source == null) return;

        var parts = prefab_content_source.Split('/');
        var cropped = new string[parts.Length - 1];
        Array.Copy(parts, 0, cropped, 0, cropped.Length);
        var folder = string.Join("/", cropped);

        var new_source_path = GetPrefabSourcePath(folder);

        if (new_source_path == null) return;
        var prefabToParse = AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(new_source_path.FullName));
        var prefabToParseInstance = (GameObject)GameObject.Instantiate(prefabToParse);
        MergePrefabs(prefabToChangeInstance, prefabToParseInstance, basicPrefabContentInstance);

        var path = newPrefabsPath + "/" + shortName + ".prefab";

        PrefabUtility.SaveAsPrefabAsset(basicPrefabContentInstance, path);
        Debug.Log("Prefab " + shortName + " updated");
        GameObject.DestroyImmediate(prefabToChangeInstance, true);
        GameObject.DestroyImmediate(basicPrefabContentInstance, true);
        GameObject.DestroyImmediate(prefabToParseInstance, true);
    }

    static void MergePrefabs(GameObject oldPrefab, GameObject newModelContent, GameObject basicPrefab)
    {
        List<string> names = new List<string>();

        var m1 = oldPrefab.transform.Find("Main").gameObject;
        var g1 = oldPrefab.transform.Find("Geometry").gameObject;
        var m2 = newModelContent.transform.Find("Main").gameObject;
        var g2 = newModelContent.transform.Find("Geometry").gameObject;
        var h1 = HierarchyNode.buildHierarchy(m1);
        var h2 = HierarchyNode.buildHierarchy(m2);
        ParseNodes(h1, h2);

        for (var i = 0; i < newModelContent.transform.childCount; i++)
        {
            var tr = newModelContent.transform.GetChild(i);
            if (!names.Contains(tr.name))
                names.Add(tr.name);
        }
        for (var i = 0; i < basicPrefab.transform.childCount; i++)
        {
            var tr = basicPrefab.transform.GetChild(i);
            if (!names.Contains(tr.name))
                names.Add(tr.name);
        }
        for (var i = 0; i < oldPrefab.transform.childCount; i++)
        {
            var tr = oldPrefab.transform.GetChild(i);
            var go = tr.gameObject;

            if (go == m1 || go == g1)
            {
                continue;
            }
            else
            {
                if (names.Contains(go.name))
                {
                    continue;
                }
            }
            go.transform.SetParent(basicPrefab.transform);
        }
        m2.transform.SetParent(basicPrefab.transform);
        g2.transform.SetParent(basicPrefab.transform);
        var a = basicPrefab.GetComponent<Animator>();
        var aold = oldPrefab.GetComponent<Animator>();
        a.runtimeAnimatorController = aold.runtimeAnimatorController;
        a.avatar = aold.avatar;
    }

    static void ParseNodes(HierarchyNode node1, HierarchyNode node2)
    {
        foreach (var node in node1.childs)
        {
            if (!node2.hasChild(node.name))
            {
                GameObject newGO;
                newGO = GameObject.Instantiate(node.gameObject, node2.gameObject.transform);
                newGO.name = node.gameObject.name;
                continue;
            }
            ParseNodes(node, node2.getChildNode(node.name));
        }
    }

    static string GetAssetPath(string fullPath)
    {
        var splitList = fullPath.Split(new string[] { "Assets\\" }, StringSplitOptions.None);
        var fpath = splitList[1];
        return "Assets/" + fpath;
    }

    static FileInfo GetPrefabSourcePath(string folderName)
    {
        var directory_info = new DirectoryInfo(folderName);
        var files_list = directory_info.GetFiles();
        foreach (var f in files_list)
        {
            if (f.Extension != ".fbx") continue;
            var isWanted = f.FullName.Split(new string[] { "_atlas" }, StringSplitOptions.None);
            if (isWanted.Length <= 1) continue;
            return f;
        }
        return null;
    }
}

public class HierarchyNode
{
    public HierarchyNode parent;
    public GameObject gameObject;
    public string name;
    public List<string> childNames = new List<string>();
    public List<HierarchyNode> childs = new List<HierarchyNode>();
    public HierarchyNode()
    {
    }

    public bool AddChild(GameObject gameObject)
    {
        if (childNames.IndexOf(gameObject.name) != -1) return false;
        childNames.Add(gameObject.name);
        var nd = buildHierarchy(gameObject, this);
        childs.Add(nd);
        return true;
    }

    public bool hasChild(string name)
    {
        return childNames.IndexOf(name) != -1;
    }

    public static HierarchyNode buildHierarchy(GameObject gameObject)
    {
        HierarchyNode root = new HierarchyNode();
        root.gameObject = gameObject;
        root.name = gameObject.name;
        var count = gameObject.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var go = gameObject.transform.GetChild(i).gameObject;
            root.AddChild(go);
        }
        return root;
    }

    public static HierarchyNode buildHierarchy(GameObject gameObject, HierarchyNode parentNode)
    {
        HierarchyNode root = buildHierarchy(gameObject);
        root.parent = parentNode;
        return root;
    }

    public HierarchyNode getChildNode(string nodeName)
    {
        int index = childNames.IndexOf(nodeName);
        if (index == -1) return null;
        return childs[index];
    }
}

#endif
