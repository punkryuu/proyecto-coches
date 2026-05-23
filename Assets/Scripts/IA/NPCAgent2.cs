using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class NPCAgent2 : Agent
{
    [SerializeField] Rigidbody rb;
    public Transform spawnPoint;
    public TrackCheck trackCheck;
    public LayerMask raycastMask;

    public float maxSpeed = 20f;
    public float accelerationForce = 40f;
    public float turnSpeed = 200f;

    public float rayLength = 15f;
    public Vector3[] rayDirections;

    float stuckTimer = 0f;
    Vector3 lastPos;

    Transform currentCheckpoint;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        trackCheck = FindAnyObjectByType<TrackCheck>();

        if (spawnPoint == null)
        {
            GameObject sp = new GameObject("AutoSpawn_" + name);
            sp.transform.position = transform.position;
            sp.transform.rotation = transform.rotation;
            spawnPoint = sp.transform;
        }

        //trackCheck.OnCorrectCheckPointAI += OnPassedCheckpoint;
        //trackCheck.OnWrongCheckPointAI += OnWrongCheckpoint;

        lastPos = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        if (spawnPoint == null) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = spawnPoint.position + Vector3.up * 0.5f;
        transform.rotation = spawnPoint.rotation;

        trackCheck.ResetCheckpoint(this);

        stuckTimer = 0f;
        lastPos = transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        currentCheckpoint = trackCheck.GetNextCheckpoint(this);

        // 1. �ngulo hacia el checkpoint
        Vector3 toCheckpoint = currentCheckpoint.position - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, toCheckpoint, Vector3.up) / 180f;
        sensor.AddObservation(angle);

        // 2. Velocidad local
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        sensor.AddObservation(localVel / maxSpeed);

        // 3. Raycasts
        foreach (var dir in rayDirections)
        {
            Vector3 worldDir = transform.TransformDirection(dir.normalized);
            if (Physics.Raycast(transform.position, worldDir, out RaycastHit hit, rayLength, raycastMask))
                sensor.AddObservation(hit.distance / rayLength);
            else
                sensor.AddObservation(1f);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float accel = Mathf.Clamp(actions.ContinuousActions[0], 0f, 1f);
        float steer = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        currentCheckpoint = trackCheck.GetNextCheckpoint(this);
        Vector3 toCheckpoint = (currentCheckpoint.position - transform.position).normalized;

        // MOVIMIENTO
        rb.AddForce(transform.forward * accel * accelerationForce, ForceMode.Acceleration);

        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        rb.AddTorque(Vector3.up * steer * turnSpeed, ForceMode.Acceleration);

        // ==========================
        // RECOMPENSAS
        // ==========================

        // 1. Progreso hacia el checkpoint
        float forwardProgress = Vector3.Dot(rb.linearVelocity, toCheckpoint);
        AddReward(forwardProgress * 0.001f);

        // 2. Alineaci�n con la curva
        float angle = Vector3.SignedAngle(transform.forward, toCheckpoint, Vector3.up);
        float angleReward = 1f - Mathf.Abs(angle) / 180f;
        AddReward(angleReward * 0.002f);

        // 3. Mantenerse en el carril
        float lateralOffset = Vector3.Dot(transform.position - currentCheckpoint.position, currentCheckpoint.right);
        AddReward(-Mathf.Abs(lateralOffset) * 0.002f);

        // 4. Penalizar quedarse atascado
        stuckTimer += Time.fixedDeltaTime;
        if (Vector3.Distance(transform.position, lastPos) > 1f)
        {
            stuckTimer = 0f;
            lastPos = transform.position;
        }

        if (stuckTimer > 5f)
        {
            AddReward(-0.5f);
            EndEpisode();
        }

        // 5. Penalizar choque frontal
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, 2f, raycastMask))
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    void OnPassedCheckpoint(NPCAgent agent)
    {
        AddReward(2f);
    }

    void OnWrongCheckpoint(NPCAgent agent)
    {
        AddReward(-0.5f);
        EndEpisode();
    }
}