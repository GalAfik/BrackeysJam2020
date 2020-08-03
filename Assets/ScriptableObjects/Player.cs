using UnityEngine;
using UnityEngine.Events;

public enum PlayerState { Paused, Playing, Recording, Done, Rewinding, Submitted };

[System.Serializable] public class _UnityEventPlayerState : UnityEvent<PlayerState, PlayerState> { }

[CreateAssetMenu(fileName = "Player", menuName = "States/Player")]
public class Player : ScriptableObject
{
    public PlayerState State {
        get {
            return state;
        }
        set
        {
            OnStateChange.Invoke(value, state);
            state = value;
        }
    }

    private PlayerState state;
    private _UnityEventPlayerState OnStateChange;

    public void AddListener(UnityAction<PlayerState, PlayerState> call)
    {
        OnStateChange.AddListener(call);
    }
}
