using UnityEngine;
using System.Collections;
using DG.Tweening;
using Legacy.Database;

public class EnemyHighlightBehaviour : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer planeRenderer;
	[SerializeField]
	private ParticleSystem[] heroParticleSystems;

	[SerializeField]
	private float timeToFullColoredPlane = 1;
	[SerializeField]
	private float timeToFullTransperentPlane = 2;
	[SerializeField]
	private float showMessageAfter = 2;
	[SerializeField]
	private float disableParticleAfter = 3;

	private void Start()
	{
		//planeRenderer.material.color = new Color(1, 1, 1, 1);
		StartCoroutine(ShowTimeline());
	}

	private IEnumerator ShowTimeline()
	{
		var sequence = DOTween.Sequence();
		sequence.Append(planeRenderer.material.DOFade(1, timeToFullColoredPlane));
		sequence.Append(planeRenderer.material.DOFade(0, timeToFullTransperentPlane));

		yield return new WaitForSeconds(showMessageAfter);

		Vector2 messagePos = new Vector2(Screen.width * 0.7f, Screen.height * 0.45f);
		PopupAlertBehaviour.ShowBattlePopupAlert(messagePos, Locales.Get("locale:1159"));

		yield return new WaitForSeconds(disableParticleAfter);

		foreach (var particle in heroParticleSystems)
		{
			var main = particle.main;
			main.loop = false;
		}

		Destroy(gameObject, 3);
	}
}
