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
	private bool Displaying;

    // Start is called before the first frame update
    void Start()
    {
		Animator = GetComponent<Animator>();
		Player = Resources.Load<Player>("Player");
	}

	private void Update()
	{
		if (Input.anyKey && Displaying)
		{
			DismissMessage();
		}
	}
	
	public void DisplayMessage(string message)
	{
		Displaying = true;
		Text?.SetText(message);
		Animator?.SetBool("Display", true);
	}

	public void DismissMessage()
	{
		Displaying = false;
		Animator?.SetBool("Display", false);

		// Go back to the game if the player just tried to submit a failed solution
		Player.ReverseSubmission();
	}
}
