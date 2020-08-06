using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]

public class SplashScreen : MonoBehaviour
{
	[HideInInspector] public RawImage Image;

	// Start is called before the first frame update
	void Awake()
	{
		Image = GetComponent<RawImage>();
	}
}
