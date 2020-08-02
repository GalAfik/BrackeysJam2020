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
		if (Input.GetButtonDown("Play")) Play();
		if (Input.GetButtonDown("Rewind")) Rewind();
		if (Input.GetButtonDown("Record")) Record();
		if (Input.GetButtonDown("Submit")) Submit();

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

    public void Play()
    {
		print("play");
    }

	public void Rewind()
	{
		print("rewind");
	}

	public void Record()
	{
		PlayerIsRecording = !PlayerIsRecording;
	}

	public void Submit()
	{
		print("submit");
	}
}
