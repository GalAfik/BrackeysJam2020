using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
	private static Menu self;

	private void Awake()
	{
		if (self == null)
		{
			self = this;
			return;
		}
		Destroy(gameObject);
	}
}
