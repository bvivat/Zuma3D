using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// This class contains the raycast laser and all the gun's behavior (loading, firing, ...)
public class Laser : MonoBehaviour
{

	LineRenderer LR;
	public Material material;
	public SphereManager sphereManager;
	public GameObject bomb;
	public bool isEmpty;
	public ReplacementHand replacementHand;

	void Start()
	{
		// Create the visible raycast with the same color than the next ball
		sphereManager.nextRandomColor();
		material = sphereManager.getMaterial();
		LR = gameObject.AddComponent<LineRenderer>();
		LR.startWidth = 0.01f;
		LR.endWidth = 0.01f;
		LR.positionCount = 2;
		LR.material = material;
		LR.startColor = Color.yellow;
		LR.endColor = Color.yellow;
	}

	void Update()
	{
		if (!isEmpty)
		{
			LR.SetPosition(0, transform.position);
			LR.material = material;
			RaycastHit hit;
			// Does the ray intersect any objects
			if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
			{
				LR.SetPosition(1, hit.point);
				LR.endColor = Color.red;
			}
			else
			{
				LR.SetPosition(1, transform.position + transform.TransformDirection(Vector3.forward) * 1000);
			}
		}

	}

	
	public void OnFire(InputValue value)
	{
		if (!isEmpty)
		{
			// Instantiate the "loaded" projectile
			var projectile = sphereManager.instantiateNextBall(transform.position + transform.TransformDirection(Vector3.forward * 2), Quaternion.identity);
			projectile.tag = "New";

			// Should not yet be on the path
			projectile.GetComponent<Follower>().enabled = false;

			// Add a force forward
			var rb = projectile.GetComponent<Rigidbody>();
			rb.velocity = transform.forward * 100;

			// Empty the gun
			isEmpty = true;
			LR.enabled = false;
		}
		else
		{
			Debug.Log("Gun is empty !");
		}

	}

	public void loadBall(Sphere ball)
	{
		// If not empty, give the replacement hand the loaded ball
		if (!isEmpty)
		{
			replacementHand.SetCurrentColor(sphereManager.nextColor);
		}
		else
		{
			replacementHand.SetCurrentColor(-1);
		}
		// Set the next instantiated color to the loaded one
		sphereManager.nextColor = ball.color;
		material = sphereManager.getMaterial();

		// Unempty the gun
		isEmpty = false;
		LR.enabled = true;

	}

	private void OnTriggerEnter(Collider other)
	{
		// This trigger is on the low part of the gun
		if (other.tag == "New")
		{
			Sphere ball = other.GetComponent<Sphere>();
			loadBall(ball);
			// We grabbed the next color to be instantiated, we don't want the bullet to be physically present anymore
			Destroy(ball.gameObject);
		}
	}
}