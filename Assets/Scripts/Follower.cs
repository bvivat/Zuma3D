using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed;
    public float distanceTravelled = 0;
    public EndOfPathInstruction endOfPathInstruction;
    Rigidbody rb;
    public GameObject trainManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update position of the ball
        distanceTravelled += speed * Time.deltaTime;
        rb.MovePosition(pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction));
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("First") && gameObject.CompareTag("Last"))
        {
            trainManager.SendMessage("OnUnification", collision.gameObject.GetComponent<Sphere>());
        }
        if (collision.gameObject.CompareTag("New") && !trainManager.GetComponent<TrainManager>().isCreating)
        {
            Sphere[] tempStorage = new Sphere[2];
            tempStorage[0] = this.GetComponentInParent<Sphere>();
            tempStorage[1] = collision.gameObject.GetComponent<Sphere>();
            trainManager.SendMessage("insertSphere", tempStorage);
        }
        if (collision.gameObject.CompareTag("Bomb"))
        {
            trainManager.SendMessage("bombExplosion", gameObject.GetComponent<Sphere>());
        }
    }
}
