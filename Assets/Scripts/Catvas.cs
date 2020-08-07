using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Catvas : MonoBehaviour
{
	public GameObject ObjectToShow;
	public GameObject ObjectToHide;

	private bool Cat = false;

    public void ToggleCat()
	{
		Cat = !Cat;
		GetComponent<Animator>().SetBool("Cat", Cat);
		// Play Cat sound
		if (Cat) FindObjectOfType<AudioManager>().Play(AudioManager.SFX.Cat);

		// Display/Hide taglines
		ObjectToShow?.SetActive(Cat);
		ObjectToHide?.SetActive(!Cat);
	}
}
