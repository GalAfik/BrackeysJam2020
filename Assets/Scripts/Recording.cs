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
	public AudioClip ReverseClip;
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
		int sentimentIndex = GetCurrentSentiment();

		switch (Player.State)
		{
			case PlayerState.Demo:
				if (sentimentIndex >= 0)
				{
					Sentiments[sentimentIndex].Demoed = true;
				}
				goto case PlayerState.Playing;

			case PlayerState.FastForward:
			case PlayerState.Playing:
				if (sentimentIndex >= 0)
				{
					Sentiments[sentimentIndex].Played = true;
					Sentiments[sentimentIndex].Recorded = Sentiments[sentimentIndex].Recorded || Player.IsRecording;
				}

				if (!IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Done;
					AudioSource.timeSamples = AudioSource.clip.samples - 1;
				}

				break;

			case PlayerState.Rewinding:
				if (sentimentIndex >= 0)
				{
					for (int i = sentimentIndex; i < Sentiments.Length; i++)
					{
						Sentiments[sentimentIndex].Played = false;
						Sentiments[sentimentIndex].Recorded = false;
					}
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

	private int GetCurrentSentiment()
	{
		for (int i = Sentiments.Length; i-- > 0;)
		{
			float currentTime = AudioSource.time;
			if (Player.State == PlayerState.Rewinding)
            {
				currentTime = AudioSource.clip.length - currentTime;
            }
			if (currentTime >= Sentiments[i].Timestamp)
			{
				return i;
			}
		}
		return -1;
	}

	private bool IsAudioSourcePlaying()
    {
		return AudioSource.isPlaying;
    }

	private void ControlAudio(PlayerState newState, PlayerState oldState)
    {
		if (newState == PlayerState.Rewinding)
        {
			int current = AudioSource.clip.samples - AudioSource.timeSamples;
			GetComponent<AudioSource>().clip = ReverseClip;
			AudioSource.timeSamples = current;
			AudioSource.pitch = AudioReverseSpeed;
		}
		else if (oldState == PlayerState.Rewinding)
        {
			int current = AudioSource.clip.samples - AudioSource.timeSamples;
			GetComponent<AudioSource>().clip = AudioClip;
			AudioSource.timeSamples = current;
			AudioSource.pitch = 1;
		}

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
