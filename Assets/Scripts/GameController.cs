﻿using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	public MessageSystem MessageSystem;
	public Recording Recording;

	public string EmptyFailureMessage;
	public string[] FailureMessages;

	private Player Player;
	private Menu Menu;

	private void Start()
	{
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

		if (Player.State == PlayerState.Demo ||
			Player.State == PlayerState.Off ||
			Player.State == PlayerState.Submitted) return;

		if (Input.GetButtonDown("Play")) Player.Play();
		if (Input.GetButtonDown("Rewind")) Player.Rewind();
		if (Input.GetButtonDown("Record")) Player.Record();
		if (Input.GetButtonDown("Submit")) Player.Submit();
		if (Input.GetButtonDown("FastForward")) Player.FastForward(true);
		else if (Input.GetButtonUp("FastForward")) Player.FastForward(false);
	}

	private void OnSubmit(PlayerState newState, PlayerState oldState)
	{
		if (newState == PlayerState.Submitted)
		{
			int[] attempt = Player.Recording.Sentiments.Where(sentiment => sentiment.Recorded).Select(sentiment => sentiment.ID).ToArray<int>();
			bool solved = Player.Recording.Solution.SequenceEqual<int>(attempt);

			// Grade the player
			if (!solved)
			{
				if (attempt.Length == 0) MessageSystem?.DisplayMessage(EmptyFailureMessage);
				else
				{
					int randomIndex = Random.Range(0, FailureMessages.Length - 1);
					MessageSystem?.DisplayMessage(FailureMessages[randomIndex]);
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
		FindObjectOfType<LevelUnlockController>().CurrentLevel.CompleteLevel();

		yield return new WaitForSecondsRealtime(.5f);

		// Fade all non-recorded text
		StartCoroutine(FindObjectOfType<Transcript>().FadeOutNonRecordedWords(1.5f));

		// Play the guilty recording
		// TODO

		// Show the newspaper
		FindObjectOfType<Newspaper>().Display();

		yield return new WaitForSecondsRealtime(4);

		// Go back to the level select screen
		Exit();
	}
}
