using UnityEngine;

public class Waypoints : MonoBehaviour {
    public int index;
    public TrackCheck circuitTrackCheck;
    public WayPointsCircuit circuit;

    private void Awake()
    {
        if (circuitTrackCheck == null)
        {
            circuitTrackCheck = FindAnyObjectByType<TrackCheck>();
        }
       
        if (circuit == null)
        {
            circuit = FindAnyObjectByType<WayPointsCircuit>();
        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
        IASINAPRENDIZAJE racer = other.GetComponentInParent<IASINAPRENDIZAJE>();
        if (racer != null)
        {
            Debug.Log("IA ha toqueteado Waypoint: " + index);
            circuitTrackCheck.AgentThroughCheckPoint(racer, index);
            return;
        }

        NPCAgent ai = other.GetComponentInParent<NPCAgent>();
        if (ai != null)
        {
            circuitTrackCheck.AgentThroughCheckPoint(ai, index);
            return;
        }

        PlayerCar player = other.GetComponentInParent<PlayerCar>();
        if (player!= null && player.currentWayPoint == index)
        {
            Debug.Log("player ha toqueteado Waypoint: " + index);

            circuitTrackCheck.AgentThroughCheckPoint(player, index);
            player.OnCorrectCheckpoint();
            return;
        }

    }


}