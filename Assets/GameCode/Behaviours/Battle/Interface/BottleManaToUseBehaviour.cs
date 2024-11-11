using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Legacy.Client;

public class BottleManaToUseBehaviour : MonoBehaviour
{
	Image image;
	
	void Start()
	{
		image = GetComponent<Image>();
	}

	void Update()
	{
		image.fillAmount = Mathf.Clamp01(ManaUpdateSystem.ManaSelected / 10f);
	}
}
