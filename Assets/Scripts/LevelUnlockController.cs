using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUnlockController : MonoBehaviour
{
	private static LevelUnlockController self;

	public Level TutorialLevel;
	public Level FinalLevel;
	public Level[] Levels;

	public TMP_Text FinalLevelUnlockLabel;

	private void Awake()
	{
		if (self == null)
		{
			self = this;
			return;
		}
		Destroy(gameObject);
	}

	private void Update()
	{
		int levelsCompleted = GetLevelsCompleted();

		if (FinalLevel.Completed == false)
		{
			// Lock the TutorialLevel once it has been completed
			if (TutorialLevel.Completed) TutorialLevel.Locked = true;
			else TutorialLevel.Locked = false;

			// Check if each level should be locked or unlocked
			foreach (var level in Levels)
			{
				if (level.NumberNeededToUnlock > levelsCompleted || level.Completed || (!FinalLevel.Locked && !FinalLevel.Completed)) level.Locked = true;
				else level.Locked = false;
			}

			// Unlock the final level and lock all other levels when enough levels have been completed
			if (FinalLevel.NumberNeededToUnlock > levelsCompleted) FinalLevel.Locked = true;
			else FinalLevel.Locked = false;
		}
		else
		{
			// Unlock all levels once the final level is completed
			TutorialLevel.Locked = false;
			foreach (var level in Levels)
			{
				level.Locked = false;
			}
		}


		// Set the final level's unlock label - only shows up after the tutorial and before completing the game
		if (TutorialLevel.Completed && FinalLevel.Locked)
		{
			FinalLevelUnlockLabel?.SetText((levelsCompleted-1)  + "/" + (FinalLevel.NumberNeededToUnlock-1));
		}
		else FinalLevelUnlockLabel?.SetText("");
	}

	// Return the number of levels completed
	public int GetLevelsCompleted()
	{
		int levelsCompleted = 0;
		// Check all regular levels
		foreach (var level in Levels)
		{
			if (level.Completed) levelsCompleted++;
		}

		// Check the tutorial level
		if (TutorialLevel.Completed) levelsCompleted++;

		return levelsCompleted;
	}
}
