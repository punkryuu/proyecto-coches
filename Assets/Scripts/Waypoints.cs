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
        PlayerCar car = other.GetComponentInParent<PlayerCar>();

        if (car == null)
            return;

        if (car.currentWayPoint != index)
            return;

        car.OnCorrectCheckpoint();

    }


}