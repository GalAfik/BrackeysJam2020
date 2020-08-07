using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Recording : MonoBehaviour
{
	public static float AudioReverseSpeed = 3.5f;
	public static float FastForwardSpeed = 2.5f;

	[System.Serializable]
	public class Sentiment
	{
		public int ID;
		public float Timestamp;
		public string Phrase;
		public bool Demoed;
		public bool Played;
		public bool Recorded;
	}

	public string LevelResource;
	public AudioClip AudioClip;
	public AudioClip EndingAudioClip;

	[HideInInspector]
	public Sentiment[] Sentiments;
	[HideInInspector]
    public int[] Solution;
	[HideInInspector]
	public string[] Hints;
	[HideInInspector]
	public string[] FailureMessages;
	[HideInInspector]
	public string EmptyFailureMessage;

	private Player Player;
	public AudioSource AudioSource { get; private set; }

	private void Start()
	{
		// Set the initial audio clip
		if (AudioClip != null) GetComponent<AudioSource>().clip = AudioClip;
	}

	private void Update()
	{
		Sentiment sentiment = GetCurrentSentiment();

		switch (Player.State)
		{
			case PlayerState.Demo:
				if (sentiment != null)
				{
					sentiment.Demoed = true;
				}
				goto case PlayerState.Playing;

			case PlayerState.FastForward:
			case PlayerState.Playing:
				if (sentiment != null)
				{
					sentiment.Played = true;
					sentiment.Recorded = sentiment.Recorded || Player.IsRecording;
				}

				if (!IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Done;
					AudioSource.timeSamples = AudioSource.clip.samples - 1;
				}

				break;

			case PlayerState.Rewinding:
				if (sentiment != null)
				{
					sentiment.Played = false;
					sentiment.Recorded = false;
				}

				if (!IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Ready;
                    AudioSource.timeSamples = 0;
                }

				break;
		}
	}

	public void Activate()
	{
		gameObject.SetActive(true);

		Player = Resources.Load<Player>("Player");
		Player.AddListener(ControlAudio);
		AudioSource = GetComponent<AudioSource>();
		AudioSource.timeSamples = 0;

		Recording recording = gameObject.GetComponent<Recording>();
		string jsonString = ((TextAsset)Resources.Load(recording.LevelResource)).text;
		JsonUtility.FromJsonOverwrite(jsonString, recording);

		foreach (var sentiment in Sentiments)
        {
			sentiment.Played = sentiment.Recorded = false;
        }
	}

	public void Deactivate()
	{
		Player.RemoveListener(ControlAudio);
		gameObject.SetActive(false);
	}

	private Sentiment GetCurrentSentiment()
	{
		for (int i = Sentiments.Length; i-- > 0;)
		{
			if (AudioSource.time >= Sentiments[i].Timestamp)
			{
				return Sentiments[i];
			}
		}
		return null;
	}

	private bool IsAudioSourcePlaying()
    {
		return AudioSource.isPlaying;
    }

	private void ControlAudio(PlayerState newState, PlayerState oldState)
    {
		if (newState == PlayerState.Demo ||
			newState == PlayerState.Playing ||
			newState == PlayerState.FastForward)
		{
			Play();
		}
		else if (newState == PlayerState.Paused)
        {
			Pause();
        }
		else if (newState == PlayerState.Rewinding)
        {
			Rewind();
        }
		else if (newState == PlayerState.Done)
		{
			AudioSource.pitch = 1;
		}
	}

	private void Play()
	{
        AudioSource.timeSamples = AudioSource.timeSamples;
        if (Player.State == PlayerState.Playing) AudioSource.pitch = 1;
		else if (Player.State == PlayerState.FastForward) AudioSource.pitch = FastForwardSpeed;
		AudioSource.Play();
	}

	private void Pause()
	{
		AudioSource.Pause();
	}

	private void Rewind()
	{
        AudioSource.timeSamples = AudioSource.timeSamples;
        AudioSource.pitch = -AudioReverseSpeed;
		AudioSource.Play();
	}

	public void PlayEndingAudioClip()
	{
		if (AudioClip != null) GetComponent<AudioSource>().clip = EndingAudioClip;
		AudioSource.timeSamples = 0;
		AudioSource.Play();
	}
}
