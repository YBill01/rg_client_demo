using Legacy.Network;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class FinishOkButton : MonoBehaviour
{
    public Button Ok;
    void Start()
    {
        Ok.onClick.AddListener(OnOkClick);
    }

    private void OnOkClick()
    {
        StartCoroutine("ToMainMenu");
    }
    IEnumerator ToMainMenu()
    {
        //GameObject.Find("FaderCanvas").GetComponent<FaderCanvas>().Loader.FadeOut();
        yield return new WaitForSeconds(0.1f);
        /*var _query_connection = ClientWorld.Instance.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<PlayerGameClient>(),
                ComponentType.ReadOnly<GameConnectRequest>(),
                ComponentType.ReadOnly<NetworkAuthorization>()
            );
        var connection = _query_connection.GetSingletonEntity();
        
        ClientWorld.Instance.EntityManager.AddComponentData(connection, default(PlayerGameDisconnect));*/
    }
}
