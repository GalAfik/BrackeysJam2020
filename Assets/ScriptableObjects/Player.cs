﻿using UnityEngine;
using UnityEngine.Events;

public enum PlayerState { Ready, Playing, Paused, Done, Rewinding, Submitted };

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
            OnStateChange.Invoke(value, state);
            state = value;
        }
    }
	public bool IsRecording;

    private PlayerState state;
    private _UnityEventPlayerState OnStateChange;

	public void Reset(Recording recording)
    {
		if (Recording != null)
        {
			Recording.Deactivate();
        }
		recording.Activate();
		Recording = recording;
		State = PlayerState.Ready;
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
			State == PlayerState.Paused)
		{
			State = PlayerState.Playing;
		}
		else if (State == PlayerState.Playing ||
			State == PlayerState.Rewinding)
		{
			State = PlayerState.Paused;
		}
	}

	public void Pause()
    {
		if (State != PlayerState.Playing &&
			State != PlayerState.Rewinding) return;

		State = PlayerState.Paused;
	}

	public void Rewind()
	{
		if (State != PlayerState.Playing &&
			State != PlayerState.Paused &&
			State != PlayerState.Done) return;

		State = PlayerState.Rewinding;
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

	public void FastForward(bool status)
	{
		if (State != PlayerState.Playing) return;

		Time.timeScale = (status ? 2f : 1f);
	}
}
