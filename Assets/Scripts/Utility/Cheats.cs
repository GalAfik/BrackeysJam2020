using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey(KeyCode.LeftShift)) Time.timeScale = 5f;
		else Time.timeScale = 1f;
    }
}
