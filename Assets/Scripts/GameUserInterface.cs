using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUserInterface : MonoBehaviour
{
	private Animator Animator;
	private Player Player;

	// Start is called before the first frame update
	void Start()
    {
		Animator = GetComponent<Animator>();
		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
		Player.AddListener(SetButtonPosition);
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
