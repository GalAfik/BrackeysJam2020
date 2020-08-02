using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Recording : MonoBehaviour
{
	private AudioSource AudioSource;
	public float AudioReverseSpeed = 3.5f;

	[System.Serializable]
	public class Sentiment
	{
		public float Timestamp;
		public string Phrase;
		public bool Current;
		public bool Recorded;
	}
	public Sentiment[] Sentiments;

    // Start is called before the first frame update
    void Start()
    {
		AudioSource = GetComponent<AudioSource>(); 
    }

	public void Play()
	{
		if (AudioSource != null)
		{
			AudioSource.timeSamples = 0;
			AudioSource.pitch = 1;
			AudioSource.Play();
		}
	}

	public void Rewind()
	{
		if (AudioSource != null)
		{
			// If the audio has reached the end, start at the end
			if (AudioSource.timeSamples == 0) AudioSource.timeSamples = AudioSource.clip.samples - 1;
			else AudioSource.timeSamples = AudioSource.timeSamples; // starting position in track - Not sure why this works, possibly an audio buffer reset error?
			AudioSource.pitch = -AudioReverseSpeed; // pitch changes direction
			AudioSource.Play();
		}
	}
}
