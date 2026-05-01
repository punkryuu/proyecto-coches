using System;
using UnityEngine;

public class WayPointsCircuit : MonoBehaviour
{
   public Transform[] waypoints;
   public Transform GetWayPoint(int index)
    {
        return waypoints[index%waypoints.Length];
    }
    public int GetWayPointsCount()
    {
        return waypoints.Length;
    }

}
