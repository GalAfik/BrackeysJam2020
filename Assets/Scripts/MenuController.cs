using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public string WebsiteURL;

    public void Play()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void ToggleSettingsMenu()
	{

	}

	public void ToggleCredits()
	{

	}

	public void Quit()
	{
		Application.Quit();
	}

	public void ToggleMuteVoice()
	{

	}

	public void ToggleMuteMusic()
	{

	}

	public void Website()
	{
		Application.OpenURL(WebsiteURL);
	}
}
