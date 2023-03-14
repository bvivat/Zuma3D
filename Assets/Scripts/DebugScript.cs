using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("I'm " + gameObject.GetComponent<MeshRenderer>().material + " and I'm at x:" + gameObject.transform.position.x + ", y:" + gameObject.transform.position.y + ", z:" + gameObject.transform.position.z);
    }
}
