
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

	public GameObject trainManager;
	public GameObject menu;

	public void Quit()
	{
		// Ferme l'application
		Application.Quit();
	}

	public void LoadScene()
	{
		trainManager.SetActive(true);
		menu.SetActive(false);
	}
}