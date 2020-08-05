using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public MessageSystem MessageSystem;
	public Recording[] Recordings;

	public string EmptyFailureMessage;
	public string[] FailureMessages;

	private Player Player;
	private int CurrentRecording = 0;

	private void Start()
	{
		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
		Player.Reset(Recordings[CurrentRecording], true);
		Player.AddListener(OnSubmit);
	}

	private void Update()
	{
		HandleInputs();
	}

	public void Exit()
	{
		FindObjectOfType<FadeCanvas>().FadeOut();
		// Exit to level select
		Invoke("GoToTitleMenu", 1);
	}

	private void GoToTitleMenu()
    {
		MenuController.LoadLevel("TitleMenu");
		MenuController menuController = FindObjectOfType<MenuController>();
		menuController.SetState(menuController.TitleState);
	}

	private void HandleInputs()
	{
		if (Player.State == PlayerState.Demo ||
			Player.State == PlayerState.Submitted) return;

		if (Input.GetButtonDown("Play")) Player.Play();
		if (Input.GetButtonDown("Rewind")) Player.Rewind();
		if (Input.GetButtonDown("Record")) Player.Record();
		if (Input.GetButtonDown("Submit")) Player.Submit();
		if (Input.GetButtonDown("Exit")) Exit();
		if (Input.GetButtonDown("FastForward")) Player.FastForward(true);
		else if (Input.GetButtonUp("FastForward")) Player.FastForward(false);
	}

	private void OnSubmit(PlayerState newState, PlayerState oldState)
	{
		if (newState == PlayerState.Submitted)
		{
			string attempt = string.Join(",", Player.Recording.Sentiments.Where(sentiment => sentiment.Recorded).Select(sentiment => sentiment.ID));
			bool solved = Player.Recording.Solutions.Contains(attempt);

			// Grade the player
			if (!solved)
			{
				if (attempt == "") MessageSystem?.DisplayMessage(EmptyFailureMessage);
				else
				{
					int randomIndex = Random.Range(0, FailureMessages.Length - 1);
					MessageSystem?.DisplayMessage(FailureMessages[randomIndex]);
				}
			}
			else
			{
				StartCoroutine(GradeSubmission(solved));
			}

			Debug.Log(solved ? "Success!" : "Try again");
		}
	}

	private IEnumerator GradeSubmission(bool solved)
	{
		yield return new WaitForSeconds(2);
	}
}
