using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IMenuState
{
	void Enter(Animator animator);
	void Exit(Animator animator);
}

public class TitleScreenState : IMenuState
{
	public void Enter(Animator animator)
	{
	}

	public void Exit(Animator animator)
	{
	}
}

public class SettingsState : IMenuState
{
	public void Enter(Animator animator)
	{
		animator.SetBool("Settings", true);
	}

	public void Exit(Animator animator)
	{
		animator.SetBool("Settings", false);
	}
}

public class LevelSelectState : IMenuState
{
	public void Enter(Animator animator)
	{
		animator.SetBool("Levels", true);
	}

	public void Exit(Animator animator)
	{
		animator.SetBool("Levels", false);
	}
}

public class MenuController : MonoBehaviour
{
	public string WebsiteURL;

	// Set up a state machine for the main menu
	public IMenuState CurrentState;
	public TitleScreenState TitleState = new TitleScreenState();
	public SettingsState SettingsState = new SettingsState();
	public LevelSelectState LevelsState = new LevelSelectState();

	public Animator MenuAnimator;

	public Texture SoundOnSprite;
	public Texture SoundOffSprite;

	public RawImage ToggleMusicIcon;
	private bool Music = true;
	public RawImage ToggleSFXIcon;
	private bool SFX = true;
	public RawImage ToggleSpeechIcon;
	private bool Speech = true;

	private void Start()
	{
		// Set the initial state
		SetState(TitleState);

		// Start the main theme
		FindObjectOfType<AudioManager>().Play(Sound.Category.Theme);
	}

	public void ReturnToMenu()
    {
		// Fade in the theme
		AudioManager AM = FindObjectOfType<AudioManager>();
		AM.StartFade(Sound.Category.Theme, .5f, true);
	}

	public void SetState(IMenuState state)
	{
		CurrentState?.Exit(MenuAnimator);
		CurrentState = state;
		CurrentState?.Enter(MenuAnimator);
	}

	public void ToggleSettingsMenu()
	{
		// Go to the settings menu
		if (CurrentState == TitleState) SetState(SettingsState);
		else if (CurrentState == SettingsState) SetState(TitleState);
	}

	public void ToggleCredits()
	{
		// Fade into the credits
		MenuAnimator.SetTrigger("Credits");
	}

	public void ToggleLevelSelect()
	{
		// Go to the level select screen
		if (CurrentState == TitleState) SetState(LevelsState);
		else if (CurrentState == LevelsState) SetState(TitleState);
	}

	public void LoadLevel(Level level)
	{
		FindObjectOfType<LevelUnlockController>().CurrentLevel = level;
		StartCoroutine(LoadScene(level.GetComponent<ScenePicker>().scenePath));
	}

	private IEnumerator LoadScene(string sceneName)
	{
		// Fade out the theme audio
		AudioManager AM = FindObjectOfType<AudioManager>();
		AM.StartFade(Sound.Category.Theme, .5f, false);

		FindObjectOfType<FadeCanvas>().FadeOut();
		yield return new WaitForSecondsRealtime(1);
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
	}

	public static void Quit()
	{
		Application.Quit();
	}

	public void ToggleMusic()
	{
		Music = !Music;
		ToggleMusicIcon.texture = (Music ? SoundOnSprite : SoundOffSprite);
		FindObjectOfType<AudioManager>().ToggleMusic();
	}

	public void ToggleSFX()
	{
		SFX = !SFX;
		ToggleSFXIcon.texture = (SFX ? SoundOnSprite : SoundOffSprite);
		FindObjectOfType<AudioManager>().ToggleSFX();
	}

	public void ToggleSpeech()
	{
		Speech = !Speech;
		ToggleSpeechIcon.texture = (Speech ? SoundOnSprite : SoundOffSprite);
		FindObjectOfType<AudioManager>().ToggleSpeech();
	}

	public void Website()
	{
		Application.OpenURL(WebsiteURL);
	}
}
