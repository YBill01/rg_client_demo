using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleFlagsBehaviour : MonoBehaviour
{
    [SerializeField] List<BattleFlagAnimationController> flags = new List<BattleFlagAnimationController>();
    private void Start()
    {
        flags = this.GetComponentsInChildren<BattleFlagAnimationController>().ToList();
    }

    public void DisableFlag(int index)
    {
        if (index != 0)
        {
            var flag = flags[index - 1];
            if (flag.gameObject.activeSelf)
            {
                flag.StartCoroutine(flag.BurnEffect());
            }
        }
    }
}
