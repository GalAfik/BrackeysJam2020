using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(TMP_Text))]

public class Transcript : MonoBehaviour
{
	public Color CurrentColor = Color.white;
	public Color RecordedColor = Color.red;
	public float TextFadeSpeed = 2f;

	private Level Level;
	private Player Player;
	private TMP_Text Text;

    void Start()
    {
		Level = AssetDatabase.LoadAssetAtPath<Level>("Assets/States/Level.asset");
		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
		Player.AddListener(StartFade);
		Text = GetComponent<TMP_Text>();
    }

    public void Update()
    {
		string transcript = string.Join(" ", Level.Sentiments.Select(sentiment => WrapPhrase(sentiment)));
		Text.SetText(transcript);
	}

	private void StartFade(PlayerState newState, PlayerState oldState)
    {
		if (newState == PlayerState.Submitted)
        {
			StartCoroutine(FadeNonRecordedWords());
		}
	}

	private IEnumerator FadeNonRecordedWords()
	{
		while (CurrentColor.a > 0)
		{
			CurrentColor.a -= .01f / TextFadeSpeed;
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
			return "<color=#" + ColorUtility.ToHtmlStringRGBA(CurrentColor) + ">" + sentiment.Phrase + "</color>";
		}
		return sentiment.Phrase;
	}
}
