using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public Sprite PlaySprite;
    public Sprite PauseSprite;

    private Player Player;

    public void Start()
    {
        Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
        Player.AddListener(ChangeIcon);
    }

    private void ChangeIcon(PlayerState newState, PlayerState oldState)
    {
        if (newState == PlayerState.Playing ||
            newState == PlayerState.Recording)
        {
            GetComponent<Image>().sprite = PauseSprite;
            return;
        }
        GetComponent<Image>().sprite = PlaySprite;
    }
}
