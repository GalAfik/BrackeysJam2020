using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScenePicker))]

public class Level : MonoBehaviour
{
	public int NumberNeededToUnlock = 1;
	public bool Locked { get; set; }
	public bool Completed { get; private set; }

	private Light SpotLight;

	private void Start()
	{
		SpotLight = GetComponentInChildren<Light>();
	}

	private void Update()
	{
		// Turn on the spotlight if the level is unlocked
		if (Locked) SpotLight?.gameObject.SetActive(false);
		else SpotLight?.gameObject.SetActive(true);
	}

	public void CompleteLevel()
	{
		// Once a level has been completed, it cannot be replayed until the game is finished
		Completed = true;
	}

	private void OnMouseOver()
	{
		// Check if the level is locked
		if (Locked) return;

		// Highlight the level by increasing the spotlight's intensity and size
		print("hover");
	}

	private void OnMouseDown()
	{
		// Check if the level is locked
		if (Locked) return;

		// Load the level
		MenuController.LoadLevel(GetComponent<ScenePicker>().scenePath);
	}
}
