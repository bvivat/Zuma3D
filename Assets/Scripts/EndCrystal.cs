using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndCrystal : MonoBehaviour
{
	public GameObject endPanel;
	public TrainManager manager;
	public GameObject menuStone;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Train")
		{
			manager.speed = 0;
			menuStone.SetActive(true);
			menuStone.GetComponentInChildren<Text>().text = "Defeat !\nMenu";
		}
	}
}
