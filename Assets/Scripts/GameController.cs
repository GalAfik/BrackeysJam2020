using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public Recording[] Recordings;
	public Transcript TranscriptLabel;
	
	private Level Level;
	private Player Player;
	private int CurrentRecording = 0;
	private float Timer = 0;

	private void Start()
	{
		Level = AssetDatabase.LoadAssetAtPath<Level>("Assets/States/Level.asset");
		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
		Level.Sentiments = Recordings[CurrentRecording].Sentiments;
		Player.State = PlayerState.Paused;
	}

	private void Update()
	{
		HandleInputs();
		UpdatePlayerState();
	}

    public void Play()
    {
		if (Player.State == PlayerState.Paused || Player.State == PlayerState.Rewinding)
		{
			Player.State = PlayerState.Playing;
			Recordings[CurrentRecording].Play();
		}
		else if (Player.State == PlayerState.Playing || Player.State == PlayerState.Recording)
		{
			Player.State = PlayerState.Paused;
			Recordings[CurrentRecording].Pause();
		}
	}

	public void Rewind()
	{
		if (Player.State != PlayerState.Playing &&
			Player.State != PlayerState.Recording &&
			Player.State != PlayerState.Done) return;

		Player.State = PlayerState.Rewinding;
		Recordings[CurrentRecording].Rewind();
	}

	public void Record()
	{
		if (Player.State == PlayerState.Playing)
		{
			Player.State = PlayerState.Recording;
		}
		else if (Player.State == PlayerState.Recording)
		{
			Player.State = PlayerState.Playing;
		}
	}

	public void Submit()
	{
		if (Player.State != PlayerState.Done) return;

		Player.State = PlayerState.Submitted;

		// Fade out the unrecorded text
		StartCoroutine(TranscriptLabel.FadeNonRecordedWords());

        Recording recording = Recordings[CurrentRecording];
        Recording.Sentiment[] sentiments = Level.Sentiments;

        string attempt = string.Join(",", sentiments.Where(sentiment => sentiment.Recorded).Select(sentiment => sentiment.ID));
        bool solved = recording.Solutions.Contains(attempt);

        // TODO
        print(solved ? "Success!" : "Try again");
    }

	private void HandleInputs()
    {
		if (Input.GetButtonDown("Play")) Play();
		if (Input.GetButtonDown("Rewind")) Rewind();
		if (Input.GetButtonDown("Record")) Record();
		if (Input.GetButtonDown("Submit")) Submit();
	}

	private void UpdatePlayerState()
    {
		Recording recording = Recordings[CurrentRecording];
		Recording.Sentiment sentiment = GetCurrentSentiment();

		switch (Player.State)
		{
			case PlayerState.Playing:
				Timer += Time.deltaTime;
				if (sentiment != null)
                {
					sentiment.Played = true;
				}

				if (!recording.IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Done;
				}

				break;
			case PlayerState.Recording:
				Timer += Time.deltaTime;
				if (sentiment != null)
                {
					sentiment.Played = true;
					sentiment.Recorded = true;
				}

				if (!recording.IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Done;
				}

				break;
			case PlayerState.Rewinding:
				Timer -= Time.deltaTime * Recording.AudioReverseSpeed;
				if (sentiment != null)
				{
					sentiment.Played = false;
					sentiment.Recorded = false;
				}

				if (Timer < 0)
				{
					Player.State = PlayerState.Paused;
					Timer = 0;
				}

				break;
		}
	}

	private Recording.Sentiment GetCurrentSentiment()
    {
		Recording.Sentiment[] sentiments = Level.Sentiments;
		for (int i = sentiments.Length; i --> 0;)
		{
			if (Timer >= sentiments[i].Timestamp)
			{
				return sentiments[i];
			}
		}
		return null;
	}
}
