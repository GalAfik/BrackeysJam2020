using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public Sprite PlaySprite;
    public Sprite PauseSprite;

    private Player Player;

    void Start()
    {
        Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
    }

    void Update()
    {
        if (Player.State == PlayerState.Playing ||
            Player.State == PlayerState.Recording)
        {
            GetComponent<Image>().sprite = PauseSprite;
            return;
        }
        GetComponent<Image>().sprite = PlaySprite;
    }
}
