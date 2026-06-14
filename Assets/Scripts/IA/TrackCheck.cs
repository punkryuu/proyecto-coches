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
       // Debug.Log($"Entró checkpoint {index}, esperado {nextCheckpointIndex[car]}");
        if (circuit == null)
        {
            Debug.LogError("TrackCheck.circuit ES NULL");
        }
        else
        {
            Debug.Log("TrackCheck.circuit OK");
        }
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

    public Transform GetCurrentCheckpoint(object car)
    {
        if (!nextCheckpointIndex.ContainsKey(car))
            nextCheckpointIndex[car] = 0;

        return circuit.GetWayPoint(nextCheckpointIndex[car]);
    }
    public Transform GetNextCheckpoint(object car)
    {
        if (!nextCheckpointIndex.ContainsKey(car))
        {
            nextCheckpointIndex[car] = 0;

        }
        return circuit.GetWayPoint(nextCheckpointIndex[car]);
    }
    public Transform GetLastCheckpoint(NPCAgent agent)
    {
        int index = nextCheckpointIndex[agent] - 1;

        if (index < 0)
            return null;

        return circuit.GetWayPoint(index);
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
    public Transform GetNextNextCheckpoint(object car)
    {
        if (!nextCheckpointIndex.ContainsKey(car))
            nextCheckpointIndex[car] = 0;

        int nextIndex = nextCheckpointIndex[car] + 1;

        if (nextIndex >= circuit.GetWayPointsCount())
            nextIndex = 0;

        return circuit.GetWayPoint(nextIndex);
    }

    public bool HasCompletedLap(Component car)
    {
        if (!nextCheckpointIndex.ContainsKey(car))
            return false;

        return nextCheckpointIndex[car] >= circuit.GetWayPointsCount();
    }
}
