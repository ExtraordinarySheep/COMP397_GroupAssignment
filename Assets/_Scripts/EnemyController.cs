using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class EnemyController : MonoBehaviour
{
    public StateMachine stateMachine;
    public StateMachine.State idle, chase; //add attack later 
    public GameObject player;
    public float senser = 10;
    public float speed = 2;
    float _objectFOV; 

    // Start is called before the first frame update
    void Start()
    {
        _objectFOV = Mathf.Cos(89 / 2f * Mathf.Deg2Rad); // in Rad
        stateMachine = new StateMachine();
        idle = stateMachine.CreateState("Idle");
        idle.onFrame = IdleOnFrame;

        chase = stateMachine.CreateState("Chase");
        chase.onFrame = ChaseOnFrame;


    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    //FSM:
    void IdleOnFrame()
    {
        //Do nothing for now

        //Idle -> chase
        if (SensePlayer(this.transform.position, player.transform.position, this.transform.forward, _objectFOV, senser))
        {
            //Debug.Log("Transitioning from Idle to Chase");
            stateMachine.ChangeState(chase);
        }

    }
    void ChaseOnFrame()
    {
        //Debug.Log("Chase.onFrame");
        Chase();

        // Look at the player
        transform.LookAt(player.transform);

        //Transition to chase ->idle
        if (!WithinRange(this.transform.position, player.transform.position, senser))
        {
            //Debug.Log("Transitioning from Chase to Idle");
            stateMachine.ChangeState(idle);
        }

        //add transition chase ->attack
    }
    private void Chase()
    {
        Vector3 playerHeading = (player.transform.position - this.transform.position);
        float playerDistance = playerHeading.magnitude;
        playerHeading.Normalize();

        Vector3 movement = playerHeading * speed * Time.deltaTime; //m/s but we want m/frame so multiply by s/frame
        Vector3.ClampMagnitude(movement, playerDistance);
        this.transform.position += movement;
    }
    public static bool SensePlayer(Vector3 start, Vector3 playerPos, Vector3 thisForward, float cutOff, float distance)
    {

        Vector3 playerHeading = (playerPos - start).normalized;
        float cosAngle = Vector3.Dot(playerHeading, thisForward);

        if (cosAngle > cutOff && Vector3.Distance(start, playerPos) <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public static bool WithinRange(Vector3 start, Vector3 playerPos, float distance)
    {
        if (Vector3.Distance(start, playerPos) <= distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
