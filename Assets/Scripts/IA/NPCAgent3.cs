using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DecisionRequester))]
public class NPCAgent3 : Agent
{
    // Reward configuration for the agent's learning
    [System.Serializable]
    public class RewardInfo
    {
        public float no_movement = -0.1f;      // Penalty for not moving
        public float mult_forward = 0.01f;    // Reward multiplier for moving forward
        public float mult_backward = -0.01f;  // Penalty multiplier for moving backward
        public float mult_barrier = -0.8f;     // Penalty for hitting barriers
        public float mult_car = -0.5f;         // Penalty for car collisions
        public float timeStepPenalty = -0.001f;
    }

    // Movement and control settings
    public float Movespeed = 30;               // Forward/backward movement speed
    public float Turnspeed = 100;              // Rotation speed
    public RewardInfo rwd = new RewardInfo();  // Reward configuration
    public bool doEpisodes = true;             // Whether to end episodes on collisions

    // Internal components and state
    private Rigidbody rb = null;
    private Vector3 posicion_original;         // Starting position
    private Quaternion rotacion_original;      // Starting rotation
    private Bounds bnd;                        // Car's bounds for raycasting
    private float distanceToNextCheckpoint;    // Distance to the next checkpoint for reward calculation
    private float lastDistanceToCheckpoint;    // Distance to the last checkpoint for reward calculation
    private Transform currentCheckPoint;
    private Transform nextCheckPoint;
    private Transform npcPosition;
    public Transform spawnPoint;
    private PlayerCar playerCar;
    private TrackCheck trackCheck;

    // Initialize the agent's components and save initial state
    public override void Initialize()
    {
        // Setup physics

  
        npcPosition = transform;
        playerCar = GetComponent<PlayerCar>();
        trackCheck = FindFirstObjectByType<TrackCheck>();
        rb = this.GetComponent<Rigidbody>();
        rb.linearDamping = 1;
        rb.angularDamping = 5;
        rb.interpolation = RigidbodyInterpolation.Extrapolate;

        // Setup collider and decision making

        this.GetComponent<DecisionRequester>().DecisionPeriod = 1;
        bnd = this.GetComponent<MeshRenderer>().bounds;

        // Save initial position and rotation
        posicion_original = this.transform.position;
        rotacion_original = this.transform.rotation;

        if (trackCheck != null)
        {
            //trackCheck.OnCorrectCheckPointAI += OnPassedCheckpoint;
            //trackCheck.OnWrongCheckPointAI += OnWrongCheckpoint;
        }
    }

    // Reset agent state at the start of each episode
    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        npcPosition.position = spawnPoint.position + Vector3.up * 0.5f;
        npcPosition.rotation = spawnPoint.rotation;
        trackCheck.ResetCheckpoint(this);
        nextCheckPoint = trackCheck.GetNextCheckpoint(this);
        lastDistanceToCheckpoint = GetDistanceToNextCheckpoint();
    }

    // Process actions from the neural network
    // Handles movement and rotation based on discrete actions
    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(rwd.timeStepPenalty);
        float mag = Mathf.Abs(rb.linearVelocity.sqrMagnitude);

        // Handle movement actions (0: no movement, 1: backward, 2: forward)
        float move = actions.ContinuousActions[0];   // -1 a 1
        float turn = actions.ContinuousActions[1];   // -1 a 1

        rb.AddForce(npcPosition.forward * move * Movespeed, ForceMode.Acceleration);
        if (rb.linearVelocity.magnitude > Movespeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * Movespeed;
        }
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn * Turnspeed * Time.fixedDeltaTime, 0f));

        //Movement rewards

        if (Mathf.Abs(move) < 0.05f)
            AddReward(rwd.no_movement);

        if (move > 0)
            AddReward(move * rwd.mult_forward);
        else if (move < 0)
            AddReward(Mathf.Abs(move) * rwd.mult_backward);

        if (nextCheckPoint != null)
        {
            float currentDistance = GetDistanceToNextCheckpoint();
            float deltaDistance = lastDistanceToCheckpoint - currentDistance;

            if (deltaDistance > 0)
            {
                // Moving toward checkpoint
                AddReward(0.02f * deltaDistance);
            }
            else if (deltaDistance < -0.5f)
            {
                // Moving away from checkpoint significantly
                AddReward(0.001f * deltaDistance);
            }

            lastDistanceToCheckpoint = currentDistance;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        nextCheckPoint = trackCheck.GetNextCheckpoint(this);
        if (nextCheckPoint == null) return;

        currentCheckPoint = trackCheck.GetNextCheckpoint(this);

        // Direction to the next checkpoint
        Vector3 dirToCheckpoint = (nextCheckPoint.position - transform.position).normalized;

        // Dot product between car's forward direction and direction to checkpoint
        // 1 = facing directly toward checkpoint, -1 = facing directly away
        float forwardDot = Vector3.Dot(transform.forward, dirToCheckpoint);
        sensor.AddObservation(forwardDot);

        // Right dot product to help with steering
        float rightDot = Vector3.Dot(transform.right, dirToCheckpoint);
        sensor.AddObservation(rightDot);

        // Distance to next checkpoint (normalized)
        float checkpointDistance = Vector3.Distance(transform.position, nextCheckPoint.position);
        sensor.AddObservation(checkpointDistance / 100f);

        // Car's velocity (normalized)
        sensor.AddObservation(rb.linearVelocity.magnitude / 20f);

        // Car's angular velocity (normalized)
        sensor.AddObservation(rb.angularVelocity.magnitude / 10f);


    }

    // Handle collisions with track barriers and other cars
    private void OnCollisionEnter(Collision collision)
    {
        float mag = collision.relativeVelocity.sqrMagnitude;

        // Check for barrier collisions
        if (collision.gameObject.layer == LayerMask.NameToLayer("muro"))
        {
            AddReward(mag * rwd.mult_barrier);  // Apply barrier collision penalty
            if (doEpisodes == true)
                EndEpisode();
        }
        // Check for car collisions
        else if (collision.gameObject.CompareTag("Enemigo") == true)
        {
            AddReward(mag * rwd.mult_car);      // Apply car collision penalty
            if (doEpisodes == true)
                EndEpisode();
        }
    }
    private float GetDistanceToNextCheckpoint()
    {
        if (nextCheckPoint != null)
        {
            return Vector3.Distance(transform.position, nextCheckPoint.position);
        }
        return 0f;
    }
    private void UpdateNextCheckpointInfo()
    {
        nextCheckPoint = trackCheck.GetNextCheckpoint(this);
        if (nextCheckPoint != null)
        {      
            distanceToNextCheckpoint = Vector3.Distance(transform.position, nextCheckPoint.position);
        }
    }

    public void OnPassedCheckpoint(NPCAgent3 agent)
    {
        // Resetear distancia para evitar recompensas negativas gigantes
        nextCheckPoint = trackCheck.GetNextCheckpoint(this);
        lastDistanceToCheckpoint = Vector3.Distance(npcPosition.position, nextCheckPoint.position);
        AddReward(5f); // recompensa fuerte por checkpoint

    }

    public void OnWrongCheckpoint(NPCAgent3 agent)
    {
        AddReward(-1f); // penalizaci�n por checkpoint incorrecto
    }

}

