using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public Sprite PlaySprite;
    public Sprite PauseSprite;

    private Player Player;

    private void Start()
    {
        Player = Resources.Load<Player>("Player");
        Player.AddListener(ChangeIcon);
    }

    private void ChangeIcon(PlayerState newState, PlayerState oldState)
    {
        if (newState == PlayerState.Playing ||
            newState == PlayerState.Rewinding)
        {
            GetComponent<Image>().sprite = PauseSprite;
            return;
        }
        GetComponent<Image>().sprite = PlaySprite;
    }
}
