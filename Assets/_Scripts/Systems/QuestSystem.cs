using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct QuestStep
{
    [SerializeField] public QuestObjective Objective;
    [SerializeField] public GameObject TargetObject;
}

public class QuestSystem : MonoBehaviour, IObserver
{
    [SerializeField] public string QuestName;

    [SerializeField] public Subject _player;
    [SerializeField] public List<QuestStep> _steps = new List<QuestStep>();
    private bool _completed = false;
    private int _currentStep = 0;

    private void Start()
    {
        for (int i = 0; i < _steps.Count; i++)
        {
            _steps[i].Objective.Progress = 0; // Stop from carrying over scenes
            _steps[i].Objective.Completed = false;
        }
    }

    void OnEnable()
    {
        if (_player == null) { return; }
        _player.AddObserver(this);
    }
    void OnDisable()
    {
        if (_player == null) { return; }
        _player.RemoveObserver(this);
    }

    public void OnNotify(SubjectEnums subjectEnum, List<System.Object> parameters)
    {
        if (subjectEnum == SubjectEnums.Quest)
        {
            // Parameters: [0] Verify the Name of Quest, [1] Add Progress if Verified
            if ((string) parameters[0] == QuestName)
            {
                MarkQuest((int) parameters[1]);
            }
        }
    }

    public QuestObjective GetCurrentObjective()
    {
        return _steps[_currentStep].Objective;
    }
    public GameObject GetCurrentTarget()
    {
        return _steps[_currentStep].TargetObject;
    }
    public void MarkQuest(int progress)
    {
        Debug.Log("Recieved Request");

        for (int i = 0; i < _steps.Count; i++)
        {
            Debug.Log("Step Completed: " + i);
            if (!_steps[i].Objective.Completed)
            {
                //_currentStep = i;
                Debug.Log("Add Progress");
                _steps[i].Objective.Progress += progress;
                if (_steps[i].Objective.Progress >= _steps[i].Objective.ProgressRequired)
                {
                    _steps[i].Objective.Completed = true;
                    _currentStep = i + 1;
                }
                Debug.Log(_steps[i].Objective.ObjectiveType);
                Debug.Log("Quest Updated: " + _currentStep);
                if (_currentStep >= _steps.Count)
                {
                    _currentStep = _steps.Count - 1;
                    _completed = true;
                    Debug.Log("Quest Completed!");
                }
                return;
            }
        }
    }

    public bool IsQuestComplete()
    {
        return _completed;
    }

}
