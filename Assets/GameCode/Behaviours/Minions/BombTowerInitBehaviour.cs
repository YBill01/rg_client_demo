using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTowerInitBehaviour : InitBehaviour
{
    [SerializeField] public GameObject additionalGeometry;
    [SerializeField] public GameObject additionalMain;
    public override void InitMaterial(bool isEnemy)
    {
        GetComponent<TeamColorBehaviour>().RefreshMaterials(ref additionalGeometry,isEnemy);
    }
    public override void DoMinionInvisible()
    {
        if (additionalGeometry) additionalGeometry.SetActive(false);
        if (additionalMain) additionalMain.SetActive(false);
    }

    public override void DoMinionVisible()
    {
        if (additionalGeometry)
            additionalGeometry.SetActive(true);
        if (additionalMain)
            additionalMain.SetActive(true);
    }
}
