using Mono.Cecil.Cil;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class NPCAgent : Agent
{
    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider hitBox;
    [SerializeField] Transform npcPosition;
    [SerializeField] public Transform spawnPoint;
    [SerializeField] PlayerCar playerCar;
    [SerializeField] public TrackCheck trackCheck;
    [SerializeField] LayerMask raycastMask;
    [SerializeField] LayerMask groundLayer;

    [SerializeField] float maxSpeed = 40f;
    [SerializeField] float accelerationForce = 40f;
    [SerializeField] float brakeForce = 20f;
    [SerializeField] float driftForce = 15f;
    [SerializeField] float driftPower = 20f;
    [SerializeField] float driftDirection = 1f;
    [SerializeField] float driftSideForce = 15f;
    [SerializeField] float driftFrictionPower = 2f;

    private float rayDistance = 2f;

    bool accelerateInput = false;


    public float rayLength = 15f; //Distancia máxima de los rayos
    public Vector3[] rayDirections;

    public float stuckTimeLimit = 50f;
    public float timeSinceLastProgress = 0f;
    public float turnSpeed = 80f;
    private Vector3 lastPosition;
    private float previousDistanceToCheckpoint = 0f;
    private Transform nextCheckPoint;

    public float power = 0f;
    public float maxPower = 100f;

    public Transform currentCheckPoint;
    public int spawnIndex;

    public bool triggerBoost;
    public float triggerBoostDuration;
    public float gravityMultiplier = 1f;
    private float gravityForce = 100f;
    private float episodeTimer = 0f;
    private float maxEpisodeTime = 120f; // 2 minutos

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<CapsuleCollider>();
        npcPosition = transform;
        // spawnPoint = npcPosition;
        playerCar = GetComponent<PlayerCar>();
        trackCheck = FindFirstObjectByType<TrackCheck>();

        lastPosition = npcPosition.position;
        MaxStep = 100000;

        trackCheck.OnCorrectCheckPointAI += OnPassedCheckpoint;
        trackCheck.OnWrongCheckPointAI += OnWrongCheckpoint;

    }
    void FixedUpdate()
    {
        if (!CheckGrounded())
        {
            ApplyGravity();
        }

    }

    private void Update()
    {
        episodeTimer += Time.deltaTime;

        if (episodeTimer >= maxEpisodeTime)
        {
            AddReward(-0.5f);   // castigo suave por no completar
            EndEpisode();
        }
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("EPISODE BEGIN");

        episodeTimer = 0f;

        playerCar.currentWayPoint = 0;

        rb.isKinematic = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = spawnPoint.position + Vector3.up * 0.5f;
        rb.rotation = spawnPoint.rotation;

        Physics.SyncTransforms();

        rb.isKinematic = false;

        rb.linearDamping = 0.5f;
        rb.angularDamping = 3f;

        rb.Sleep();
        rb.WakeUp();

        trackCheck.ResetCheckpoint(this);

        timeSinceLastProgress = 0f;

        lastPosition = rb.position;

        currentCheckPoint = trackCheck.GetNextCheckpoint(this);

        if (currentCheckPoint != null)
        {
            previousDistanceToCheckpoint =
                Vector3.Distance(rb.position, currentCheckPoint.position);
        }
    

}
    public void Move(float steer, int movement)
    {
        // GIRO
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steer * turnSpeed * Time.fixedDeltaTime, 0f));

        // MOVIMIENTO
        switch (movement)
        {
            case 0:
                break;

            case 1:
                ApplyAccelerationNPC();
                break;

            case 2:
                ApplyBrakeNPC();
                break;

            case 3:
                ApplyDrift(steer);
                break;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        
        currentCheckPoint = trackCheck.GetNextCheckpoint(this);


        // Track direction in LOCAL SPACE
        Vector3 dirToCheckpoint = (currentCheckPoint.position - transform.position).normalized;

        Vector3 localTrackDir = transform.InverseTransformDirection(dirToCheckpoint);

        // Local velocity
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);

        // Angular speed
        float angularSpeed = rb.angularVelocity.y;

        // Ground check
        bool grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1f);


        // ===== RAYCASTS =====

        foreach (var direction in rayDirections)
        {
            Vector3 worldDir = transform.TransformDirection(direction.normalized);

            if (Physics.Raycast(transform.position, worldDir, out RaycastHit hit, rayLength, raycastMask))
            {
                sensor.AddObservation(hit.distance / rayLength);
            }
            else
            {
                sensor.AddObservation(1f);
            }
        }

        // ===== OBSERVATIONS =====

        Vector3 toCheckpoint = currentCheckPoint.position - transform.position;
        float angleToCheckpoint = Vector3.SignedAngle(transform.forward, toCheckpoint, Vector3.up) / 180f;
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);

        sensor.AddObservation(localVelocity.x / maxSpeed); // drift lateral
        sensor.AddObservation(localVelocity.z / maxSpeed); // forward speed
        sensor.AddObservation(angleToCheckpoint);
        sensor.AddObservation(localTrackDir);       // 3
        sensor.AddObservation(localVel / maxSpeed); // 3
        sensor.AddObservation(angularSpeed / 10f);  // 1
        sensor.AddObservation(grounded ? 1f : 0f); // 1
    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.0005f);
        float steer = actions.ContinuousActions[0];
        int movementAction = actions.DiscreteActions[0];

        currentCheckPoint = trackCheck.GetNextCheckpoint(this);
        nextCheckPoint = trackCheck.GetNextNextCheckpoint(this);

        Vector3 toNext = (currentCheckPoint.position - npcPosition.position).normalized;
        Vector3 velocity = rb.velocity;


        // --- MOVIMIENTO ---


        accelerateInput = (movementAction == 1);

        Move(steer, movementAction);

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
        float forwardSpeed = Vector3.Dot(rb.velocity.normalized, (currentCheckPoint.position - transform.position).normalized);
        AddReward(forwardSpeed * 0.01f);

        Vector3 toCheckpoint = (currentCheckPoint.position - transform.position).normalized;
        float alignment = Vector3.Dot(transform.forward, toCheckpoint);

        // recompensa clara por mirar hacia donde debe ir
        AddReward(alignment * 0.01f);

        timeSinceLastProgress += Time.fixedDeltaTime;

        if (timeSinceLastProgress > stuckTimeLimit)
        {
            AddReward(-5f);
           // EndEpisode();

        }

    }

    public void OnPassedCheckpoint(NPCAgent agent)
    {
        Debug.Log("Checkpoint pasasdo: " + agent.currentCheckPoint.name);
        agent.currentCheckPoint = agent.trackCheck.GetNextCheckpoint(agent);
        agent.previousDistanceToCheckpoint = Vector3.Distance( agent.npcPosition.position, agent.currentCheckPoint.position);

        agent.timeSinceLastProgress = 0f;
        agent.AddReward(7f);

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
            AddReward(-2f);
            EndEpisode();

        }
    }

    public void OnLapCompleted()
    {
        AddReward(1.0f);   // recompensa por completar vuelta
        EndEpisode();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & raycastMask) != 0)
        {
            Debug.Log("muro tocando" + Time.fixedDeltaTime);
            AddReward(-0.1f * Time.fixedDeltaTime);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & raycastMask) != 0)
        {
            Debug.Log("muro salido");
            AddReward(0.1f);

        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        var discrete = actionsOut.DiscreteActions;

        // Igual que tu FSM
        bool accelerate = Input.GetKey(KeyCode.W);
        bool brake = Input.GetKey(KeyCode.S);
        bool drift = Input.GetKey(KeyCode.LeftShift);
        float steer = Input.GetAxis("Horizontal");

        // ACCIÓN CONTINUA: dirección
        continuous[0] = steer;

        // ACCIÓN DISCRETA:
        // 0 = idle
        // 1 = accelerate
        // 2 = brake
        // 3 = drift
        int movementAction = 0;

        if (drift && Mathf.Abs(steer) > Mathf.Epsilon)
            movementAction = 3;
        else if (brake)
            movementAction = 2;
        else if (accelerate)
            movementAction = 1;

        discrete[0] = movementAction;
    }
    public void Respawn()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        this.transform.position = new Vector3(CheckPoints.GetActiveCheckPointPosition().x, CheckPoints.GetActiveCheckPointPosition().y + 0.5f, CheckPoints.GetActiveCheckPointPosition().z);
    }

    private void ApplyAccelerationNPC(float power = -1f)
    {
        if (power < 0) power = accelerationForce;

        if (rb.velocity.magnitude < maxSpeed)
            rb.AddForce(hitBox.transform.forward * power, ForceMode.Acceleration);
    }


    private void ApplyBrakeNPC(float power = -1f)
    {
        if (power < 0) power = brakeForce;

        if (rb.velocity.magnitude < maxSpeed)
            rb.AddForce(-hitBox.transform.forward * power, ForceMode.Acceleration);
    }

    private void ApplyDrift(float steer)
    {
        float driftRotation = steer * driftPower;
        hitBox.transform.localRotation = Quaternion.Slerp(hitBox.transform.localRotation, Quaternion.Euler(0, driftRotation * driftDirection, 0), Time.deltaTime * 1.5f
);
        Vector3 forwardVel = hitBox.transform.forward * rb.velocity.magnitude;
        rb.velocity = Vector3.Lerp(rb.velocity, forwardVel, Time.deltaTime * 2f);
        if (accelerateInput)
        {
            rb.AddForce(hitBox.transform.forward * accelerationForce * 0.6f, ForceMode.Acceleration);
        }
        rb.AddForce(hitBox.transform.right * steer * driftSideForce, ForceMode.Acceleration);
        ApplyLateralFriction(driftFrictionPower);
        RotateHitboxDrift();
    }
    private void ApplyLateralFriction(float friction)
    {
        Vector3 lateral = Vector3.Project(rb.velocity, transform.right);
        rb.velocity -= lateral * friction * Time.deltaTime;
    }
    private void RotateHitboxDrift()
    {

        hitBox.transform.localRotation = Quaternion.Lerp(
            hitBox.transform.localRotation,
            Quaternion.identity,
            Time.deltaTime * 5f
        );
    }
    public Vector3[] GroundRaycasts()
    {
        float r = hitBox.radius;
        float h = hitBox.height;
        return new Vector3[]
        {
            new Vector3(-r, 0,  h/2),
            new Vector3( r, 0,  h/2),
            new Vector3(-r, 0, -h/2),
            new Vector3( r, 0, -h/2)
        };
    }

    public bool CheckGrounded()
    {
        foreach (Vector3 offset in GroundRaycasts())
        {
            Vector3 origin = hitBox.transform.TransformPoint(offset);
            if (Physics.Raycast(origin, -hitBox.transform.up, rayDistance, groundLayer))
            {
                return true;
            }
        }
        return false;
    }
    public void ApplyGravity()
    {
        rb.AddForce(gravityForce * gravityMultiplier * Vector3.down, ForceMode.Acceleration);
    }
}
