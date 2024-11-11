using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeamColorBehaviour : MonoBehaviour
{

    [SerializeField]
    public Color[] colors;

    public byte TeamID { get; private set; }

    private GameObject geometryContainer;
    private SkinnedMeshRenderer[] renderers;
    void Start()
    {
        geometryContainer = GetComponent<MinionInitBehaviour>() ? GetComponent<MinionInitBehaviour>().geometry : transform.Find("Geometry").gameObject;
        renderers = geometryContainer.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public bool SetTeam(byte teamID)
    {
        if (colors.Length <= teamID) return false;
        TeamID = teamID;
        RefreshMaterials();
        return true;
    }

    public void RefreshMaterials()
    {
        if (!Application.isPlaying)
        {
            geometryContainer = GetComponent<MinionInitBehaviour>() ? GetComponent<MinionInitBehaviour>().geometry : transform.Find("Geometry").gameObject;
            renderers = geometryContainer.GetComponentsInChildren<SkinnedMeshRenderer>();
        }
        if (renderers != null && renderers.Length > 0)
            foreach (var r in renderers)
            {
                foreach (var m in r.materials)
                {
                    m.SetColor("_color_accessories", colors[TeamID]);
                }
            }
    }
    public void RefreshMaterials(ref GameObject customGeometry, bool isEnemy)
    {
        var additionalRenderers = customGeometry.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in additionalRenderers)
        {
            foreach (var m in r.materials)
            {
                m.SetColor("_color_accessories", colors[System.Convert.ToInt32(isEnemy)]);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TeamColorBehaviour))]
public class colorTest : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = (TeamColorBehaviour)target;
        if (GUILayout.Button("Set next team"))
        {
            t.SetTeam((byte)((t.TeamID + 1) % t.colors.Length));
        }
    }
}
#endif