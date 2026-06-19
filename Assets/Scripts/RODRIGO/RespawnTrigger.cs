using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    WayPointsCircuit checkPoints;
    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();
        if (fsm != null)
        {
            fsm.Respawn();
        }

        IASINAPRENDIZAJE ia = other.GetComponentInParent<IASINAPRENDIZAJE>();
        if (ia != null)
        {
            Vector3 respawnPos = checkPoints.waypoints[88].position;
            ia.transform.position = respawnPos;
            ia.transform.rotation = checkPoints.waypoints[88].rotation;

        }
    }
}
