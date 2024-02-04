using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene("LevelOne");
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LOAD_SCENE(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
