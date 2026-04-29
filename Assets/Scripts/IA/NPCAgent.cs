using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NPCAgent:Agent
{
    [SerializeField] Rigidbody rb;
    //[SerializeField] Transform npcPosition;
    //[SerializeField] Waypoints waypoints;
    [SerializeField] private TrackCheck trackCheck;
    public Transform startPosition;
    [SerializeField] LayerMask raycastMask;

    float maxSpeed = 20f;
    float accelerationForce = 20f;
    float brakeForce = 20f;

    public float rayLength = 15f; //Distancia máxima de los rayos
    public Vector3[] rayDirections;

    public float stuckTimeLimit = 4f;
    public float timeSinceLastProgress = 0f;
    public float turnSpeed = 100f;
    bool initialized;
    private Vector3 lastPosition;

    private Transform currentCheckPoint;

    public override void Initialize()
    {
        if(rb == null) rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        if (trackCheck == null) trackCheck = FindObjectOfType<TrackCheck>();
        lastPosition = transform.position;
        initialized = false;

    }

    public void SetTrackCheckpoints(TrackCheck tc)
    {
        trackCheck = tc;
        initialized = true;
    }

    public void SetStartPosition(Transform t)
    {
        startPosition = t;
    }

    public override void OnEpisodeBegin()
    {

        if (!initialized)
        {
            Debug.LogError("Start position not set for NPCAgent.");
            return;
        }
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;

        trackCheck.ResetCheckpoint(this);
        timeSinceLastProgress = 0f;
        lastPosition = transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (!initialized || this == null || transform == null)
            return;

        if (currentCheckPoint == null)
            return;
        // Agregar la dirección al próximo checkpoint
        currentCheckPoint = trackCheck.GetNextCheckpoint(this);
        Vector3 directionToNextCheckpoint = (currentCheckPoint.position - transform.position).normalized;

        // Agregar la distancia al próximo checkpoint
        float distanceToNextCheckpoint = Vector3.Distance(transform.position, currentCheckPoint.position);

        float speed = rb.linearVelocity.magnitude;
        float angularSpeed = rb.angularVelocity.magnitude;

        float angelToCheckPoint = Vector3.SignedAngle(transform.forward, directionToNextCheckpoint, Vector3.up) / 180f; // Normalizado entre -1 y 1

        bool grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 0.5f);

        foreach (var direction in rayDirections)
        {
            Vector3 worldDirecction = transform.TransformDirection(direction.normalized);
            if (Physics.Raycast(transform.position, worldDirecction, out RaycastHit hit, rayLength, raycastMask))
            {
                sensor.AddObservation(hit.distance / rayLength); // Normalizar la distancia
            }
            else
            {
                sensor.AddObservation(1f); // No hay obstáculo, distancia máxima
            }

            sensor.AddObservation(directionToNextCheckpoint); // Agregar la dirección del rayo
            sensor.AddObservation(distanceToNextCheckpoint/100f);
            sensor.AddObservation(speed/maxSpeed);
            sensor.AddObservation(angularSpeed / maxSpeed);
            sensor.AddObservation(angelToCheckPoint);
            sensor.AddObservation(grounded ? 1f : 0f); // Agregar si el agente está en el suelo
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!initialized || this == null || transform == null)
            return;
        int movementAction = actions.DiscreteActions[0]; // 0: adelante, 1: atrás
        int turnAction = actions.DiscreteActions[1]; // 0: sin acción, 1: izquierda, 2: derecha

        Vector3 movement = Vector3.zero;

        if(movementAction == 1)
            movement += transform.forward * accelerationForce;
        else if(movementAction == 2)
            movement -= transform.forward * brakeForce;

        if(rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(movement, ForceMode.Acceleration);
       
        float turn = 0f;
        if (turnAction == 1) turn = -1f;
        else if (turnAction == 2) turn = 1f;

        transform.Rotate(Vector3.up, turn * turnSpeed * Time.fixedDeltaTime);

        Vector3 directionToNextCheckpoint = (currentCheckPoint.position - transform.position).normalized;
        float aligment = Vector3.Dot(transform.forward, directionToNextCheckpoint);
        AddReward(aligment * 0.01f); // Recompensa por alineación con el próximo checkpoint

        if(rb.linearVelocity.magnitude < 1f)
        AddReward(-0.005f); // Penalización por estar casi detenido

        timeSinceLastProgress += Time.fixedDeltaTime;
        if(Vector3.Distance(transform.position, lastPosition) > 2f)
        {
            timeSinceLastProgress = 0f;
            lastPosition = transform.position;
        }
        if(timeSinceLastProgress > stuckTimeLimit)
        {
            AddReward(-1f); // Penalización por estar atascado
            EndEpisode();
        }
    }
}
