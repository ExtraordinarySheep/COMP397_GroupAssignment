using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject saveButton;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Time.timeScale = 1;
    }

    public void SaveGame()
    {
        SaveGameManager.Instance().SaveGame(player.transform);
        saveButton.GetComponent<Image>().color = Color.green;
        saveButton.GetComponentInChildren<TMP_Text>().text = "Saved";
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Pause()
    {
        saveButton.GetComponent<Image>().color = Color.white;
        saveButton.GetComponentInChildren<TMP_Text>().text = "Save";
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void LoadGame()
    {
        SaveGameManager.Instance().LoadGame();
    }
}
