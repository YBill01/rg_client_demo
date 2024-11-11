using Legacy.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSubstracteBehaviour : MonoBehaviour
{
    private bool isNew;

    public void UpdateLableNew(ushort binaryIndex, bool flag = true)
    {
        isNew = ClientWorld.Instance.Profile.Inventory.GetCardData(binaryIndex).level == 0 && ClientWorld.Instance.Profile.Inventory.GetCardData(binaryIndex).count == 0;
        if(!isNew && ClientWorld.Instance.Profile.Inventory.GetCardData(binaryIndex).isNew)
          isNew = ClientWorld.Instance.Profile.Inventory.GetCardData(binaryIndex).isNew;
        if (!flag)
            this.gameObject.SetActive(flag);
        else 
            this.gameObject.SetActive(isNew);
    }

}
