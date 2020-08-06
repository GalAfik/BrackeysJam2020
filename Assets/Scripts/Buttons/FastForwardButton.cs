using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FastForwardButton : MonoBehaviour
{
	private Player Player;

	private void Start()
	{
		Player = Resources.Load<Player>("Player");
		Player.AddListener(ToggleInteractivity);
	}

	private void ToggleInteractivity(PlayerState newState, PlayerState oldState)
	{
		// Disable the button when in the Done state
		if (newState == PlayerState.Done)
		{
			GetComponent<Button>().interactable = false;
		}
		else
		{
			GetComponent<Button>().interactable = true;
		}
	}
}
