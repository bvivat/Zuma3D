using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObjectMove : MonoBehaviour
{

	public Vector3 target;
	public float speed = 15;
	public float acceleration = 0.967f;
	public float distance = 5;


	// Update is called once per frame
	void FixedUpdate()
	{
		// Slow down when close enough to the player
		if (Vector3.Distance(target, transform.position) <= distance)
        {
			speed = speed * acceleration;
			gameObject.tag = "bomb";
		}

		// Move the bomb toward the player
		var step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, target, step);

		// Player has to catch it quickly, or it will disapear once it has reached its target
		if (transform.position == target)
		{
			Destroy(this.gameObject);
		}
	}
}
