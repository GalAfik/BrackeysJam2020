using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour
{
    public Color RecordingColor = Color.red;

    private Player Player;
    private Color RecordButtonColor;

    void Start()
    {
        Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
        RecordButtonColor = gameObject.GetComponent<Image>().color;
    }

    void Update()
    {
        gameObject.GetComponent<Image>().color = Player.State == PlayerState.Recording ? RecordingColor : RecordButtonColor;
    }
}
