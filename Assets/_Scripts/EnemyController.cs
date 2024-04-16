using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;


public enum EnemyStates
{
    Patrolling,
    Chasing
}

public class EnemyController : MonoBehaviour
{
    [SerializeField] Subject _player;
    [SerializeField] private List<Transform> points;
    [SerializeField] private int index = 0;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask mask;
    [SerializeField] private int viewDistance = 10;
    [SerializeField] private bool inRange = false;
    Vector3 destination;
    NavMeshAgent agent;
    public EnemyStates enemyState = EnemyStates.Patrolling;
    private bool patrolDebounce = false; // Quest Related

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        destination = points[index].position;
        agent.destination = destination;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) > viewDistance) inRange = false;

        if (enemyState == EnemyStates.Chasing && inRange)
        {
            patrolDebounce = false;
            destination = player.position;
        }
        else
        {
            enemyState = EnemyStates.Patrolling;
            if (patrolDebounce == false)
            {
                //Debug.Log("Escaped!");

                patrolDebounce = true;
                // Signal Quest Objective
                //Debug.Log(_player.GetComponent<PlayerController>().activeQuest.GetCurrentObjective().ObjectiveType);
                if (_player.GetComponent<PlayerController>().activeQuest.GetCurrentObjective().ObjectiveType == QuestEnums.Escape) // If quest is on an escape objective, add progress.
                {
                    Debug.Log("Escape Achieved!");

                    List<System.Object> specifics = new List<System.Object>(); specifics.Add("Tutorial"); specifics.Add(1);

                    _player.NotifyObservers(SubjectEnums.Quest, specifics);
                }
            }
            if (Vector3.Distance(destination, agent.transform.position) < 1f)
            {
                index = (index + 1) % points.Count;
                destination = points[index].position;
            }
        }
        agent.destination = destination;

    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, viewDistance, mask))
        {
            if (hit.transform.gameObject.name.Equals("Player"))
            {
                inRange = true;
                enemyState = EnemyStates.Chasing;
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
        }
        else
        {
            
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * viewDistance, Color.red);
        }
    }
}
