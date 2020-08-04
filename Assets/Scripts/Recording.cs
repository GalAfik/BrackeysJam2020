using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Recording : MonoBehaviour
{
	public static float AudioReverseSpeed = 3.5f;

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
	[HideInInspector]
	public Sentiment[] Sentiments;
	[HideInInspector]
    public string[] Solutions;

	private Player Player;
	private AudioSource AudioSource;
	private float Timer = 0;

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

			case PlayerState.Playing:
				Timer += Time.deltaTime;
				if (sentiment != null)
				{
					sentiment.Played = true;
					sentiment.Recorded = Player.IsRecording;
				}

				if (!IsAudioSourcePlaying())
				{
					Player.State = PlayerState.Done;
				}

				// Audio pitch should always be set to timescale if the player is playing
				AudioSource.pitch = Time.timeScale;

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

		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
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
		if (newState == PlayerState.Demo)
		{
			Play();
		}
		if (newState == PlayerState.Playing)
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
	}

	private void Play()
	{
		AudioSource.timeSamples = AudioSource.timeSamples;
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
		AudioSource.Play();
	}
}
