using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void LoadOpenWater()
    {
        SceneManager.LoadScene("Openocean");
        SceneManager.UnloadScene("Menu");
    }

    public void LoadMainland()
    {
        SceneManager.LoadScene("Mainland");
        SceneManager.UnloadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
