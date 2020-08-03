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
		public bool Played;
		public bool Recorded;
	}

	public string LevelResource;
	[HideInInspector]
	public Sentiment[] Sentiments;
	[HideInInspector]
    public string[] Solutions;

	private AudioSource AudioSource;

	void Start()
    {
		AudioSource = GetComponent<AudioSource>(); 
    }

	public void Play()
	{
        AudioSource.timeSamples = AudioSource.timeSamples;
        AudioSource.pitch = 1;
		AudioSource.Play();
	}

	public void Pause()
    {
		AudioSource.Pause();
    }

	public void Rewind()
	{
		AudioSource.timeSamples = IsAudioSourcePlaying() ? AudioSource.timeSamples : AudioSource.clip.samples - 1;
		AudioSource.pitch = -AudioReverseSpeed;
		AudioSource.Play();
	}

	public bool IsAudioSourcePlaying()
    {
		return AudioSource.isPlaying;
    }
}
