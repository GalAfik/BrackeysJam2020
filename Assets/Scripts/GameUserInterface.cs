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

	// Start is called before the first frame update
	void Start()
    {
		Animator = GetComponent<Animator>();
		Player = Resources.Load<Player>("Player");
		Player.AddListener(SetButtonPosition);

		// Set the number of phrases required in the UI
		NumberPhrasesRequired?.SetText(Player.Recording.NumberOfSentimentsInSolution.ToString());
	}

	private void SetButtonPosition(PlayerState newState, PlayerState oldState)
	{
		if (newState == PlayerState.Submitted)
		{
			Animator.SetBool("Visible", false);
		}
		else if (newState != PlayerState.Demo)
		{
			Animator.SetBool("Visible", true);
			// If the demo just ended, show the Press Rewind animation
			if (oldState == PlayerState.Demo) Animator.SetTrigger("PressRewind");
		}
	}
}
