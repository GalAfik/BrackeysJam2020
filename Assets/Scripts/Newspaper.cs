using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newspaper : MonoBehaviour
{
	private Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
		Animator = GetComponent<Animator>();    
    }

    public void Display()
	{
		Animator?.SetTrigger("Display");
	}
}
