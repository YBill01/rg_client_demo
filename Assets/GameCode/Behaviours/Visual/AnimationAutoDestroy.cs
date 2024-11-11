using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAutoDestroy : MonoBehaviour
{
	private Animator animator;

	public Animator Animator { get => animator; }

	void Start()
	{
	}

	void Update()
	{
		if(animator == null)
		{
			animator = GetComponent<Animator>();
			return;
		}
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("Destroy"))
		{
			Destroy(gameObject);
		}
	}
}
