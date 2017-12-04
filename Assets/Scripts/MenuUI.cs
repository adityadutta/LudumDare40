using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour {

	public void Play()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void Quit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
