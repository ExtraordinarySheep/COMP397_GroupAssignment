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
        //achievements = AchievementManager.instance.GetAchievements();

        //foreach (Achievement achievement in achievements)
        //{
        //    GameObject item = Instantiate(AchievementListItemPrefab);
        //    item.GetComponent<AchievementListItem>().name.text = achievement.name;
        //    item.GetComponent<AchievementListItem>().desc.text = achievement.description;
        //    item.transform.SetParent(this.transform);
        //}
    }
}
