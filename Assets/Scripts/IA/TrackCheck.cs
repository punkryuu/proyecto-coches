using UnityEngine;
using System.Collections.Generic;
using System;
public class TrackCheck : MonoBehaviour

{
    public WayPointsCircuit circuit;
    private Dictionary<object, int> nextCheckpointIndex = new Dictionary<object, int>();

    public event Action<NPCAgent> OnCorrectCheckPointAI;
    public event Action<NPCAgent> OnWrongCheckPointAI;
    public event Action<PlayerCar> OnCorrectCheckPointPlayer;
    public event Action<PlayerCar> OnWrongCheckPointPlayer;

    public void AgentThroughCheckPoint(object car, int index)
    {      
        if (!nextCheckpointIndex.ContainsKey(car))
        {
            nextCheckpointIndex[car] = 0;
        }
        int expectedCheckpointIndex = nextCheckpointIndex[car];
        bool correct = index == expectedCheckpointIndex;

        if (correct)
        {
            nextCheckpointIndex[car]++;
            if(nextCheckpointIndex[car] >= circuit.GetWayPointsCount())
            {
                nextCheckpointIndex[car] = 0;
            }
            if(car is NPCAgent agent)
            {
                OnCorrectCheckPointAI?.Invoke(agent);
            }
            if(car is PlayerCar player)
            {
                OnCorrectCheckPointPlayer?.Invoke(player);
            }
        }
        else
        {
            if(car is NPCAgent agent)
            {
                OnWrongCheckPointAI?.Invoke(agent);
            }
            if(car is PlayerCar player)
            {
                OnWrongCheckPointPlayer?.Invoke(player);
            }
        }
    }

    public Transform GetNextCheckpoint(object car)
    {
        if (!nextCheckpointIndex.ContainsKey(car))
        {
            nextCheckpointIndex[car] = 0;
        }
        return circuit.GetWayPoint(nextCheckpointIndex[car]);
    }

    public void ResetCheckpoint(object car)
    {
        nextCheckpointIndex[car] = 0;
    }

    public Vector3 GetStartPosition()
    {
        return circuit.GetWayPoint(0).position;
    }
    public Quaternion GetStartRotation()
    {
        return circuit.GetWayPoint(0).rotation;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
