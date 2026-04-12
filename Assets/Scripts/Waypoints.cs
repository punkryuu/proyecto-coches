using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public int index;
    public WayPointsCircuit circuit;

    private void OnTriggerEnter(Collider other)
    {
        CarIdetifier car = other.GetComponentInParent<CarIdetifier>();
        if (car == null)
        {
          return;
        }
         if (car.currentWayPoint == index)
            {
            car.currentWayPoint++;
                
            /*if(car.currentWayPoint >= circuit.GetWayPointsCount())
             {
                car.currentWayPoint = 0;
                car.currentLap++;
            }*/
        }
    }
}

