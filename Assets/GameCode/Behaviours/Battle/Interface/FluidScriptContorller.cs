using Legacy.Client;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FluidScriptContorller : MonoBehaviour
{
	public TextMeshProUGUI manaText;
	public float maxMana = 10;
	public float valueToJump;
	public Image image;
	public Material fluidMaterial;
	public float normalAmplitude;
	private float currentAmplitude;
	[Range(0,1)]
	public float currentPosition;
	[Range(0,1)]
	public float nextPosition;
    void Start()
    {
		fluidMaterial = image.material;
		//normalAmplitude = fluidMaterial.GetFloat("_AmplitudeMultiplier");
		currentAmplitude = normalAmplitude;
		//currentPosition = fluidMaterial.GetFloat("_Progress");
		newPlayerMana = (int)ManaUpdateSystem.PlayerMana;
		currentPosition = (float)newPlayerMana / (float)maxMana;
		nextPosition = currentPosition;
		particleSystem = particleSystemObject.GetComponent<ParticleSystem>();
	}

	private float newPlayerMana;
	private float disapplyTime = 0;
	private float applyTime = 0;
	private bool applied;
    private void CheckManaChanged()
	{
		if (ManaUpdateSystem.setImmediatelly)
		{
			newPlayerMana = ManaUpdateSystem.PlayerMana;
			Jump(newPlayerMana / maxMana);
			manaText.text = ((int)newPlayerMana).ToString();
			applyTime = 0;
			return;
		}
		
		if ((int)newPlayerMana == (int)ManaUpdateSystem.PlayerMana)
		{
			if(!applied)
				applyTime += Time.deltaTime;
		}
		else
		{
			applied = false;
			newPlayerMana = (int)ManaUpdateSystem.PlayerMana;
			applyTime = 0;
		}
		if (applyTime < 0.4f)
			return;
		applied = true;
		applyTime = 0;
		Jump(newPlayerMana / maxMana);
		manaText.text = ((int)ManaUpdateSystem.PlayerMana).ToString();
	}

    void Update()
    {
		fluidMaterial.SetFloat("_SpendProgress", ManaUpdateSystem.ManaToUse / maxMana);
		CheckManaChanged();
		currentPosition = Mathf.Lerp(currentPosition, nextPosition, 0.5f);
		fluidMaterial.SetFloat("_Progress", currentPosition);
		punchProgress = Mathf.Clamp((float)punchProgress + Time.deltaTime * (float)punchProgressSpeed * (float)punchDirection, 0f, 1f);

		if (punchProgress >= 1 && punchDirection != -1)
		{
			punchDirection = -1;
			punchProgress = 1;
		}
		if (punchProgress <= 0)
		{
			punchDirection = 0;
			punchProgress = 0;
		}

		//currentAmplitude = normalAmplitude + Mathf.Sin(punchProgress * Mathf.PI / 2);
		currentAmplitude = normalAmplitude + punchProgress * maxAmplitude;
		fluidMaterial.SetFloat("_AmplitudeMultiplier", currentAmplitude);
		Vector3 addScale = (Vector3.one * (Mathf.Sin((punchProgress * Mathf.PI / 2) * 4) + 1) / 2) * 0.05f;
		if(addScale.y<0)
		{
			Debug.Log("Whyy!");
		}
		transform.localScale = Vector3.one + addScale;

		UpdateRotation();

		var c = floassImage.color;
		if (currentPosition >= 0.95)
		{

			addTimeMultiplier = 3f;
			c.a = 255;
			if (!particleSystemObject.active)
			{
				particleSystemObject.SetActive(true);
			}
			if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
		else
		{
			addTimeMultiplier = 1f;
			c.a = 0;
			particleSystem.Stop();
		}
		floassImage.color = c;
	}

	public GameObject particleSystemObject;
	private ParticleSystem particleSystem;
	private float addTimeMultiplier = 1;

	public float maxAmplitude = 0.05f;
	public float punchProgressSpeed = 0.1f;
	[Range(0,1)]
	private float punchProgress;
	private float punchDirection = 0;
	public void Jump(float value)
	{
		nextPosition = value;
		punchDirection = 1;
		//float startTime = fluidMaterial.GetFloat("_time");
		//fluidMaterial.SetFloat("_startTime", startTime);
	}

	public RectTransform bottleImage;
	public Image floassImage;
	public float rotationAplitude = 5;
	public float rotationSpeedMultiplier = 1;
	private float rotationProgress;
	private void UpdateRotation()
	{
		rotationProgress += Time.deltaTime * addTimeMultiplier;
		var e = bottleImage.eulerAngles;
		e.z = Mathf.Sin(rotationProgress * rotationSpeedMultiplier) * rotationAplitude;
		bottleImage.eulerAngles = e;
	}

	private bool _spend;
	public void ToggleSpend()
	{
		_spend = !_spend;
		if(_spend)
		{
			fluidMaterial.SetFloat("_SpendProgress", 0.3f);
		}
		else
		{
			fluidMaterial.SetFloat("_SpendProgress", 0);
		}
	}
}
