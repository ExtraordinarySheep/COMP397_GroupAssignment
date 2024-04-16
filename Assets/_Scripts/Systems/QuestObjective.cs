using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Quest Objective")]
public class QuestObjective : ScriptableObject
{
    public string ObjectiveName;
    public string ObjectiveDescription;
    public QuestEnums ObjectiveType;

    public int Progress;
    public int ProgressRequired;
    public bool Completed;
}
