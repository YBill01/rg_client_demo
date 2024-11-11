using UnityEngine;
using UnityEngine.UI;
using Legacy.Client;

public class BottleBlickBehaviour : MonoBehaviour
{
	private const float maxFrame = 45;
	private const string paramName = "_frameNumber";
	private static int frameParamId = Shader.PropertyToID(paramName);

	[SerializeField]
	[Tooltip("Magic value for current shader and sprites. In miliseconds")]
	private float magicProlongator = 1.18f; //Заполнение бутылки выглядит не красиво, но если полное заполненение проводить во столько раз дольше, все красиво

	private Material bottleMaterial;

	private bool inited;
	private bool isPause;
	private float startTime;
	private float pauseTime;
	
	void Init()
	{
		if (inited)
			return;

		inited = true;
		bottleMaterial = GetComponent<Image>().material;
	}

	public void Pause()
	{
		isPause = true;
		pauseTime = Time.time;
	}

	public void Play()
	{
		Init();


		if (!gameObject.activeSelf)
			gameObject.SetActive(true);

		if (isPause)
		{
			isPause = false;
			var delta = pauseTime - startTime;
			startTime = Time.time - delta;
		}
		else
		{
			startTime = Time.time;
			bottleMaterial.SetFloat(frameParamId, 0);
		}
	}

	void Update()
	{
		if (isPause)
			return;
		
		var intMana = Mathf.FloorToInt(ManaUpdateSystem.PlayerMana);
		var partOfMana = ManaUpdateSystem.PlayerMana - intMana;

		var frame = partOfMana * maxFrame / magicProlongator;

		bottleMaterial.SetFloat(frameParamId, frame);
	}
}
