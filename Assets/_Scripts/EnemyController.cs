using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private List<Transform> points;
    [SerializeField] private int index = 0;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask mask;
    [SerializeField] private int viewDistance = 10;
    [SerializeField] private bool inRange = false;
    Vector3 destination;
    NavMeshAgent agent;
    EnemyStates enemyState = EnemyStates.Patrolling;

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
            destination = player.position;
        }
        else
        {
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
