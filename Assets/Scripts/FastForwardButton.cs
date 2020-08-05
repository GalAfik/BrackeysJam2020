using System.Linq;
using UnityEditor;
using UnityEngine;

public class FastForwardButton : MonoBehaviour
{
	private Player Player;

	private void Start()
	{
		Player = Resources.Load<Player>("Player");
	}

	public void onPress()
	{
		Player.FastForward(true);
	}

	public void onRelease()
	{
		Player.FastForward(false);
	}
}
