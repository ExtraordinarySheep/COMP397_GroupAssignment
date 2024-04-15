using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] public string QuestName;

    [SerializeField] public Subject _player;
    [SerializeField] public List<QuestObjective> _steps = new List<QuestObjective>();
    private bool _completed = false;
    private int _currentStep = 0;

    public QuestObjective GetCurrentObjective()
    {
        return _steps[_currentStep];
    }
    public void MarkQuest(int progress)
    {
        for (int i = 0; i < _steps.Count; i++)
        {
            if (!_steps[i].Completed)
            {
                _currentStep = i;
                _steps[i].Progress += progress;
                if (_steps[i].Progress >= _steps[i].ProgressRequired)
                {
                    _completed = true;
                    _currentStep = i + 1;
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
