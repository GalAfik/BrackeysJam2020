using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(TMP_Text))]

public class Transcript : MonoBehaviour
{
	private Color HiddenColor = new Color(0,0,0,0);
	public Color PlayedColor = Color.white;
	public Color RecordedColor = Color.red;
	public float TextFadeSpeed = 2f;

	private Player Player;
	private TMP_Text Text;

    private void Start()
    {
		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
		Text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
		string transcript = string.Join(" ", Player.Recording.Sentiments.Select(sentiment => WrapPhrase(sentiment)));
		Text.SetText(transcript);
	}

	public IEnumerator FadeOutNonRecordedWords()
	{
		while (PlayedColor.a > 0)
		{
			PlayedColor.a -= .01f / TextFadeSpeed;
			yield return new WaitForSeconds(.01f);
		}
	}

	private IEnumerator FadeInNonRecordedWords()
	{
		if (PlayedColor.a < 0) PlayedColor.a = 0;
		while (PlayedColor.a < 1)
		{
			PlayedColor.a += .01f / TextFadeSpeed;
			yield return new WaitForSeconds(.01f);
		}
	}

	private string WrapPhrase(Recording.Sentiment sentiment)
	{
		if (sentiment.Recorded)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(RecordedColor) + ">" + sentiment.Phrase + "</color>";
		}
		else if (sentiment.Played)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(PlayedColor) + ">" + sentiment.Phrase + "</color>";
		}
		else if (!sentiment.Demoed)
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(HiddenColor) + ">" + sentiment.Phrase + "</color>";
		}
		return sentiment.Phrase;
	}
}
