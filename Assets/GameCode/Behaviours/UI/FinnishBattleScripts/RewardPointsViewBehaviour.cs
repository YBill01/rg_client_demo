using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RewardPointsViewBehaviour : MonoBehaviour
{
	public UnityEvent doneEvent;
	public int value = 125;

	[SerializeField]
	public SpeedValuables[] speedValuables;

	[SerializeField]
	private TextMeshProUGUI valueText;
	private float startTime;
	public void DoTakeDamageEffect()
	{
		hitProgress = 1 - hitProgress;
	}
	private float hitProgress = 1;
	public float hitEffectTime = 0.2f;
	public float hitEffectAmplitude = 0.3f;

	private bool done;
	void Update()
	{
		UpdateHitProgress();
		if (done) return;
		UpdateNumber();
	}

	private void UpdateHitProgress()
	{
		if (hitProgress >= 1)
		{
			hitProgress = 1;
			valueText.transform.localScale = Vector3.one;
		}
		else
		{
			hitProgress += Time.deltaTime / hitEffectTime;
			if (hitProgress > 1)
				hitProgress = 1;
			float finalProgress = Mathf.Abs(Mathf.Sin(hitProgress * (Mathf.PI) * 2));
			valueText.transform.localScale = Vector3.one * (1 + finalProgress * hitEffectAmplitude);
		}
	}

	int lastValueableNumber;
	private void UpdateNumber()
	{
		int currentValue = (int)((Time.time - startTime) * currentValuable.speed);
		if(currentValue >= value)
		{
			currentValue = value;
			done = true;
		}
		valueText.text = currentValue.ToString();
		if (currentValue - lastValueableNumber > currentValuable.valueNumber)
		{
			lastValueableNumber = currentValue;
			DoTakeDamageEffect();
		}
		if(done)
		{
			doneEvent.Invoke();
		}
	}

	private SpeedValuables currentValuable;
	private void OnEnable()
	{
		done = false;
		startTime = Time.time;
		currentValuable = speedValuables[0];
		foreach (var sv in speedValuables)
		{
			if (value > sv.availableOn && currentValuable.availableOn < sv.availableOn)
			{
				currentValuable = sv;
			}
		}
		valueText.text = "0";
	}

}

[System.Serializable]
public struct SpeedValuables
{
	public float speed;
	public int availableOn;
	public int valueNumber;
}