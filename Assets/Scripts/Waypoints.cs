using UnityEngine;

public class Waypoints : MonoBehaviour {
    public int index;
    public TrackCheck circuitTrackCheck;
    public WayPointsCircuit circuit;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning(other);

        NPCAgent ai = other.GetComponentInParent<NPCAgent>();
        if (ai != null)
        {
            circuitTrackCheck.AgentThroughCheckPoint(ai, index);
            return;
        }

        PlayerCar player = other.gameObject.GetComponent<PlayerCar>();
        Debug.LogWarning(index + "/" + player);

        if (player.currentWayPoint == index)
        {
            Debug.Log("player ha toqueteado Waypoint: " + index);

            circuitTrackCheck.AgentThroughCheckPoint(player, index);
            player.OnCorrectCheckpoint();
            return;
        }

    }


}