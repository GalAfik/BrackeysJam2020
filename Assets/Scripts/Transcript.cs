using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(TMP_Text))]

public class Transcript : MonoBehaviour
{
	private Color HiddenColor = new Color(0,0,0,0);
	public Color DefaultColor = Color.gray;
	public Color PlayedColor = Color.white;
	public Color RecordedColor = Color.red;

	private Player Player;
	private TMP_Text Text;

	private string Dash;


	private void Start()
    {
		Player = Resources.Load<Player>("Player");
		Text = GetComponent<TMP_Text>();
	}

    private void Update()
    {
		Dash = "<color=#" + ColorUtility.ToHtmlStringRGBA(DefaultColor) + "> - </color>";
		if (Player.Recording != null)
		{
			string transcript = string.Join("", Player.Recording.Sentiments.Select(sentiment => WrapPhrase(sentiment)));
			if (transcript.StartsWith(Dash))
			{
				transcript = transcript.Substring(Dash.Length, transcript.Length - Dash.Length);
			}
			Text.SetText(transcript);
		}
	}

	public IEnumerator FadeOutNonRecordedWords(float textFadeSpeed = 2.5f)
	{
		while (PlayedColor.a > 0)
		{
			PlayedColor.a -= .01f / textFadeSpeed;
			DefaultColor.a -= .01f / textFadeSpeed;
			yield return new WaitForSeconds(.01f);
		}
	}

	private IEnumerator FadeInNonRecordedWords(float textFadeSpeed = 2.5f)
	{
		if (PlayedColor.a < 0) PlayedColor.a = 0;
		while (PlayedColor.a < 1)
		{
			PlayedColor.a += .01f / textFadeSpeed;
			yield return new WaitForSeconds(.01f);
		}
	}

	private string WrapPhrase(Recording.Sentiment sentiment)
	{
		if (sentiment.Recorded)
		{
			return Dash + "<color=#" + ColorUtility.ToHtmlStringRGBA(RecordedColor) + ">" + sentiment.Phrase + "</color>";
		}
		else if (sentiment.Played)
		{
			return Dash + "<color=#" + ColorUtility.ToHtmlStringRGBA(PlayedColor) + ">" + sentiment.Phrase + "</color>";
		}
		else if (!sentiment.Demoed)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(HiddenColor) + "> - " + sentiment.Phrase + "</color>";
		}
		return Dash + "<color=#" + ColorUtility.ToHtmlStringRGBA(DefaultColor) + ">" + sentiment.Phrase + "</color>";
	}
}
