using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]

public class FinalLevelController : MonoBehaviour
{
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

		StartCoroutine(StartEndingSequence());
	}

	public IEnumerator StartEndingSequence()
	{
		yield return new WaitForSeconds(5);

		// Play the AudioSource
		GetComponent<AudioSource>()?.Play();

		// Wait for the audio clip to finish
		yield return new WaitForSecondsRealtime(GetComponent<AudioSource>().clip.length + 3);

		// Mark level as complete
		FindObjectOfType<LevelUnlockController>()?.CurrentLevel.CompleteLevel();

		FindObjectOfType<FadeCanvas>().FadeOut();

		yield return new WaitForSeconds(1);

		// Exit to the credits screen
		MenuController menuController = FindObjectOfType<MenuController>();
		menuController.SetState(menuController.TitleState);
		yield return new WaitForSeconds(.5f);
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(gameObject.scene.name));
		Menu.transform.position = new Vector3(0, 0, 0);

		menuController.ToggleCredits();


		FindObjectOfType<FadeCanvas>().FadeIn();
		Menu.transform.Find("UI").GetComponent<Canvas>().enabled = true;
	}
}
