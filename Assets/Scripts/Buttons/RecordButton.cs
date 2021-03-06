﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour
{
    public Color RecordingColorLight = Color.white;
	public Color RecordingColorDark = Color.red;
	public float LerpTime = 1f;

	private Player Player;
    private Color RecordButtonColor;

    private void Start()
    {
        Player = Resources.Load<Player>("Player");
        RecordButtonColor = gameObject.GetComponent<Image>().color;
		Player.AddListener(ToggleInteractivity);
	}

    private void Update()
    {
		if (Player.IsRecording)
		{
			// While the button is recording, shift the button color slightly
			float timeInterval = map(Mathf.Sin(Time.time * (1/LerpTime*Mathf.PI)), -1, 1, 0, 1);
			gameObject.GetComponent<Image>().color = Color.Lerp(RecordingColorLight, RecordingColorDark, timeInterval);
		}
		else gameObject.GetComponent<Image>().color = RecordButtonColor;
    }

	private float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
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
