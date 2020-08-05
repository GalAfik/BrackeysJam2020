using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IMenuState
{
	void Enter(Animator animator);
	void Exit(Animator animator);
}

public class TitleScreenState : IMenuState
{
	public void Enter(Animator animator)
	{
		MonoBehaviour.FindObjectOfType<FadeCanvas>()?.FadeIn();
	}

	public void Exit(Animator animator)
	{
		MonoBehaviour.FindObjectOfType<FadeCanvas>()?.FadeOut();
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
		MonoBehaviour.FindObjectOfType<FadeCanvas>()?.FadeIn();
		animator.SetBool("Levels", true);
	}

	public void Exit(Animator animator)
	{
		animator.SetBool("Levels", false);
	}
}

public class MenuController : MonoBehaviour
{
	private static MenuController self;

	public string WebsiteURL;

	// Set up a state machine for the main menu
	public IMenuState CurrentState;
	public TitleScreenState TitleState = new TitleScreenState();
	public SettingsState SettingsState = new SettingsState();
	public LevelSelectState LevelsState = new LevelSelectState();

	public Animator MenuAnimator;

	private void Start()
	{
		// Set the initial state
		SetState(TitleState);
	}

	private void Awake()
	{
		if (self == null)
		{
			self = this;
			return;
		}
		Destroy(gameObject);
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

	public static void LoadLevel(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	public static void Quit()
	{
		Application.Quit();
	}

	public static void ToggleMuteMusic()
	{

	}

	public void Website()
	{
		Application.OpenURL(WebsiteURL);
	}
}
