using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class MessageSystem : MonoBehaviour
{
	public TMP_Text Text;
	private Animator Animator;
	private Player Player;

    // Start is called before the first frame update
    void Start()
    {
		Animator = GetComponent<Animator>();
		Player = Resources.Load<Player>("Player");
	}
	
	public void DisplayMessage(string message)
	{
		Text?.SetText(message);
		Animator?.SetBool("Display", true);
	}

	public void DismissMessage()
	{
		Animator?.SetBool("Display", false);

		// Go back to the game if the player just tried to submit a failed solution
		Player.ReverseSubmission();
	}
}
