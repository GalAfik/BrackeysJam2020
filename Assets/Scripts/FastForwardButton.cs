using System.Linq;
using UnityEditor;
using UnityEngine;

public class FastForwardButton : MonoBehaviour
{
	private Player Player;

	private void Start()
	{
		Player = AssetDatabase.LoadAssetAtPath<Player>("Assets/States/Player.asset");
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
