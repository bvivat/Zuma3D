using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
	public float force;
	public float radius;
	public float upwardForce;
    // Start is called before the first frame update
    void Start()
    {
		transform.Rotate(new Vector3(Random.Range(0,360), Random.Range(0, 360), Random.Range(0, 360)));
		foreach (Transform t in transform)
		{
			t.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius, upwardForce, ForceMode.Impulse);
		}
		Destroy(gameObject, 10);
    }

}
