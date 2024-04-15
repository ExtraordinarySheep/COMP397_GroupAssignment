using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public string lastScene;
    public string position;
    public List<string> unlockedAchievements = new();
    public List<string> unlockedAchievementIds; // Add a list to store unlocked achievement IDs

    public static PlayerData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerData>(jsonString);
    }
}

[System.Serializable]
public class SaveGameManager
{
    private static SaveGameManager instance = null;
    private SaveGameManager() { }
    public static SaveGameManager Instance()
    {
        return instance ??= new SaveGameManager();
    }

    public bool saveLoaded = false;
    public Vector3 playerPosition = Vector3.zero;

    public void SaveGame(Transform playerTransform, List<Achievement> unlockedAchievements)
    {
        Scene activeScene = SceneManager.GetActiveScene();
        Debug.Log(activeScene.name);
        Debug.Log(playerTransform.position);

        var binaryFormatter = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + "/PlayerSaveData.txt");
        List<string> achievementIds = unlockedAchievements.ConvertAll(achievement => achievement.id);
        var data = new PlayerData
        {
            lastScene = JsonUtility.ToJson( new Vector2(activeScene.buildIndex, 0) ),
            position = JsonUtility.ToJson(playerTransform.position),
            unlockedAchievements = achievementIds
        };
        Debug.Log(data.lastScene);
        Debug.Log(data.position);

        binaryFormatter.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved at " + Application.persistentDataPath);
    }

    public PlayerData LoadGame()
    {
        var path = Application.persistentDataPath + "/PlayerSaveData.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = new FileStream(path, FileMode.Open);
            PlayerData data = formatter.Deserialize(file) as PlayerData;

            // Load unlocked achievements IDs and convert them back to achievements
            AchievementManager achievementManager = AchievementManager.instance;
            if (achievementManager != null && data.unlockedAchievementIds != null)
            {
                foreach (string achievementId in data.unlockedAchievementIds)
                {
                    Achievement achievement = achievementManager.GetAchievementById(achievementId);
                    if (achievement != null)
                    {
                        achievementManager.UnlockAchievement(achievement);
                    }
                }
            }

            Vector2 scene = JsonUtility.FromJson<Vector2>(data.lastScene);
            Vector3 position = JsonUtility.FromJson<Vector3>(data.position);
            SaveGameManager.Instance().playerPosition = position;
            SaveGameManager.Instance().saveLoaded = true;
            SceneManager.LoadScene(Convert.ToInt32(scene.x));
            file.Close();
            return data;
        }
        return null;
    }
}
