using System.Linq;
using UnityEditor;
using UnityEngine;

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
		if (Input.GetKeyDown("n")) StartLevel();

		if (Input.GetButtonDown("Play")) Player.Play();
		if (Input.GetButtonDown("Rewind")) Player.Rewind();
		if (Input.GetButtonDown("Record")) Player.Record();
		if (Input.GetButtonDown("Submit")) Player.Submit();
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
			Debug.Log(solved ? "Success!" : "Try again");
		}
    }
}
