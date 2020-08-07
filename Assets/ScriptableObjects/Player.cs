using UnityEngine;
using UnityEngine.Events;

public enum PlayerState { Off, Ready, Demo, Playing, Paused, Done, Rewinding, FastForward, Submitted };

[System.Serializable] public class _UnityEventPlayerState : UnityEvent<PlayerState, PlayerState> { }

[CreateAssetMenu(fileName = "Player", menuName = "States/Player")]
public class Player : ScriptableObject
{
	public Recording Recording;
	public PlayerState State {
        get {
            return state;
        }
        set
        {
			PlayerState oldState = state;
			state = value;
			OnStateChange.Invoke(value, oldState);
        }
    }
	public bool IsRecording;

    private PlayerState state = PlayerState.Off;
	private _UnityEventPlayerState OnStateChange = new _UnityEventPlayerState();

	public void Reset(Recording recording, bool demo)
    {
		if (Recording != null)
        {
			Recording.Deactivate();
        }
		recording.Activate();
		Recording = recording;
		State = PlayerState.Off;
		IsRecording = false;
	}

    public void AddListener(UnityAction<PlayerState, PlayerState> call)
    {
        OnStateChange.AddListener(call);
    }

	public void RemoveListener(UnityAction<PlayerState, PlayerState> call)
	{
		OnStateChange.RemoveListener(call);
	}

	public void Play()
	{
		if (State == PlayerState.Ready ||
			State == PlayerState.Paused ||
			State == PlayerState.FastForward)
		{
			State = PlayerState.Playing;
		}
		else if (State == PlayerState.Playing ||
			State == PlayerState.Rewinding)
		{
			State = PlayerState.Paused;
		}
	}

	public void Rewind()
	{
		if (State != PlayerState.Playing &&
			State != PlayerState.Paused &&
			State != PlayerState.Done) return;

		State = PlayerState.Rewinding;
	}

	public void FastForward()
	{
		if (State != PlayerState.Playing &&
			State != PlayerState.Paused &&
			State != PlayerState.Ready) return;

		State = PlayerState.FastForward;
	}

	public void Record()
	{
		IsRecording = !IsRecording;
	}

	public void Submit()
	{
		if (State != PlayerState.Done) return;

		State = PlayerState.Submitted;
	}

	public void ReverseSubmission()
	{
		if (State != PlayerState.Submitted) return;

		State = PlayerState.Done;
	}
}
