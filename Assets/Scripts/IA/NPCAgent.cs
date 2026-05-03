using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class NPCAgent:Agent
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform npcPosition;
    //[SerializeField] Waypoints waypoints;
    [SerializeField] TrackCheck trackCheck;
    [SerializeField] LayerMask raycastMask;

    float maxSpeed = 20f;
    float accelerationForce = 20f;
    float brakeForce = 20f;

    public float rayLength = 15f; //Distancia máxima de los rayos
    public Vector3[] rayDirections;

    public float stuckTimeLimit = 4f;
    public float timeSinceLastProgress = 0f;
    public float turnSpeed = 100f;
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

        npcPosition.position = trackCheck.GetStartPosition();
        npcPosition.rotation = trackCheck.GetStartRotation();

        trackCheck.ResetCheckpoint(this);
        timeSinceLastProgress = 0f;
        lastPosition = npcPosition.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agregar la dirección al próximo checkpoint
        currentCheckPoint = trackCheck.GetNextCheckpoint(this);
        Vector3 directionToNextCheckpoint = (currentCheckPoint.position - npcPosition.position).normalized;

        // Agregar la distancia al próximo checkpoint
        float distanceToNextCheckpoint = Vector3.Distance(npcPosition.position, currentCheckPoint.position);

        float speed = rb.linearVelocity.magnitude;
        float angularSpeed = rb.angularVelocity.magnitude;

        float angelToCheckPoint = Vector3.SignedAngle(npcPosition.forward, directionToNextCheckpoint, Vector3.up) / 180f; // Normalizado entre -1 y 1

        bool grounded = Physics.Raycast(npcPosition.position + Vector3.up, Vector3.down, 0.5f);

        foreach (var direction in rayDirections)
        {
            Vector3 worldDirecction = npcPosition.TransformDirection(direction.normalized);
            if (Physics.Raycast(npcPosition.position, worldDirecction, out RaycastHit hit, rayLength, raycastMask))
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
        int movementAction = actions.DiscreteActions[0]; // 0: adelante, 1: atrás
        int turnAction = actions.DiscreteActions[1]; // 0: sin acción, 1: izquierda, 2: derecha

        Vector3 movement = Vector3.zero;

        if(movementAction == 1)
            movement += npcPosition.forward * accelerationForce;
        else if(movementAction == 2)
            movement -= npcPosition.forward * brakeForce;

        if(rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(movement, ForceMode.Acceleration);
       
        float turn = 0f;
        if (turnAction == 1) turn = -1f;
        else if (turnAction == 2) turn = 1f;

        npcPosition.Rotate(Vector3.up, turn * turnSpeed * Time.fixedDeltaTime);

        Vector3 directionToNextCheckpoint = (currentCheckPoint.position - npcPosition.position).normalized;
        float aligment = Vector3.Dot(npcPosition.forward, directionToNextCheckpoint);
        AddReward(aligment * 0.01f); // Recompensa por alineación con el próximo checkpoint

        if(rb.linearVelocity.magnitude < 1f)
        AddReward(-0.005f); // Penalización por estar casi detenido

        timeSinceLastProgress += Time.fixedDeltaTime;
        if(Vector3.Distance(npcPosition.position, lastPosition) > 2f)
        {
            timeSinceLastProgress = 0f;
            lastPosition = npcPosition.position;
        }
        if(timeSinceLastProgress > stuckTimeLimit)
        {
            AddReward(-1f); // Penalización por estar atascado
            EndEpisode();
        }
    }
}
