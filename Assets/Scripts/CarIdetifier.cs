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
    bool activated = false;
    public Transform currentCheckpoint;
    public WayPointsCircuit circuit;

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
    public void OnCorrectCheckpoint()
    {
        currentWayPoint++;

        if (CompareTag("NPC"))
        {
            NPCAgent npc = GetComponent<NPCAgent>();

            if (npc != null)
            {
                npc.OnPassedCheckpoint(npc);
            }
        }

    }

    public void OnWrongCheckpoint()
    {
        Debug.Log("Checkpoint incorrecto");
    }

    public void ResetProgress()
    {
        currentWayPoint = 0;
        currentLap = 0;
    }

   public void ResetFromFalling()
        {
        Transform spawnPoint = circuit.GetWayPoint(currentWayPoint);
        transform.position = spawnPoint.position + Vector3.up * 0.5f;
        transform.rotation = spawnPoint.rotation;
    }

    public void OnLapCompleted()
    {
        currentLap++;
        Debug.Log("Player completed lap " + currentLap);
    }
}
    



