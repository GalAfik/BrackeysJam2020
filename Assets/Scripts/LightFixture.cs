using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixture : MonoBehaviour
{
	public enum Axis { X, Y, Z };
	public Axis SwayAxis = Axis.Z;
	public float SwayAmount = 2f;
	public float SwaySpeed = 0.4f;

	private Vector3 InitialRotation;

	private void Start()
	{
		// Get the initial rotation to offset later
		InitialRotation = transform.eulerAngles;
	}

	// Update is called once per frame
	private void Update()
    {
		Vector3 newRotation = InitialRotation;
		switch (SwayAxis)
		{
			case Axis.X:
				newRotation.x = InitialRotation.x + Mathf.Sin(SwaySpeed * 2 * Mathf.PI * Time.time) * SwayAmount;
				break;
			case Axis.Y:
				newRotation.y = InitialRotation.y + Mathf.Sin(SwaySpeed * 2 * Mathf.PI * Time.time) * SwayAmount;
				break;
			default:
				newRotation.z = InitialRotation.z + Mathf.Sin(SwaySpeed * 2 * Mathf.PI * Time.time) * SwayAmount;
				break;
		}
		transform.rotation = Quaternion.Euler(newRotation);
    }
}
