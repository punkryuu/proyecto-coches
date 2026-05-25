using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class NPCAgent : Agent
{
    [SerializeField] Rigidbody rb;
    Transform npcPosition;
    public Transform spawnPoint;
    PlayerCar playerCar;
    [SerializeField] public TrackCheck trackCheck;
    [SerializeField] LayerMask raycastMask;

    float maxSpeed = 20f;
    float accelerationForce = 40f;
    float brakeForce = 20f;

    public float rayLength = 15f; //Distancia máxima de los rayos
    public Vector3[] rayDirections;

    public float stuckTimeLimit = 10f;
    public float timeSinceLastProgress = 0f;
    public float turnSpeed = 80f;
    private Vector3 lastPosition;
    private float previousDistanceToCheckpoint = 0f;
    private Transform nextCheckPoint;

    public float power = 0f;
    public float maxPower = 100f;

    private Transform currentCheckPoint;
    public int spawnIndex;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        npcPosition = transform;
       // spawnPoint = npcPosition;
        playerCar = GetComponent<PlayerCar>();
        trackCheck = FindFirstObjectByType<TrackCheck>();

        lastPosition = npcPosition.position;
        MaxStep = 5000;

        trackCheck.OnCorrectCheckPointAI += OnPassedCheckpoint;
        trackCheck.OnWrongCheckPointAI += OnWrongCheckpoint;

    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

       npcPosition.position = spawnPoint.position + Vector3.up * 0.5f;
       npcPosition.rotation = spawnPoint.rotation;

       


        trackCheck.ResetCheckpoint(this);
        timeSinceLastProgress = 0f;
        lastPosition = npcPosition.position;
        previousDistanceToCheckpoint = Vector3.Distance(
        npcPosition.position,
        trackCheck.GetNextCheckpoint(this).position);
     
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        currentCheckPoint = trackCheck.GetNextCheckpoint(this);

        // Track direction in LOCAL SPACE
        Vector3 dirToCheckpoint = (currentCheckPoint.position - transform.position).normalized;

        Vector3 localTrackDir = transform.InverseTransformDirection(dirToCheckpoint);

        // Local velocity
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);

        // Angular speed
        float angularSpeed = rb.angularVelocity.y;

        // Ground check
        bool grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1f);

        rb.linearDamping = 0.5f;
        rb.angularDamping = 3f;

        // ===== RAYCASTS =====

        foreach (var direction in rayDirections)
        {
            Vector3 worldDir =
                transform.TransformDirection(direction.normalized);

            if (Physics.Raycast(transform.position,worldDir, out RaycastHit hit, rayLength,raycastMask))
            {
                sensor.AddObservation(hit.distance / rayLength);
            }
            else
            {
                sensor.AddObservation(1f);
            }
        }

        // ===== OBSERVATIONS =====

        
        sensor.AddObservation(localTrackDir);       // 3
        sensor.AddObservation(localVel / maxSpeed); // 3
        sensor.AddObservation(angularSpeed / 10f);  // 1
        sensor.AddObservation(grounded ? 1f : 0f); // 1
    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.0005f);
        float accel = Mathf.Clamp(actions.ContinuousActions[0], 0f, 1f);
        float steer = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        currentCheckPoint = trackCheck.GetNextCheckpoint(this);
        nextCheckPoint = trackCheck.GetNextNextCheckpoint(this);

        Vector3 toNext = (currentCheckPoint.position - npcPosition.position).normalized;
        Vector3 velocity = rb.linearVelocity;


        // --- MOVIMIENTO ---
        rb.AddForce(npcPosition.forward * accel * accelerationForce, ForceMode.Acceleration);
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steer * turnSpeed * Time.fixedDeltaTime, 0f));

        // ============================================================
        // ===================== RECOMPENSAS ===========================
        // ============================================================

        Collider[] nearby = Physics.OverlapSphere(transform.position, 1.5f);

        foreach (var c in nearby)
        {
            if (c.attachedRigidbody != null && c.attachedRigidbody != rb)
            {
                AddReward(-0.01f);
            }
        }
        Vector3 trackForward = currentCheckPoint.forward;

        timeSinceLastProgress += Time.fixedDeltaTime;


        if (timeSinceLastProgress > stuckTimeLimit)
        {
            AddReward(-2f);
            //EndEpisode();

        }

        if (transform.position.y < -200f)
        {

            EndEpisode();
        }

    }


    public void OnPassedCheckpoint(NPCAgent agent)
    {
        // Resetear distancia para evitar recompensas negativas gigantes
        previousDistanceToCheckpoint = Vector3.Distance(npcPosition.position, currentCheckPoint.position);
        timeSinceLastProgress = 0f;
        AddReward(7f); // recompensa fuerte por checkpoint
       
    }

    public void OnWrongCheckpoint(NPCAgent agent)
    {
        AddReward(-1f); // penalización por checkpoint incorrecto
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & raycastMask) != 0)
        {
            Debug.Log("muro tocando2");
            AddReward(-5f);
            EndEpisode();

        }

        if (collision.gameObject.CompareTag("META"))
        {
            AddReward(3f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & raycastMask) != 0)
        {
            Debug.Log("muro tocando" + Time.fixedDeltaTime);
            AddReward(-0.1f * Time.fixedDeltaTime);
            
        }
    }
}
