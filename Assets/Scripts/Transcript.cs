using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]

public class Transcript : MonoBehaviour
{
	public Color CurrentColor = Color.white;
	public Color RecordedColor = Color.red;
	private TMP_Text Text;
	
	public float TextFadeSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
		Text = GetComponent<TMP_Text>();
    }

    public void SetText(Recording.Sentiment[] sentiments)
	{
		string transcript = string.Join(" ", sentiments.Select(sentiment => WrapPhrase(sentiment)));
		Text.SetText(transcript);
	}

	public string WrapPhrase(Recording.Sentiment sentiment)
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

	public IEnumerator FadeNonRecordedWords()
	{
		while(CurrentColor.a > 0)
		{
			CurrentColor.a -= .01f / TextFadeSpeed;
			yield return new WaitForSeconds(.01f);
		}
	}
}
