using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public int CurrentRecording = 0;
	public Recording[] Recordings;
	public Transcript TranscriptLabel;
	private float Timer;

	public Button RecordButton;
	private Color RecordButtonColor;
	public Color RecordingColor;

	private bool PlayerIsPlaying;
	private bool PlayerIsRecording;

	private void Start()
	{
		Timer = 0;

		// Grab the initial record button color
		if (RecordButton != null)
		{
			RecordButtonColor = RecordButton.GetComponent<Image>().color;
		}
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

		// Count up the timer if the player is playing the recording
		if (PlayerIsPlaying) Timer += Time.deltaTime;

		// Set the transcript text on the UI every frame
		TranscriptLabel.SetText(Recordings[CurrentRecording].Sentiments);
	}

    public void Play()
    {
		PlayerIsPlaying = true;
	}

	public void Rewind()
	{
		Timer = 0;
		PlayerIsPlaying = false;
		PlayerIsRecording = false;
		ResetButtonColor();

		// Reset all sentiments
		Recording.Sentiment[] Sentiments = Recordings[CurrentRecording].Sentiments;
		foreach (var sentiment in Sentiments)
		{
			sentiment.Current = false;
			sentiment.Recorded = false;
		}
	}

	private void ResetButtonColor()
	{
		// Toggle record button color
		if (RecordButton != null)
		{
			RecordButton.GetComponent<Image>().color = (PlayerIsRecording ? RecordingColor : RecordButtonColor);
		}
	}

	public void Record()
	{
		// Toggle recording state
		PlayerIsRecording = !PlayerIsRecording;

		// Toggle record button color
		ResetButtonColor();
	}

	public void Submit()
	{
		string recordedTranscript = "";
		// Grab the current sentiments that are recorded
		Recording.Sentiment[] Sentiments = Recordings[CurrentRecording].Sentiments;
		foreach (var sentiment in Sentiments)
		{
			if (sentiment.Recorded)
			{
				recordedTranscript += sentiment.Phrase + " ";
			}
		}

		// TODO
		print(recordedTranscript);
	}
}
