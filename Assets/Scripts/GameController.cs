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
		Player.AddListener(OnSubmit);

		// Set the UI phrases required textbox
		FindObjectOfType<GameUserInterface>()?.SetPhrasesRequiredText(Recording.Solution.Length);
	}

	public IEnumerator StartLevel()
	{
		yield return new WaitForSeconds(1);

		Player.State = PlayerState.Demo;
	}

	private void Update()
	{
		HandleInputs();
	}

	public void Exit()
	{
		FindObjectOfType<FadeCanvas>().FadeOut();
		// Exit to level select
		Invoke("GoToLevelMenu", 1);
	}

	private void GoToLevelMenu()
    {
		MenuController menuController = FindObjectOfType<MenuController>();
		menuController.SetState(menuController.LevelsState);
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(gameObject.scene.name));
		Menu.transform.position = new Vector3(0, 0, 0);
		Menu.transform.Find("UI").GetComponent<Canvas>().enabled = true;
		FindObjectOfType<FadeCanvas>()?.FadeIn();
	}

	private void HandleInputs()
	{
		if (Input.GetButtonDown("Exit")) Exit();
		if (Input.GetButtonUp("FastForward")) Player.FastForward();

		if (Player.State == PlayerState.Demo ||
			Player.State == PlayerState.Off ||
			Player.State == PlayerState.Submitted) return;

		if (Input.GetButtonDown("Play")) Player.Play();
		if (Input.GetButtonDown("Rewind")) Player.Rewind();
		if (Input.GetButtonDown("Record")) Player.Record();
		if (Input.GetButtonUp("Record")) Player.Record();
		if (Input.GetButtonDown("Submit")) Player.Submit();
		if (Input.GetButtonDown("FastForward")) Player.FastForward();
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

		// Play the guilty recording
		Player.Recording.PlayEndingAudioClip();

		// Wait for the audio clip to finish
		yield return new WaitForSecondsRealtime(Player.Recording.AudioSource.clip.length - 1);

		// Show the newspaper
		FindObjectOfType<Newspaper>().Display();

		yield return new WaitForSecondsRealtime(5);

		// Go back to the level select screen
		Exit();
	}
}
