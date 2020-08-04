using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public Recording[] Recordings;

	private Player Player;
	private int CurrentRecording = 0;

	private void Start()
	{
		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
		Player.Reset(Recordings[CurrentRecording]);
		Player.AddListener(OnSubmit);
	}

	private void Update()
	{
		HandleInputs();
	}

	private void HandleInputs()
	{
		if (Input.GetKeyDown(KeyCode.N)) StartLevel();

		if (Input.GetKeyDown(KeyCode.A)) Player.Rewind();
		if (Input.GetKeyUp(KeyCode.A)) Player.Pause();
		if (Input.GetKeyDown(KeyCode.S)) Player.Play();
		if (Input.GetKeyDown(KeyCode.D)) Player.Record();
		if (Input.GetKeyDown(KeyCode.F)) Player.FastForward(true);
		else if (Input.GetKeyUp(KeyCode.F)) Player.FastForward(false);

		if (Input.GetButtonDown("Play")) Player.Play();
		if (Input.GetButtonDown("Rewind")) Player.Rewind();
		if (Input.GetButtonDown("Record")) Player.Record();
		if (Input.GetButtonDown("Submit")) Player.Submit();
		if (Input.GetButtonDown("FastForward")) Player.FastForward(true);
		else if (Input.GetButtonUp("FastForward")) Player.FastForward(false);
	}

	private void StartLevel()
	{
		CurrentRecording = (CurrentRecording + 1) % Recordings.Length;
		Player.Reset(Recordings[CurrentRecording]);
	}

	private void OnSubmit(PlayerState newState, PlayerState oldState)
	{
		if (newState == PlayerState.Submitted)
		{
			string attempt = string.Join(",", Player.Recording.Sentiments.Where(sentiment => sentiment.Recorded).Select(sentiment => sentiment.ID));
			bool solved = Player.Recording.Solutions.Contains(attempt);

			// TODO
			StartCoroutine(GradeSubmission(solved));
			Debug.Log(solved ? "Success!" : "Try again");
		}
	}

	private IEnumerator GradeSubmission(bool solved)
	{
		yield return new WaitForSeconds(4);
		if (!solved) Player.ReverseSubmission();
	}
}
