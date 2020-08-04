using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Caption : MonoBehaviour
{
	public TMP_Text CaptionLabel;
	private Color TextColor;
	private string Text;

	// Start is called before the first frame update
	void Start()
	{
		Text = CaptionLabel.text;
		TextColor = CaptionLabel.color;
		TextColor.a = 0;

		CaptionLabel?.SetText("<color=#" + ColorUtility.ToHtmlStringRGBA(TextColor) + ">" + Text + "</color>");
	}

	public void OnMouseOver()
	{
		TextColor.a = 1;
		CaptionLabel?.SetText("<color=#" + ColorUtility.ToHtmlStringRGBA(TextColor) + ">" + Text + "</color>");
	}

	public void OnMouseExit()
	{
		TextColor.a = 0;
		CaptionLabel?.SetText("<color=#" + ColorUtility.ToHtmlStringRGBA(TextColor) + ">" + Text + "</color>");
	}
}
