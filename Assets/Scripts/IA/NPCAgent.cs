using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NPCAgent:Agent
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform npcPosition;
    [SerializeField] Waypoints waypoints;
    [SerializeField] LayerMask raycastMask;

    public float rayLength = 15f; //Distancia máxima de los rayos
    public Vector3[] rayDirections;

    public float stuckTimeLimit = 4f;
    public float timeSinceLastProgress = 0f;
    private Vector3 lastPosition;

    private Transform currentCheckPoint;

    public override void Initialize()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
        lastPosition = npcPosition.position;
       
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
