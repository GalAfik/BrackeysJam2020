using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Recording : MonoBehaviour
{
	public static float AudioReverseSpeed = 3.5f;
	private AudioSource AudioSource;

	[System.Serializable]
	public class Sentiment
	{
		public float Timestamp;
		public string Phrase;
		public bool Played;
		public bool Recorded;
	}
	public Sentiment[] Sentiments;

    void Start()
    {
		AudioSource = GetComponent<AudioSource>(); 
    }

	public void Play()
	{
		AudioSource.timeSamples = 0;
		AudioSource.pitch = 1;
		AudioSource.Play();
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
