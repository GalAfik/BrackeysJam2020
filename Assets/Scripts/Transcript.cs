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
	private TMP_Text Text;

    // Start is called before the first frame update
    void Start()
    {
		Level = AssetDatabase.LoadAssetAtPath<Level>("Assets/States/Level.asset");
		Text = GetComponent<TMP_Text>();
    }

    public void Update()
    {
		string transcript = string.Join(" ", Level.Sentiments.Select(sentiment => WrapPhrase(sentiment)));
		Text.SetText(transcript);
	}

	public IEnumerator FadeNonRecordedWords()
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
