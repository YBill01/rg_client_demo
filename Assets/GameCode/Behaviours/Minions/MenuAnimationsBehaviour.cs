using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationsBehaviour : MonoBehaviour
{
    /// <summary>
    /// Called from animator. Event in clip.
    /// </summary>
    public void FinishAction()
    {
        GetComponent<Animator>().SetBool("Action", false);
    }

    public void Action()
    {
        GetComponent<Animator>().SetBool("Action", true);
    }
}
