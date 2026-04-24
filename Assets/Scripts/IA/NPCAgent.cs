using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NPCAgent:Agent
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform npcPosition;
    [SerializeField] LayerMask raycastMask;
}
