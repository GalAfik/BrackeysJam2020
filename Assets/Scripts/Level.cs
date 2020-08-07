using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ScenePicker))]

public class Level : MonoBehaviour
{
	public int NumberNeededToUnlock = 1;
	public bool Locked { get; set; }
	public bool Completed;// { get; private set; }
	public TMP_Text CaptionLabel;
	public string Caption;
	public string UnlockedCaption;
	public GameObject[] DifficultyMarkers;

	private Light SpotLight;
	private Animator Animator;

	private void Start()
	{
		SpotLight = GetComponentInChildren<Light>();
		Animator = GetComponent<Animator>();

		// Set the caption label
		if (CaptionLabel != null)
		{
			CaptionLabel.SetText(Caption);
			CaptionLabel.gameObject.SetActive(false);
		}
		// Hide the difficulty markers
		foreach (var marker in DifficultyMarkers)
		{
			marker.SetActive(false);
		}
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

	private void OnMouseEnter()
	{
		// Show the caption label
		if (!Locked) CaptionLabel?.SetText(UnlockedCaption);
		if (CaptionLabel != null) CaptionLabel.gameObject.SetActive(true);
		
		// Check if the level is locked
		if (Locked) return;

		// Show the difficulty markers
		foreach (var marker in DifficultyMarkers)
		{
			marker.SetActive(true);
		}

		// Highlight the level by increasing the spotlight's intensity and size
		Animator.SetBool("Selected", true);
	}

	private void OnMouseExit()
	{
		// Hide the caption label
		if (CaptionLabel != null) CaptionLabel.gameObject.SetActive(false);
		
		// Check if the level is locked
		if (Locked) return;

		// Hide the difficulty markers
		foreach (var marker in DifficultyMarkers)
		{
			marker.SetActive(false);
		}

		Animator.SetBool("Selected", false);
	}

	private void OnMouseDown()
	{
		// Check if the level is locked
		if (Locked) return;

		// Load the level
		FindObjectOfType<MenuController>().LoadLevel(this);
	}
}
