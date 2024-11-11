using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class SkillChargeEffectBehaviour : MonoBehaviour
{
	[SerializeField]
	float delay_0 = 0.15f;
	[SerializeField]
	float delay_1 = 0.11f;
	[SerializeField]
	float delay_2 = 0.7f;

	[SerializeField]
	float speed_0 = 360f;
	[SerializeField]
	float speed_1 = 360f;
	[SerializeField]
	float speed_2 = 360f;

	[SerializeField]
	Vector2 width_0 = new Vector2(210, 50);
	[SerializeField]
	Vector2 width_1 = new Vector2(210, 70);
	[SerializeField]
	Vector2 width_2 = new Vector2(210, 90);

	[SerializeField]
	ParticleSystem effect_1;
	[SerializeField]
	ParticleSystem effect_2;
	[SerializeField]
	ParticleSystem effect_2_2;

	private int myBridgesCount = -1;
	[SerializeField]
	private float currentSpeed = 360;
	private float currentDelay = 0.2f; // max value
	private float delay = 0f; // value

	[SerializeField]
	bool runInEditoMode;

	private RectTransform rect;

	private void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	public void UpdateState(byte curentBridgesCount, bool isCharging)
	{
		if (myBridgesCount == curentBridgesCount)
			return;

		myBridgesCount = curentBridgesCount;

		if(effect_1) effect_1.gameObject.SetActive(myBridgesCount == 1);
		if (effect_2) effect_2.gameObject.SetActive(myBridgesCount == 2);
		if (effect_2_2) effect_2_2.gameObject.SetActive(myBridgesCount == 2 && isCharging && gameObject.activeSelf);

		//switch (myBridgesCount)
		//{
		//	case 0:
		//		currentSpeed = speed_0;
		//		rect.sizeDelta = width_0;
		//		currentDelay = delay_0;
		//		break;
		//	case 1:
		//		currentSpeed = speed_1;
		//		rect.sizeDelta = width_1;
		//		currentDelay = delay_1;
		//		break;
		//	default:
		//		currentSpeed = speed_2;
		//		rect.sizeDelta = width_2;
		//		currentDelay = delay_2;
		//		break;
		//}
	}

	public void UpdateChargeEffect(bool isFull, byte myBridgesCount)
	{
		if (gameObject.activeSelf && isFull)
		{
			gameObject.SetActive(false);
			if (effect_2_2) effect_2_2.gameObject.SetActive(false);
		}

		if (!isFull)
		{
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);
		}

		UpdateState(myBridgesCount, !isFull);
	}

	void Update()
	{
		if (!Application.isPlaying && !runInEditoMode)
			return;

		if (delay > 0)
		{
			delay -= Time.deltaTime;
			return;
		}

		var angle = transform.eulerAngles;
		var newValue = angle;
		newValue.z -= Time.deltaTime * currentSpeed;

		if (newValue.z >= 90 && angle.z < 90)
		{
			delay = currentDelay;
			newValue.z = 90;
		}

		transform.eulerAngles = newValue;
	}
}
