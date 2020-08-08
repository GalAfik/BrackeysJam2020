using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class AudioManager : MonoBehaviour
{
	public Sound[] Sounds;
	public static AudioManager Instance;

	private Player Player;
	[HideInInspector]
	public bool SpeechEnabled = true;

	private int CatCount;

	private void Awake()
	{
		// Persistent
		if (Instance == null) Instance = this;
		else
		{
			Destroy(gameObject);
			return;
		}

		// Create an audio source for each sound
		foreach (Sound sound in Sounds)
		{
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.source.clip = sound.clip;
			sound.source.volume = sound.volume;
			sound.source.pitch = sound.pitch;
			sound.source.loop = sound.loop;
			sound.source.mute = sound.mute;
		}
	}

	private void Start()
	{
		Player = Resources.Load<Player>("Player");
		Player.AddListener(OnStateChange);
	}

	public void Play(string name)
	{
		Sound sound = Array.Find(Sounds, s => s.name == name);
		if (sound == null)
		{
			Debug.Log("Sound: " + name + " not found!");
			return;
		}
		Debug.Assert(sound.source != null);
		sound.source.Play();
	}

	public void Play(Sound.Category category)
	{
		// Play a special cat sound on every fourth click
		if (category == Sound.Category.Cat)
			CatCount++;
		if (CatCount >= UnityEngine.Random.Range(4, 6))
		{
			Play("cat9");
			CatCount = 0;
		}
		else
		{
			// Cat mode
			if (FindObjectOfType<Catvas>().Cat)
			{
				// Make all button presses cat sounds
				if (category == Sound.Category.Button ||
					category == Sound.Category.MenuButton ||
					category == Sound.Category.Pop)
				{
					category = Sound.Category.Cat;
				}
			}
			// Play sounds normally
			Play(GetRandomSound(category).name);
		}
	}

	public void PlayButtonSound()
	{
		FindObjectOfType<AudioManager>().Play(Sound.Category.Button);
	}

	public void PlayMenuButtonSound()
	{
		FindObjectOfType<AudioManager>().Play(Sound.Category.MenuButton);
	}

	private Sound GetRandomSound(Sound.Category category)
	{
		// Get all sounds that match the category
		Sound[] matchingSounds = Sounds.Where(sound => sound.soundCategory == category).ToArray();

		if (matchingSounds.Length <= 0) return null;

		// Pick a sound at random
		int randomIndex = UnityEngine.Random.Range(0, matchingSounds.Length);
		return matchingSounds[randomIndex];
	}

	private void OnStateChange(PlayerState newState, PlayerState oldState)
	{
		// Play the tape sound at the start and end of each recording
		if (newState == PlayerState.Demo ||
			newState == PlayerState.Ready ||
			newState == PlayerState.Done)
		{
			Play(Sound.Category.Tape);
		}
	}

	public void ToggleMusic()
	{
		foreach (Sound sound in Sounds)
		{
			if (sound.soundCategory == Sound.Category.Theme ||
				sound.soundCategory == Sound.Category.LevelTheme)
			{
				sound.mute = !sound.mute;
				sound.source.mute = sound.mute;
			}

			// Pause / Play the song
			if (sound.soundCategory == Sound.Category.Theme)
			{
				if (sound.mute) sound.source.Pause();
				else sound.source.Play();
			}
		}
	}

	public void ToggleSFX()
	{
		foreach (Sound sound in Sounds)
		{
			if (sound.soundCategory == Sound.Category.Button ||
				sound.soundCategory == Sound.Category.MenuButton ||
				sound.soundCategory == Sound.Category.Pop ||
				sound.soundCategory == Sound.Category.Cat)

				sound.mute = !sound.mute;
		}
	}

	public void ToggleSpeech()
	{
		SpeechEnabled = !SpeechEnabled;
	}

	public float GetInitialVolume(string name)
	{
		return Sounds.Single(sound => sound.name == name).volume;
	}

	public void SetAudioPitch(Sound.Category category, float pitch)
    {
		foreach (Sound sound in Sounds)
		{
			if (sound.soundCategory == category)
			{
				AudioSource audioSource = sound.source;
				audioSource.pitch = pitch;
			}
		}
	}

	public void StartFade(Sound.Category category, float duration, bool isFadeIn)
	{
		// Pick a random audio track to fade in/out
		Sound[] soundsMatchingCategory = Sounds.Where(sound => sound.soundCategory == category).ToArray();
		int randomIndex = UnityEngine.Random.Range(0, soundsMatchingCategory.Length);

		if (isFadeIn)
		{
			Sound randomSound = soundsMatchingCategory[randomIndex];
			if (isFadeIn) StartCoroutine(StartFade(randomSound.name, duration, 0, randomSound.volume));
			else StartCoroutine(StartFade(randomSound.name, duration, randomSound.volume, 0));
		}
		else
		{
			foreach (Sound sound in soundsMatchingCategory)
			{
				StartCoroutine(StartFade(sound.name, duration, sound.volume, 0));
			}
		}
	}

	public IEnumerator StartFade(string name, float duration, float startVolume, float targetVolume)
	{
		float currentTime = 0;
		AudioSource audioSource = Sounds.Single(sound => sound.name == name).source;
		float start = startVolume;

		// Start playing the audiosource if it was silent before
		if (startVolume == 0) audioSource.Play();

		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
			yield return null;
		}

		// Stop the audiosource if it gets to 0
		if (targetVolume == 0) audioSource.Stop();
		yield break;
	}
}
