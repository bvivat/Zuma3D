using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireFPS : MonoBehaviour
{
	// Script used with non-vr first person controller
	public Laser laser;
  
	public void OnFire(InputValue value)
	{
		laser.OnFire(value);
	}
}
