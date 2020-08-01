using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFixture : MonoBehaviour
{
	public float SwayAmount = 2f;
	public float SwaySpeed = 0.4f;

    // Update is called once per frame
    void Update()
    {
		float zRotation = Mathf.Sin(SwaySpeed * 2 * Mathf.PI * Time.time) * SwayAmount;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, zRotation));
    }
}
