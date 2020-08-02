using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public enum PlayerState { Ready, Playing, Recording, Done, Rewinding, Submitted };
	private PlayerState playerState = PlayerState.Ready;

	public Recording[] Recordings;
	public Transcript TranscriptLabel;
	private int CurrentRecording = 0;
	private float Timer = 0;

	public Button RecordButton;
	private Color RecordButtonColor;
	public Color RecordingColor;

	private void Start()
	{
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
		if (playerState != PlayerState.Ready) return;

		playerState = PlayerState.Playing;
		Recordings[CurrentRecording].Play();
	}

	public void Rewind()
	{
		if (playerState != PlayerState.Playing &&
			playerState != PlayerState.Recording &&
			playerState != PlayerState.Done) return;

		playerState = PlayerState.Rewinding;
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

		string recordedTranscript = "";
		// Grab the current sentiments that are recorded
		Recording.Sentiment[] Sentiments = Recordings[CurrentRecording].Sentiments;
		foreach (var sentiment in Sentiments)
		{
			if (!sentiment.Played)
			{
				return;
			}
			if (sentiment.Recorded)
			{
				recordedTranscript += sentiment.Phrase + " ";
			}
		}

		// Fade out the unrecorded text
		StartCoroutine(TranscriptLabel.FadeNonRecordedWords());

		// TODO
		print(recordedTranscript);
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
					playerState = PlayerState.Ready;
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
