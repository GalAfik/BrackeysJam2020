using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]

public class Sound
{
	public string name;
	public enum Category { Theme, Button, MenuButton, Tape, Cat, LevelTheme, Pop }
	public Category soundCategory;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = .2f;
	[Range(.1f, 3f)]
	public float pitch = 1f;

	public bool loop;
	public bool mute;

	[HideInInspector]
	public AudioSource source;
}
