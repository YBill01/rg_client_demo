using UnityEngine;
using System.Collections;
using Unity.Entities;
using Legacy.Client;
using Unity.Collections;
using System.Linq;
using static Legacy.Client.InputSystem;

public class ArenaZonesBehaviour : MonoBehaviour
{
    public static ArenaZonesBehaviour instance;
    [SerializeField]
    private MeshRenderer allyZoneView1;
    [SerializeField]
    private MeshRenderer allyZoneView2;
    [SerializeField]
    private MeshRenderer allZoneView;

    private EntityQuery zonesHighlightQuery;

    private bool someEnabled;

    void Start()
    {
        instance = this;
        zonesHighlightQuery = ClientWorld.Instance.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ArenaZoneHighlight>());
    }

    void Update()
    {
        if (zonesHighlightQuery.IsEmptyIgnoreFilter)
        {
            allyZoneView1.enabled = false;
            allyZoneView2.enabled = false;
            allZoneView.enabled = false;

            return;
        }

        var zones = zonesHighlightQuery.ToComponentDataArray<ArenaZoneHighlight>(Allocator.TempJob);

        if (zones.Any((x) => x.allZone))
        {
            allyZoneView1.enabled = false;
            allyZoneView2.enabled = false;
            allZoneView.enabled = true;
        }
        else
        {
            allyZoneView1.enabled = true;
            allyZoneView2.enabled = true;
            allZoneView.enabled = false;
        }
        zones.Dispose();

    }

    private void OnDestroy()
    {
        if (!zonesHighlightQuery.IsEmptyIgnoreFilter)
        {
            var entities = zonesHighlightQuery.ToEntityArray(Allocator.TempJob);
            foreach (var entity in entities)
                ClientWorld.Instance.EntityManager.DestroyEntity(entity);
            entities.Dispose();
        }
    }
    Vector3 RayDirection = Vector3.down*10;
    public void SetRegularPosition(Vector3 minionPosition)
    {
        minionPosition.y = minionPosition.y + 0.5f;

        GameObject Zone = null;

        RaycastHit[] ZonesTouchRay = Physics.RaycastAll(minionPosition, RayDirection);
        Debug.DrawRay(minionPosition, RayDirection);
        foreach (RaycastHit r in ZonesTouchRay)
        {
            if (r.transform.gameObject.tag != "AllySpawnZone") continue;
            Zone = r.transform.gameObject;
            break;
        }

        HighlightAllyQuarter(Zone);
    }

    private GameObject HighLighted;
    private void HighlightAllyQuarter(GameObject Quarter)
    {
        if (HighLighted == Quarter) return;
        UnlightAllyZone();
        if (Quarter == null) return;
        HighLighted = Quarter;
        SetStateTo(Quarter, true);
    }

    private void UnlightAllyZone()
    {
        if (HighLighted == null) return;
        SetStateTo(HighLighted, false);
        HighLighted = null;
    }
    private void SetStateTo(GameObject Zone, bool active)
    {
        MeshRenderer mRend = Zone.GetComponent<MeshRenderer>();
        Material zoneMaterial = mRend.material;
        float Alpha = active ? 0 : 0.5f;
        zoneMaterial.SetColor("_BaseColor", Color.Lerp(Color.cyan, Color.black, Alpha));
        mRend.material = zoneMaterial;
    }
}
