using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : Subject
{
    // Singleton instance
    public static AchievementManager instance;

    // List of unlocked achievements
    private List<Achievement> unlockedAchievements = new List<Achievement>();

    // Dictionary to store achievements by their identifiers
    private Dictionary<string, Achievement> achievementsById = new Dictionary<string, Achievement>();

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Create the "Collector" achievement instance
        Achievement collectorAchievement = new Achievement("Collector", "Collector", "Collect an item.");

        // Register the "Collector" achievement with the AchievementManager
        AchievementManager.instance.AddAchievement("Collector", collectorAchievement);
    }

    // Method to unlock an achievement
    public void UnlockAchievement(Achievement achievement)
    {
        if (!achievement.unlocked)
        {
            achievement.unlocked = true;
            unlockedAchievements.Add(achievement);

            // Notify observers about the unlocked achievement
            NotifyObservers(SubjectEnums.Achievement, new List<Type>() { achievement.GetType() });
        }
    }

    // Method to check if an achievement is unlocked
    public bool IsAchievementUnlocked(Achievement achievement)
    {
        return unlockedAchievements.Contains(achievement);
    }

    // Method to add an achievement to the dictionary
    public void AddAchievement(string id, Achievement achievement)
    {
        achievementsById[id] = achievement;
    }

    // Method to retrieve an achievement by its identifier
    public Achievement GetAchievementById(string id)
    {
        Achievement achievement;
        if (achievementsById.TryGetValue(id, out achievement))
        {
            return achievement;
        }
        else
        {
            Debug.LogError("Achievement not found with ID: " + id);
            return null;
        }
    }
}
