using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaysActivateEffect : MonoBehaviour
{
	[SerializeField]
	private RectTransform raysContainer;
	[SerializeField]
	private RectTransform glow;
	[SerializeField]
	float rotateSpeedStart = 1f;
	[SerializeField]
	float rotateSpeedEnd = 0.3f;
	[SerializeField]
	float rotateSlowTime = 3f;
	[SerializeField]
	float bumpStart = 0.5f;
	[SerializeField]
	float bumpValue = 1.5f;
	[SerializeField]
	float bumpTime = 0.5f;
	[SerializeField]
	float alphaTime = 0.5f;
	[SerializeField]
	float glowAmplitude = 0.5f;
	[SerializeField]
	float glowPeriod = 2f;
	void Start()
	{
	}

	void Update()
	{
		doUpdate();
	}

	private void  doUpdate()
	{
		UpdateAplha();
		UpdateRotation();
		UpdateRotateSpeed();
		UpdateBump();
		UpdateGlowAmplitude();
	}

	private float currentGlowTime;
	private void UpdateGlowAmplitude()
	{
		glow.localScale = Vector3.one + Vector3.one * Mathf.Sin(currentGlowTime / glowPeriod) * glowAmplitude;
		currentGlowTime += Time.deltaTime;
	}

	float currentAlphaTime;
	bool alphaDone;
	private void UpdateAplha()
	{
		if (alphaDone) return;
		currentAlphaTime += Time.deltaTime;
		if (currentAlphaTime > alphaTime)
		{
			currentAlphaTime = alphaTime;
			alphaDone = true;
		}
		float alpha = currentAlphaTime / alphaTime;
		foreach(var ray in rays)
			SetAlpha(ray, alpha);
		SetAlpha(glowImage, alpha);
	}

	private void SetAlpha(Image image,float alpha)
	{
		var c = image.color;
		c.a = alpha;
		image.color = c;
	}

	private float currentBumpTime;
	[SerializeField]
	int bumpTimes = 2;
	bool bumped;
	private void UpdateBump()
	{
		if (bumped) return;
		currentBumpTime += Time.deltaTime;
		if (currentBumpTime > bumpTime)
		{
			currentBumpTime = bumpTime;
			bumped = true;
		}
		float rel = currentBumpTime / bumpTime;
		float andgleValue = (rel * (Mathf.PI * 2)) * bumpTimes;
		float currentBumpValue = Mathf.Cos(andgleValue);
		raysContainer.transform.localScale = Vector3.one + Vector3.one * currentBumpValue * (1 - rel);
	}

	private float speedDelta;
	private float currentRotationSpeed;
	private void UpdateRotation()
	{
		var rot = raysContainer.localEulerAngles;
		rot.z += currentRotationSpeed * Time.deltaTime;
		raysContainer.localEulerAngles = rot;
	}

	bool slown;
	float currentTimePassed;
	private void UpdateRotateSpeed()
	{
		if (slown) return;
		currentTimePassed += Time.deltaTime;
		var rel = currentTimePassed / rotateSlowTime;
		currentRotationSpeed = rotateSpeedStart + speedDelta * rel;
		if (rel > 1)
		{
			currentRotationSpeed = rotateSpeedEnd;
			slown = true;
		}
	}

	private Image[] rays;
	private Image glowImage;
	private void OnEnable()
	{
		slown = false;
		bumped = false;
		alphaDone = false;
		currentBumpTime = 0;
		rays = raysContainer.GetComponentsInChildren<Image>();
		glowImage = glow.GetComponent<Image>();
		speedDelta = rotateSpeedEnd - rotateSpeedStart;
		currentTimePassed = 0;
		currentGlowTime = 0;
		currentRotationSpeed = rotateSpeedStart;
	}
}
