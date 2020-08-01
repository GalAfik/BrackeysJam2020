using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public int CurrentRecording = 0;
	public Recording[] Recordings;
	public Transcript TranscriptLabel;
	private float Timer;
	private bool PlayerIsRecording;

	private void Start()
	{
		Timer = 0;
	}

	// Update is called once per frame
	void Update()
    {
		// Handle recording input
		if (Input.GetKeyDown(KeyCode.Space)) PlayerIsRecording = !PlayerIsRecording;

		// Grab the current sentiments
		Recording.Sentiment[] Sentiments = Recordings[CurrentRecording].Sentiments;
		foreach (var sentiment in Sentiments)
		{
			// Change the color of the sentiment phrase depending on if it is being recorded or has already been played
			if (Timer >= sentiment.Timestamp && sentiment.Current == false)
			{
				sentiment.Current = true;
				if (PlayerIsRecording) sentiment.Recorded = true;
			}
		}

		// Count up the timer
		Timer += Time.deltaTime;

		// Set the transcript text on the UI every frame
		TranscriptLabel.SetText(Recordings[CurrentRecording].Sentiments);
	}
}
