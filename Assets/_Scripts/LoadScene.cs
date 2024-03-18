using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private static LoadScene instance = null;
    private LoadScene() { }
    public static LoadScene Instance()
    {
        return instance ??= new LoadScene();
    }

    public GameObject pausePanel;
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
        SaveGameManager.Instance().saveLoaded = false;
        SceneManager.LoadScene(sceneName);
    }
    public void ExitButtonClicked()
    {
        //#if UNITY_EDITOR
        //#else
        //    Application.Quit();
        //#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
