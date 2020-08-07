using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameUserInterface : MonoBehaviour
{
	private Animator Animator;
	private Player Player;
	public TMP_Text NumberPhrasesRequired;
	public TMP_Text NumberPhrasesRecorded;

	// Start is called before the first frame update
	private void Start()
    {
		Animator = GetComponent<Animator>();
		Player = Resources.Load<Player>("Player");
		Player.AddListener(SetButtonPosition);
	}

	private void Update()
    {
		SetPhrasesRecordedText();
	}

	private void SetButtonPosition(PlayerState newState, PlayerState oldState)
	{
		if (newState == PlayerState.Demo)
		{
			Animator.SetTrigger("ShowExitButton");
		}
		else if (newState == PlayerState.Done && oldState == PlayerState.Demo)
		{
			Animator.SetTrigger("ShowPlayButtons");
			// If the demo just ended, show the Press Rewind animation
			Animator.SetTrigger("PressRewind");
		}
		else if (newState == PlayerState.Submitted)
		{
			Animator.SetBool("Submitted", true);
		}
		else if (newState == PlayerState.Done && oldState == PlayerState.Submitted)
		{
			Animator.SetBool("Submitted", false);
		}
	}

	public void OnSplashComplete()
	{
		// Just started the level. Turn on the player to the DEMO state
		StartCoroutine(FindObjectOfType<GameController>().StartLevel());
	}

	public void SetPhrasesRequiredText(int phrasesRequired)
	{
		// Set the number of phrases required in the UI
		NumberPhrasesRequired?.SetText(phrasesRequired.ToString());
	}

	public void SetPhrasesRecordedText()
	{
		// Set the number of phrases recorded in the UI
		int numRecorded = Player.Recording.Sentiments.Count(sentiment => sentiment.Recorded);
		// Set the number of phrases required in the UI
		NumberPhrasesRecorded?.SetText(numRecorded.ToString());
	}
}
