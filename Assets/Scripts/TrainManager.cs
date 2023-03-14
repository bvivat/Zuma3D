using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.UI;

public class TrainManager : MonoBehaviour
{
    // Variable for train creation
    public int sphereNumber = 15;
    int sphereCounter = 0;

    public GameObject bombPrefab;
    public bool isCreating = true; // Train manager is still creating balls

    bool isReverse = false; // To set the correct variable when inserting behind a moving back part

    // Train and path variables
    public PathCreator pathCreator;
    public float speed = 5;
    public float acceleration = 1.0001f;
    public float maxSpeed = 15f;
    float distanceTravelled;
    EndOfPathInstruction endOfPathInstruction;

    List<Sphere> train = new List<Sphere>(); // Logical representation of the train

    public SphereManager sphereManager;
    public GameObject player;
	public GameObject menu;


    // Start is called before the first frame update
    void Start()
    {
        // Launch sphereManager
        sphereManager.nextRandomColor();

        // Reset
        resetLevel();

        // Instantiate the first sphere
        Vector3 newPosition = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);

        Sphere sphere = sphereManager.instantiateNextBall(newPosition, Quaternion.identity);
        sphere.GetComponent<Follower>().enabled = true;
        sphere.GetComponent<Follower>().pathCreator = pathCreator;
        sphere.GetComponent<Follower>().speed = speed;
        sphere.GetComponent<Follower>().trainManager = gameObject;
        train.Add(sphere);
        sphereCounter++; //+1 Sphere
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distanceTravelled += speed * Time.deltaTime;
        // Check number of spheres and the distance from the last
        if (sphereCounter < sphereNumber && distanceTravelled >= 1)
        {
            distanceTravelled = 0;
            // Instantiate new sphere
            Vector3 newPosition = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            Sphere sphere = sphereManager.instantiateNextBall(newPosition, Quaternion.identity);
            sphere.GetComponent<Follower>().enabled = true;
            sphere.GetComponent<Follower>().pathCreator = pathCreator;
            sphere.GetComponent<Follower>().speed = speed;
            sphere.GetComponent<Follower>().trainManager = gameObject;
            train.Add(sphere);

            sphereCounter++; //+1 Sphere
        }
        if (sphereCounter >= sphereNumber)
        {
            isCreating = false;
        }

