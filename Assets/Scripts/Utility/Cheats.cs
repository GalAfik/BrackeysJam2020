using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
	private static Cheats self;

	private void Awake()
	{
		if (self == null)
		{
			self = this;
			return;
		}
		Destroy(gameObject);
	}

	private void Update()
    {
		if (Input.GetKey(KeyCode.LeftShift)) Time.timeScale = 5f;
		else Time.timeScale = 1f;
    }
}
