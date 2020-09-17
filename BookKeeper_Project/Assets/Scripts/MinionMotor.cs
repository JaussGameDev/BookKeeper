using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MinionMotor : MonoBehaviour
{
    [SerializeField] Vector3 destination = Vector3.zero;
    [SerializeField] Minion minion = null;
    [SerializeField] bool isMoving = false;
    [SerializeField] private Transform aim = null;
    [SerializeField] private Vector3 steeringDirection = Vector3.zero;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        minion = GetComponent<Minion>();
        destination = transform.position;
        aim = new GameObject("aim").transform;
    }
    private void Update()
    {
        aim.position = agent.transform.position + steeringDirection;

        //If agent is on position, set IsMoving to false
        if (Mathf.Abs(agent.transform.position.x - destination.x) <= 0.2f
           && Mathf.Abs(agent.transform.position.z - destination.z) <= 0.2f
            && destination != Vector3.zero)
        {
            isMoving = false;
            SteerAt(aim);
        }

        //if IsMoving = false dans curr position != destination then go there
        if ((Mathf.Abs(agent.transform.position.x - destination.x) > 0.2f
           || Mathf.Abs(agent.transform.position.z - destination.z) > 0.2f)
            && destination != Vector3.zero && !isMoving)
        {
            isMoving = true;
            MoveToPoint(destination);
        }

    }

    public void MoveToPoint (Vector3 point)
    {
        if (minion.GetIsHome()) { return; }
        destination = point;
        agent.SetDestination(destination);
        isMoving = true;
    }
    public void SteerAt(Transform aim)
    {
        Debug.Log("MinionMotor, SteerAt" + aim.position);
        //Rotate the agent.
        var angle = Vector3.Angle(agent.transform.position, steeringDirection);
        if (angle > 0.2f)
        {
            agent.transform.LookAt(aim);
            Debug.Log("MinionMotor, SteerAt : aim = " + aim.position + ", angle = " + angle);
        }
    }
    public void SetSteering(Vector3 dir)
    {
        steeringDirection = dir;
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position if aim exist
        if (aim != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(aim.position, 0.3f);
        }
    }
    public bool GetIsMoving()
    {
        return isMoving;
    }

    public void SetAgentRadius(float value)
    {
        agent.radius = value;
    }
}

