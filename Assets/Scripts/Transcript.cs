using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]

public class Transcript : MonoBehaviour
{
	public Color CurrentColor = Color.white;
	public Color RecordedColor = Color.red;
	private TMP_Text Text;

    // Start is called before the first frame update
    void Start()
    {
		Text = GetComponent<TMP_Text>();
    }

    public void SetText(Recording.Sentiment[] sentiments)
	{
		string transcript = "";
		foreach (var sentiment in sentiments)
		{
			// Change the color of the sentiment phrase depending on if it is being recorded or has already been played
			if (sentiment.Recorded)
			{
				// Has been heard and recorded, or is recording
				transcript += "<color=#"+ ColorUtility.ToHtmlStringRGBA(RecordedColor) +">" + sentiment.Phrase + "</color> ";
			}
			else if (sentiment.Current)
			{
				// Has been heard, but not recorded
				transcript += "<color=#"+ ColorUtility.ToHtmlStringRGBA(CurrentColor) + ">" + sentiment.Phrase + "</color> ";
			}
			else
			{
				// Has not yet been heard
				transcript += sentiment.Phrase + " ";
			}
		}

		// Set the transcript label
		Text?.SetText(transcript);
	}
}
