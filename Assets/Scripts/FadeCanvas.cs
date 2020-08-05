using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCanvas : MonoBehaviour
{
	private static FadeCanvas self;

	private Animator Animator;

	private void Awake()
	{
		if (self == null)
		{
			self = this;
			return;
		}
		Destroy(gameObject);
	}

	// Start is called before the first frame update
	void Start()
    {
		Animator = GetComponent<Animator>();
    }

    public void FadeOut()
	{
		Animator?.SetBool("FadeOut", true);
	}

	public void FadeIn()
	{
		Animator?.SetBool("FadeOut", false);
	}
}
