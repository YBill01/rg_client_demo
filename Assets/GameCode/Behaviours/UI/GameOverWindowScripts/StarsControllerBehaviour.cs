using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StarsControllerBehaviour : MonoBehaviour
{
    [SerializeField] private Sprite validStar;
    [SerializeField] private Sprite notValidStar;

    public IEnumerator SetStars(int countOfValidStars)
    {
        var childrenImages = transform.GetComponentsInChildren<StarEmptyBehaviour>();
        for (int i = 0; i < childrenImages.Length; i++)
        {
            if (i < countOfValidStars && countOfValidStars != 0)
            {
                childrenImages[i].GetComponent<Image>().sprite = validStar;
                childrenImages[i].GetComponent<Animator>().enabled = true;//play anim
                childrenImages[i].GetComponent<AudioSource>().Play();
                if (childrenImages[i].CanFly())
                {
                    childrenImages[i].GetComponent<Animator>().Play("MyToCenter");
                }
                yield return new WaitForSeconds(0.35f);
            }
            else
            {
                childrenImages[i].GetComponent<Image>().sprite = notValidStar;
            }
        }
    }
}
