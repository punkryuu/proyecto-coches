using UnityEngine;

public class Waypoints : MonoBehaviour {
    public int index;
    public TrackCheck circuitTrackCheck;
    public WayPointsCircuit circuit;

    private void OnTriggerEnter(Collider other)
    {
        NPCAgent ai = other.GetComponentInParent<NPCAgent>();
        if (ai != null)
        {
            circuitTrackCheck.AgentThroughCheckPoint(ai, index);
            return;
        }

        PlayerCar player = other.GetComponentInParent<PlayerCar>();
        if (player.currentWayPoint == index)
        {
            circuitTrackCheck.AgentThroughCheckPoint(player, index);
            player.OnCorrectCheckpoint();
            return;
        }
    }
}