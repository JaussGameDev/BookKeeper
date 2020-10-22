using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] bool isMoving = false;
    [SerializeField] Vector3 PreviousPosition = Vector3.zero;
    [SerializeField] Vector3 NextPosition = Vector3.zero;
    [SerializeField] Vector3 CurrSteeringDirection = Vector3.zero;
    [SerializeField] Vector3 SteeringDirection = Vector3.zero;
    [SerializeField] Transform aim = null;
    [SerializeField] float Angle = 0;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PreviousPosition = agent.transform.position;
        aim = new GameObject("aim").transform;
        NextPosition = PreviousPosition; 
        SteeringDirection = agent.transform.forward;
    }

    private void FixedUpdate()
    {   //Get the actual steering direction without Y axis
        CurrSteeringDirection = agent.transform.forward;
        CurrSteeringDirection.y = 0;
        //Set the aim position
        aim.position = agent.transform.position + SteeringDirection;
        //If agent is on position, set IsMoving to false
        if (Mathf.Abs(agent.transform.position.x - NextPosition.x) < 1f
            && Mathf.Abs(agent.transform.position.z - NextPosition.z) < 1f)
        {
            Debug.Log("PlayerMotor, FixedUpdate : First If");
            isMoving = false;
            Angle = Vector3.Angle(CurrSteeringDirection, SteeringDirection);
            if (Angle > 0.1f)
            {
                Debug.Log("PlayerMotor, FixedUpdate : First If, If");
                SteerAt();
            }
        }
        //if IsMoving = false and curr position != destination, then go to destination
        if ((Mathf.Round(agent.transform.position.x) != Mathf.Round(NextPosition.x)
           || Mathf.Round(agent.transform.position.z) != Mathf.Round(NextPosition.z)) && !isMoving)
        {
            Debug.Log("PlayerMotor, FixedUpdate : Second If");
            MoveToPoint(NextPosition, false);
        }
    }

    public void MoveToPoint(Vector3 point, bool value)
    {   //set previous and next position
        PreviousPosition = agent.transform.position;
        PreviousPosition.y = 0;
        NextPosition = point;
        NextPosition.y = 0;
        // Make the agent move
        agent.SetDestination(point);
        isMoving = true;
        //Set the steering direction if asked
        if (value) SetSteerToPoint(point);

    }
    public void SetSteerToPoint(Vector3 point)
    {
        // Set normalize steering direction withour Y axis
        SteeringDirection = (point - PreviousPosition);
        SteeringDirection.y = 0;
        SteeringDirection.Normalize();
    }
    private void SteerAt()
    {
        Debug.Log("PlayerMoto, SteerAt : aim = " + aim.localPosition);
        //Rotate the agent.
        agent.transform.LookAt(aim);
    }
    public Vector3 GetSteeringDirection()
    {
        return SteeringDirection;
    }


    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position if aim exist
        if (aim != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(aim.position + Vector3.up, 0.5f);
        }
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }
}
