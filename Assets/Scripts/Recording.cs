using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Recording : MonoBehaviour
{
	private AudioSource AudioSource;

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
}
