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
	private float Timer = 0;

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
				Timer += Time.deltaTime;
				if (sentiment != null)
				{
					sentiment.Demoed = true;
					sentiment.Played = true;
				}

				if (!IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Done;
				}

				break;

			case PlayerState.FastForward:
				Timer += Time.deltaTime * (FastForwardSpeed - 1);
				goto case PlayerState.Playing;

			case PlayerState.Playing:
				Timer += Time.deltaTime;
				if (sentiment != null)
				{
					sentiment.Played = true;
					sentiment.Recorded = sentiment.Recorded || Player.IsRecording;
				}

				if (!IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Done;
				}

				break;

			case PlayerState.Rewinding:
				Timer -= Time.deltaTime * AudioReverseSpeed;
				if (sentiment != null)
				{
					sentiment.Played = false;
					sentiment.Recorded = false;
				}

				if (Timer < 0)
				{
					Player.State = PlayerState.Ready;
					Timer = 0;
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
		Timer = 0;

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
			if (Timer >= Sentiments[i].Timestamp)
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
		// Mute the speech source if required
		AudioSource.mute = !FindObjectOfType<AudioManager>().SpeechEnabled;
		// Play the speech recording
		AudioSource.Play();
	}

	private void Pause()
	{
		AudioSource.Pause();
	}

	private void Rewind()
	{
		AudioSource.timeSamples = AudioSource.timeSamples == 0 ? AudioSource.clip.samples - 1 : AudioSource.timeSamples;
		AudioSource.pitch = -AudioReverseSpeed;
		// Mute the speech source if required
		AudioSource.mute = !FindObjectOfType<AudioManager>().SpeechEnabled;
		AudioSource.Play();
	}

	public void PlayEndingAudioClip()
	{
		if (AudioClip != null) GetComponent<AudioSource>().clip = EndingAudioClip;
		AudioSource.timeSamples = 0;
		// Mute the speech source if required
		AudioSource.mute = !FindObjectOfType<AudioManager>().SpeechEnabled;
		AudioSource.Play();
	}
}
