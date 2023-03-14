using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereManager : MonoBehaviour
{
	// This class provides the balls
	public Sphere ballPrefab;
	public int nextColor;

	public int nextRandomColor()
	{
		// Give the next ball a new random color
		nextColor = Random.Range(0, ballPrefab.materials.Capacity);
		return nextColor;
	}

	public Material getMaterial()
	{
		// Get the material of the next ball to be insantiated
		return ballPrefab.materials[nextColor];
	}

	public Sphere instantiateNextBall(Vector3 position, Quaternion rotation)
	{
		// Instantiate a ball with the current color
		var sphere = Instantiate(ballPrefab,position, rotation);

		if (nextColor < 0 || nextColor >= ballPrefab.materials.Capacity)
		{
			nextRandomColor();
		}
		sphere.setColor(nextColor);

		nextRandomColor();

		return sphere;
	}
	public Sphere instantiateNextBall(Vector3 position, Quaternion rotation, int color)
	{
		// Used when we want to specify which color to instantiate
		nextColor = color;
		return instantiateNextBall(position, rotation);
	}

}
