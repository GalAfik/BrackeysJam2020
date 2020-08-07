using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip BackgroundTrack;
	public AudioClip[] ButtonSFX;
	public AudioClip[] TapeSFX;
	public AudioClip[] CatSFX;

	public enum SFX { Button, Tape, Cat }

	private AudioSource BackgroundAudioSource;
	private AudioSource SFXAudioSource;

	private void Start()
	{
		BackgroundAudioSource = new AudioSource();
		SFXAudioSource = new AudioSource();
	}

	public void Play(SFX soundEffect)
	{
		try
		{
			int randomIndex;
			switch (soundEffect)
			{
				case SFX.Button:
					randomIndex = Random.Range(0, ButtonSFX.Length - 1);
					AudioSource.PlayClipAtPoint(ButtonSFX[randomIndex], Camera.main.transform.position);
					break;

				case SFX.Tape:
					randomIndex = Random.Range(0, TapeSFX.Length - 1);
					AudioSource.PlayClipAtPoint(TapeSFX[randomIndex], Camera.main.transform.position);
					break;

				case SFX.Cat:
					randomIndex = Random.Range(0, CatSFX.Length - 1);
					AudioSource.PlayClipAtPoint(CatSFX[randomIndex], Camera.main.transform.position);
					break;
			}
		}
		catch (System.Exception)
		{
			throw;
		}
	}

	public void PlayMusic()
	{
		AudioSource.PlayClipAtPoint(BackgroundTrack, Camera.main.transform.position);
	}

	public void ToggleMusic()
	{
		BackgroundAudioSource.mute = !BackgroundAudioSource.mute;
	}

	public void ToggleSFX()
	{
		SFXAudioSource.mute = !SFXAudioSource.mute;
	}

	public void ToggleSpeech()
	{
		//BackgroundAudioSource.mute = !BackgroundAudioSource.mute;
	}
}
