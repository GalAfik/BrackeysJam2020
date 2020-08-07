using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	public MessageSystem MessageSystem;
	public Recording Recording;

	private Player Player;
	private Menu Menu;

	private int SubmissionsAttempts;
	public int HintThreshold = 3;
	public Texture SplashScreenImage;
	public SplashScreen SplashScreen;
	public Texture NewspaperImage;
	public Newspaper Newspaper;

	private void Start()
	{
		// Set the splash screen and newspaper image
		if (SplashScreen != null && SplashScreenImage != null) SplashScreen.Image.texture = SplashScreenImage;
		if (Newspaper != null && NewspaperImage != null) Newspaper.Image.texture = NewspaperImage;

		FindObjectOfType<FadeCanvas>()?.FadeIn();

		// Get the Menu object to move away
		Menu = FindObjectOfType<Menu>();
		if (Menu != null)
		{
			Menu.transform.position = new Vector3(-1000, 0, 0);
			Menu.transform.Find("UI").GetComponent<Canvas>().enabled = false;
		}

		Player = Resources.Load<Player>("Player");
		Player.Reset(Recording, true);
		Player.AddListener(ReverseAudio);
		Player.AddListener(OnSubmit);

		// Set the UI phrases required textbox
		FindObjectOfType<GameUserInterface>()?.SetPhrasesRequiredText(Recording.Solution.Length);
	}

	public IEnumerator StartLevel()
	{
		// Fade in the theme
		AudioManager AM = FindObjectOfType<AudioManager>();
		AM.StartFade(Sound.Category.LevelTheme, .5f, true);

		yield return new WaitForSeconds(1);

		Player.State = PlayerState.Demo;
	}

	public void PlayButtonSound()
    {
		AudioManager AM = FindObjectOfType<AudioManager>();
		AM.PlayButtonSound();
	}

	public void PlayMenuButtonSound()
	{
		AudioManager AM = FindObjectOfType<AudioManager>();
		AM.PlayMenuButtonSound();
	}

	private void Update()
	{
		HandleInputs();
	}

	private void HandleInputs()
	{
		if (Input.GetButtonDown("Exit")) Exit();

		if (Player.State == PlayerState.Demo ||
			Player.State == PlayerState.Off ||
			Player.State == PlayerState.Submitted) return;

		if (Input.GetButtonDown("Play")) Player.Play();
		if (Input.GetButtonDown("Record")) Player.Record();
		if (Input.GetButtonUp("Record")) Player.Record();
		if (Input.GetButtonDown("Rewind")) Player.Rewind();
		if (Input.GetButtonUp("Rewind")) Player.Play();
		if (Input.GetButtonDown("FastForward")) Player.FastForward();
		if (Input.GetButtonUp("FastForward")) Player.Play();
		if (Input.GetButtonDown("Submit")) Player.Submit();

		if (Input.GetButtonDown("Play") ||
			Input.GetButtonDown("Record") ||
			Input.GetButtonUp("Record") ||
			Input.GetButtonDown("Rewind") ||
			Input.GetButtonUp("Rewind") ||
			Input.GetButtonDown("FastForward") ||
			Input.GetButtonUp("FastForward")) PlayButtonSound();
		if (Input.GetButtonDown("Submit")) PlayMenuButtonSound();
	}

	private void ReverseAudio(PlayerState newState, PlayerState oldState)
    {
		AudioManager AM = FindObjectOfType<AudioManager>();
		if (newState == PlayerState.Rewinding)
        {
			AM.SetAudioPitch(Sound.Category.LevelTheme, -1);
			return;
		}
		AM.SetAudioPitch(Sound.Category.LevelTheme, 1);
	}

	private void OnSubmit(PlayerState newState, PlayerState oldState)
	{
		if (newState == PlayerState.Submitted)
		{
			SubmissionsAttempts++;
			int[] attempt = Player.Recording.Sentiments.Where(sentiment => sentiment.Recorded).Select(sentiment => sentiment.ID).ToArray<int>();
			bool solved = Player.Recording.Solution.SequenceEqual<int>(attempt);

			// Grade the player
			if (!solved)
			{
				if (SubmissionsAttempts >= HintThreshold)
				{
					// Display a hint message to the player
					int randomIndex = Random.Range(0, Player.Recording.Hints.Length - 1);
					MessageSystem?.DisplayMessage(Player.Recording.Hints[randomIndex]);
				}
				else if (attempt.Length == 0) MessageSystem?.DisplayMessage(Player.Recording.EmptyFailureMessage);
				else
				{
					int randomIndex = Random.Range(0, Player.Recording.FailureMessages.Length - 1);
					MessageSystem?.DisplayMessage(Player.Recording.FailureMessages[randomIndex]);
				}
			}
			else
			{
				// WINNER WINNER CHICKEN DINNER
				StartCoroutine(FinishLevel());
			}
		}
	}

	private IEnumerator FinishLevel()
	{
		// Mark level as complete
		FindObjectOfType<LevelUnlockController>()?.CurrentLevel.CompleteLevel();

		yield return new WaitForSecondsRealtime(.5f);

		// Fade all non-recorded text
		StartCoroutine(FindObjectOfType<Transcript>()?.FadeOutNonRecordedWords(1.5f));

		if (FindObjectOfType<AudioManager>().SpeechEnabled)
		{
			// Play the guilty recording
			Player.Recording.PlayEndingAudioClip();

			// Wait for the audio clip to finish
			yield return new WaitForSecondsRealtime(Player.Recording.AudioSource.clip.length - 1);
		}

		// Show the newspaper
		FindObjectOfType<Newspaper>().Display();

		yield return new WaitForSecondsRealtime(5);

		// Go back to the level select screen
		Exit();
	}


	private void Exit()
	{
		// Fade out the theme
		AudioManager AM = FindObjectOfType<AudioManager>();
		AM.SetAudioPitch(Sound.Category.LevelTheme, 1);
		AM.StartFade(Sound.Category.LevelTheme, .5f, false);
		StartCoroutine(FadeRecording(.5f, 1, 0));

		FindObjectOfType<FadeCanvas>().FadeOut();
		// Exit to level select
		Invoke("GoToLevelMenu", 1);
	}

	private IEnumerator FadeRecording(float duration, float startVolume, float targetVolume)
	{
		float currentTime = 0;
		AudioSource audioSource = Player.Recording.AudioSource;
		float start = startVolume;

		while (currentTime < duration)
		{
			currentTime += Time.deltaTime;
			audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
			yield return null;
		}

		yield break;
	}

	private void GoToLevelMenu()
	{
		MenuController menuController = FindObjectOfType<MenuController>();
		menuController.SetState(menuController.LevelsState);
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(gameObject.scene.name));
		Menu.transform.position = new Vector3(0, 0, 0);
		Menu.transform.Find("UI").GetComponent<Canvas>().enabled = true;
		FindObjectOfType<FadeCanvas>()?.FadeIn();
		menuController.ReturnToMenu();
	}
}
