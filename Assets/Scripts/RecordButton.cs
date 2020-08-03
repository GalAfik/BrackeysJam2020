using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour
{
    public Color RecordingColor = Color.red;

    private Player Player;
    private Color RecordButtonColor;

    public void Start()
    {
        Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
        Player.AddListener(ChangeRecordButtonColor);
        RecordButtonColor = gameObject.GetComponent<Image>().color;
    }

    public void ChangeRecordButtonColor(PlayerState newState, PlayerState oldState)
    {
        gameObject.GetComponent<Image>().color = newState == PlayerState.Recording ? RecordingColor : RecordButtonColor;
    }
}
