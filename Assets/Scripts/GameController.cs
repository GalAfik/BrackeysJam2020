using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public enum PlayerState { Paused, Playing, Recording, Done, Rewinding, Submitted };
	private PlayerState playerState = PlayerState.Paused;

	public Recording[] Recordings;
	public Transcript TranscriptLabel;
	private int CurrentRecording = 0;
	private float Timer = 0;

	public Button RecordButton;
	public Button PlayButton;
	public Sprite PlaySprite;
	public Sprite PauseSprite;
	private Color RecordButtonColor;
	public Color RecordingColor;

	private void Start()
	{
		var recordings = GameObject.FindGameObjectsWithTag("recording");
		foreach (var obj in recordings)
        {
			Recording recording = obj.GetComponent<Recording>();
			string jsonString = ((TextAsset)Resources.Load(recording.LevelResource)).text;
			JsonUtility.FromJsonOverwrite(jsonString, recording);
		}

		// Grab the initial record button color
		RecordButtonColor = RecordButton.GetComponent<Image>().color;
	}

	private void Update()
	{
		HandleInputs();
		UpdatePlayerState();

		ResetButtonColor();
		TranscriptLabel.SetText(Recordings[CurrentRecording].Sentiments);
	}

    public void Play()
    {
		if (playerState == PlayerState.Paused || playerState == PlayerState.Rewinding)
		{
			playerState = PlayerState.Playing;
			PlayButton.GetComponent<Image>().sprite = PauseSprite;
			Recordings[CurrentRecording].Play();
		}
		else if (playerState == PlayerState.Playing)
		{
			playerState = PlayerState.Paused;
			PlayButton.GetComponent<Image>().sprite = PlaySprite;
			Recordings[CurrentRecording].Pause();
		}
	}

	public void Rewind()
	{
		if (playerState != PlayerState.Playing &&
			playerState != PlayerState.Recording &&
			playerState != PlayerState.Done) return;

		playerState = PlayerState.Rewinding;
		PlayButton.GetComponent<Image>().sprite = PlaySprite;
		Recordings[CurrentRecording].Rewind();
	}

	public void Record()
	{
		if (playerState == PlayerState.Playing)
		{
			playerState = PlayerState.Recording;
		}
		else if (playerState == PlayerState.Recording)
		{
			playerState = PlayerState.Playing;
		}
	}

	public void Submit()
	{
		if (playerState != PlayerState.Done) return;

		playerState = PlayerState.Submitted;

		// Fade out the unrecorded text
		StartCoroutine(TranscriptLabel.FadeNonRecordedWords());

        Recording recording = Recordings[CurrentRecording];
        Recording.Sentiment[] sentiments = recording.Sentiments;

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

		switch (playerState)
		{
			case PlayerState.Playing:
				Timer += Time.deltaTime;
				if (sentiment != null)
                {
					sentiment.Played = true;
				}

				if (!recording.IsAudioSourcePlaying())
				{
					playerState = PlayerState.Done;
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
					playerState = PlayerState.Done;
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
					playerState = PlayerState.Paused;
					Timer = 0;
				}

				break;
		}
	}

	private Recording.Sentiment GetCurrentSentiment()
    {
		Recording.Sentiment[] sentiments = Recordings[CurrentRecording].Sentiments;
		for (int i = sentiments.Length; i --> 0;)
		{
			if (Timer >= sentiments[i].Timestamp)
			{
				return sentiments[i];
			}
		}
		return null;
	}

	private void ResetButtonColor()
	{
		RecordButton.GetComponent<Image>().color = playerState == PlayerState.Recording ? RecordingColor : RecordButtonColor;
	}
}
