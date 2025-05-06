using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBlockMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> wayPointsList = new List<Transform>();
    [SerializeField] private Transform currentWaypoint;
    [SerializeField] private int wayPointNumber = 0;
    [SerializeField] private float speed = 3f;
    private void Start()
    {
        currentWaypoint = wayPointsList[wayPointNumber];
    }
    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        float distanceToCurrent = Vector2.Distance(transform.position, currentWaypoint.position);
        Vector3 relativePos = currentWaypoint.position - transform.position;
        if (distanceToCurrent == 0)
        {
            if (wayPointNumber != wayPointsList.Count - 1)
            {
                wayPointNumber++;
                currentWaypoint = wayPointsList[wayPointNumber];
            }
            else
            {
                wayPointNumber = 0;
                currentWaypoint = wayPointsList[wayPointNumber];
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, speed * Time.deltaTime);
    }
}