        if (!isCreating) // When train is fully created
        {
            //Speed up the train
            if (!isReverse)
            {
                if (speed < maxSpeed)
                {
                    speed = speed * acceleration;
                }

                //Update speed of balls
                for (int i = 0; i < train.Count; i++)
                {
                    train[i].GetComponent<Follower>().speed = speed;
                }
            }
        }
    }

	private void Update()
	{
        // Check the victory of the player
		if (train.Count == 0)
		{
			menu.SetActive(true);
			menu.GetComponentInChildren<Text>().text = "Victory !\nMenu";
		}
	}

	public void resetLevel()
    {
        isCreating = true;
        sphereCounter = 0;

        // Reset train
        for (int i = train.Count -1; i > 0; i--)
        {
            train[i].gameObject.GetComponent<Sphere>().Shatter();
            train.RemoveAt(i);
        }
    }

    public void OnUnification(Sphere sphere)
    {
        int index = train.IndexOf(sphere);

        // Untag balls
        train[index - 1].tag = "Train";
        train[index].tag = "Train";
        isReverse = false;

        // Reset speed
        for (int i = 0; i<train.Count; i++)
        {
            train[i].GetComponent<Follower>().speed = speed;
        }

        // Trigger pattern detection
        PatternDetectionAtIndex(index);
    }

    public void insertSphere(Sphere[] newSphere)
    {
        int index = train.IndexOf(newSphere[0]);

		// Move back each ball after
		for (int i = train.Count - 1; i > index; i--)
        {
            train[i].GetComponent<Follower>().distanceTravelled -= 1;
        }

        // Setting new ball
        newSphere[1].tag = "Train"; // Remove tag
        newSphere[1].GetComponent<Follower>().enabled = true;
        newSphere[1].GetComponent<Follower>().pathCreator = pathCreator;
        if (isReverse && train[index].CompareTag("Last")) // When reversing, become the Last ball of first part
        {
            newSphere[1].GetComponent<Follower>().speed = -speed;
            train[index].tag = "Train";
            newSphere[1].tag = "Last";
        }
        else if (index + 1 >= train.Count)
        {
            newSphere[1].GetComponent<Follower>().speed = speed;
        }
        else
        {
            newSphere[1].GetComponent<Follower>().speed = train[index + 1].GetComponent<Follower>().speed;
        }
        newSphere[1].GetComponent<Follower>().trainManager = gameObject;
        newSphere[1].GetComponent<Rigidbody>().velocity = transform.forward * 0; // Reset speed from the shoot
        newSphere[1].GetComponent<Follower>().distanceTravelled = train[index].GetComponent<Follower>().distanceTravelled - 1; // Insert graphically inserted ball
        train.Insert(index + 1, newSphere[1]);

        // Pattern detection if the train is united
        if (!isReverse)
        {
            PatternDetectionAtIndex(index);
        }
    }

    void PatternDetectionAtIndex(int index)
    {
        List<int> patternIndexes = new List<int>();

        // Add triggered ball
        patternIndexes.Add(index);

        // Check before
        int iBefore = index - 1;
        while (iBefore >= 0 && train[iBefore].getColor() == train[index].getColor())
        {
            patternIndexes.Add(iBefore);
            iBefore--;
        }

        // Check After
        int iAfter = index + 1;
        while (iAfter < train.Count && train[iAfter].getColor() == train[index].getColor())
        {
            patternIndexes.Add(iAfter);
            iAfter++;
        }

        // Count successive balls
        if (patternIndexes.Count >= 3)
        {
            destroyBallsByIndex(patternIndexes);

            // Reverse Balls
            if (iBefore >= 0 && iBefore + 1 < train.Count)
            {
                isReverse = true;
                for (int i = iBefore; i >= 0; i--)
                {
                    if (train[i].GetComponent<Follower>().speed > 0)
                    {
                        train[i].GetComponent<Follower>().speed *= -1;
                    }
                }

                // Use "First" and "Last" to get only 1 call when collision (from "Last")
                train[iBefore].tag = "Last";
                train[iBefore + 1].tag = "First";
            }

            // Special object
            if (Random.Range(0,1) == 0) // Random generation (at each destruction)
            {
                var specialObject = Instantiate(bombPrefab, train[patternIndexes[0]].transform.position, Quaternion.identity);
                Vector3 randVector = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f))*0.5f; //Used as a target close to the player
                specialObject.GetComponent<SpecialObjectMove>().target = player.transform.position + randVector;
            }
        }
    }

    void bombExplosion(Sphere collided)
    {
        int index = train.IndexOf(collided);

        // Create list of ball to destroy arouns the bomb explosion
        List<int> patternIndexes = new List<int>();
        int iBefore = index - 3;
        for (int i = index -2; i <= index + 2 && i < train.Count; i++)
        {
            if (i < 0)
            {
                i = 0;
                iBefore = 0;
            }
            patternIndexes.Add(i);
        }

        destroyBallsByIndex(patternIndexes);

        // Reverse Balls
        if (iBefore >= 0 && iBefore + 1 < train.Count)
        {
            isReverse = true;
            for (int i = iBefore; i >= 0; i--)
            {
                if (train[i].GetComponent<Follower>().speed > 0)
                {
                    train[i].GetComponent<Follower>().speed *= -1;
                }
            }

            // Use "First" and "Last" to get only 1 call when collision (from "Last")
            train[iBefore].tag = "Last";
            train[iBefore + 1].tag = "First";
        }
    }

    void destroyBallsByIndex(List<int> patternIndexes)
    {
        patternIndexes.Sort();
        // Destroy every gameObject in patternIndexes
        for (int i = patternIndexes.Count - 1; i >=0; i--)
        {
			//Destroy(train[patternIndexes[i]].gameObject);
			train[patternIndexes[i]].gameObject.GetComponent<Sphere>().Shatter();
            train.RemoveAt(patternIndexes[i]);
        }
    }
}
