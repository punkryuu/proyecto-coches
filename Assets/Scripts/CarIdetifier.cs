using UnityEngine;
using System.Collections.Generic;

public class PlayerCar : MonoBehaviour
{
    public bool isPlayer;
    public int currentLap = 0;
    public int currentWayPoint = 0;
    public int totalWaypoints;
    public float distanceToNextWayPoint = 0f;
    public PersonajeSO personajeData;
    RaceManager modoCarrera;
    public Transform modelParent;
    public WayPointsCircuit circuit;
    public TrackCheck trackCheck;
    public bool canMove = false;

    void Start()
    {
        if (circuit == null)
        {
            circuit = FindFirstObjectByType<WayPointsCircuit>();
            if (circuit == null)
            {
                Debug.LogError("PlayerCar: circuit no asignado");
                return;
            }
        }

        totalWaypoints = circuit.waypoints.Length;
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
    public void OnCorrectCheckpoint()
    {
        currentWayPoint++;
      

    }

    public void OnWrongCheckpoint()
    {
        Debug.Log("Checkpoint incorrecto");
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void ResetProgress()
    {
        currentWayPoint = 0;
        currentLap = 0;
    }

}
    



