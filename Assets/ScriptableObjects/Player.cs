using UnityEngine;

public enum PlayerState { Paused, Playing, Recording, Done, Rewinding, Submitted };

[CreateAssetMenu(fileName = "Player", menuName = "States/Player")]
public class Player : ScriptableObject
{
    public PlayerState State { get; set; }
}
