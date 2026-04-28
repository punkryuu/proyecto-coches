using UnityEngine;
using System.Collections.Generic;

public class CarIdetifier : MonoBehaviour
{
    public bool isPlayer;
    public int currentLap = 0;
    public int currentWayPoint = 0;
    public int totalWaypoints;
    public float distanceToNextWayPoint = 0f;

    public WayPointsCircuit circuit;

    void Start()
    {
        totalWaypoints = circuit.GetWayPointsCount();
    }
    void Update()
    {
        UpdateDistance();
    }
    void UpdateDistance()
    {
        if (circuit == null)
        {
            Debug.LogError("Circuit reference is missing!");
            return;
        }
        Transform nextWayPoint = circuit.GetWayPoint(currentWayPoint);
        distanceToNextWayPoint = Vector3.Distance(transform.position, nextWayPoint.position);
    }
}
    



