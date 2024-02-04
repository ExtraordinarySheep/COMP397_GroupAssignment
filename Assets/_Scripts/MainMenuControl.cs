using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{

    [SerializeField] List<GameObject> panels = new List<GameObject>();

    private void DisableAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    public void SwitchToPanel(GameObject panel)
    {
        DisableAllPanels();
        panel.SetActive(true);
    }

}
