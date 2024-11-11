using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Legacy.Client
{
    public class ExitSandboxBehaviour : MonoBehaviour
    {
        public void Exit()
        {
            ClientWorld.Instance.ExitSandbox();
        }

    }
}