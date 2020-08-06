using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]

public class Newspaper : MonoBehaviour
{
	private Animator Animator;
	[HideInInspector] public RawImage Image;

    // Start is called before the first frame update
    void Awake()
    {
		Animator = GetComponent<Animator>();
		Image = GetComponent<RawImage>();
    }

    public void Display()
	{
		Animator?.SetTrigger("Display");
	}
}
