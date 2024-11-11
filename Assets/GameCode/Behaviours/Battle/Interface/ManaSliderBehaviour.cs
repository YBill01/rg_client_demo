using Legacy.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaSliderBehaviour : MonoBehaviour
{
    public GameObject[] Accumulate;
    public GameObject[] Full;
    public GameObject[] Need;
    //public GameObject Highlight;
    //public GameObject TextHighlight;
    //public GameObject BoostImage;
    public TextMeshProUGUI ManaText;

    //public Animator SliderAnimator;

    private GameObject ActiveNeedObject = null;
    private GameObject ActiveAccumulateObject = null;

    void Update()
    {
        ManaText.text = ((int)ManaUpdateSystem.PlayerMana).ToString();
        NeedUpdate();
        MainUpdate();
        AccumulateUpdate();
    }

    private void MainUpdate()
    {
        for(var i = 0; i < Full.Length; i++)
        {
            Activate(Full[i], i + 1 <= ManaUpdateSystem.PlayerMana);
        }
    }

    private void AccumulateUpdate()
    {
        if (ActiveAccumulateObject != null)
        {
            for (var i = 0; i < Accumulate.Length; i++)
            {
                Activate(Accumulate[i], Accumulate[i] == ActiveAccumulateObject);
            }
        }
        float accumulatingNumber = Mathf.Ceil(ManaUpdateSystem.PlayerMana);
        float delta = -(ManaUpdateSystem.PlayerMana - accumulatingNumber);
        if(accumulatingNumber > 0 && accumulatingNumber < 11 && delta > 0)
        {            
            ActiveAccumulateObject = Accumulate[(int)accumulatingNumber - 1];
            ActiveAccumulateObject.GetComponent<Image>().fillAmount = 1 - delta;
        }        
    }

    void Activate(GameObject obj, bool enabled)
    {
        var needA = enabled ? 255 : 0;
        if (obj.GetComponent<Image>().color.a != needA)
        {
            var color = obj.GetComponent<Image>().color;
            color.a = needA;
            obj.GetComponent<Image>().color = color;
            
            if(enabled)
            {
                var animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play("ManaEntry");
                }
            }            
        }
    }

    
    public void ActivateHighlight()
    {
        //BoostImage.SetActive(true);
        //Highlight.SetActive(true);
        //TextHighlight.SetActive(true);
    }

    public void DeactivateHighlight()
    {
        //BoostImage.SetActive(false);
        //Highlight.SetActive(false);
        //TextHighlight.SetActive(false);
    }

    void NeedUpdate()
    {
        for (var i = 0; i < Need.Length; i++)
        {
            Need[i].SetActive(i + 1 == ManaUpdateSystem.ManaToUse);                
        }
    }
}
