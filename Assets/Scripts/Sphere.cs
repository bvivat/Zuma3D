using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
	// Contains all possible materials for the balls. Just add a new material here to add a color !
	public List<Material> materials;
	public int color;
	public GameObject shattered_ball;

	public int getColor()
	{
		return color;
	}
	public void setColor(int color)
	{
		this.color = color;
		gameObject.GetComponent<MeshRenderer>().material = materials[color];
	}

	void Update()
    {
		// Destroy balls when too far from the player
		if (Vector3.Distance(Vector3.zero, transform.position) > 500)
		{
			Destroy(gameObject);
		}
    }

	void OnCollisionEnter(Collision collision)
	{
		var other = collision.gameObject;
		if (this.tag == "New")
		{
			if (other.tag == "Gun")
			{
				other.GetComponent<Laser>().loadBall(this);
				Destroy(gameObject);
			}
			else if (other.tag == "Ground")
			{
				Shatter();
			}

		}
	}

	public void Shatter()
	{
		// Generate the shattered version of the destroyed ball
		var shattered =  Instantiate(shattered_ball, transform.position, transform.rotation);
		shattered.transform.localScale = transform.localScale;

		// Give all the shattered pieces the good color
		Renderer[] children;
		children = shattered.GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in children)
		{
			var mats = new Material[rend.materials.Length];
			for (var j = 0; j < rend.materials.Length; j++)
			{
				mats[j] = materials[color];
			}
			rend.materials = mats;
		}

		// Destroy the original unshatterd ball
		Destroy(gameObject);
	}
}
