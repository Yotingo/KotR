using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit();
    }

	public void GoToScene(string name)
	{
		SceneManager.LoadScene(name);
	}
}
