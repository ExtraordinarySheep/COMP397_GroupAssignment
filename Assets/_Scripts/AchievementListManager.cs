using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementListManager : MonoBehaviour
{
    List<Achievement> achievements;

    public GameObject AchievementListItemPrefab;
    void Start()
    {
        PlayerData data = SaveGameManager.Instance().LoadGame();

        achievements = AchievementManager.instance.GetUnlockedAchievements();

        foreach (Achievement achievement in achievements)
        {
            GameObject item = Instantiate(AchievementListItemPrefab);
            item.GetComponent<AchievementListItem>().name.text = achievement.name;
            item.GetComponent<AchievementListItem>().desc.text = achievement.description;
            item.transform.SetParent(this.transform);
            item.transform.localScale = Vector3.one;
            item.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        }
    }
}
