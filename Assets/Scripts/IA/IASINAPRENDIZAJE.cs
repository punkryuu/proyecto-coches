using UnityEngine;

public class IASINAPRENDIZAJE : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public CapsuleCollider hitBox;
    public SphereCollider sphereCollider;
    public TrackCheck trackCheck;
    public PlayerCar IaPlayerCar;


    [Header("Driving")]
    public float maxSpeed = 800f;
    public float turnSpeed = 80f;
    public float accelerationForce =50f;
    public float brakeForce = 20f;
    public float driftPower = 20f;
    public float driftSideForce = 15f;

    [Header("AI")]
    public float checkpointLookAhead = 10f;
    public float driftAngleThreshold = 20f;
    public float steeringSmooth = 5f;

    private Transform currentCheckpoint;
    private Vector3 targetDirection;
    private float steerInput;
    private bool accelerateInput;
    private bool brakeInput;
    private bool driftInput;
    public Transform spawnPoint;
    

    public float gravityMultiplier = 1f;
    public float gravityForce = 20f;
    public float rayDistance = 2f;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<CapsuleCollider>();
        sphereCollider = GetComponentInChildren<SphereCollider>();
        IaPlayerCar = GetComponent<PlayerCar>();
        trackCheck = FindFirstObjectByType<TrackCheck>();


        currentCheckpoint = trackCheck.GetNextCheckpoint(this);
        trackCheck.OnCorrectCheckPointRacer += OnPassedCheckpoint;
       // trackCheck.OnWrongCheckPointRacer += OnWrongCheckpoint;
    }

    void FixedUpdate()
    {
        bool grounded = CheckGrounded();

        if (!grounded)
            ApplyGravity();
        UpdateAI();
        Drive();


        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // ============================================================
    // ======================= IA PRINCIPAL ========================
    // ============================================================

    void UpdateAI()
    {
        // Obtener checkpoint actual
        currentCheckpoint = trackCheck.GetNextCheckpoint(this);

        Vector3 toCheckpoint = currentCheckpoint.position - transform.position;
        targetDirection = toCheckpoint.normalized;

        // Convertir dirección a espacio local
        Vector3 localDir = transform.InverseTransformDirection(targetDirection);

        // Dirección de giro
        steerInput = Mathf.Lerp(steerInput, localDir.x, Time.fixedDeltaTime * steeringSmooth);

        // Acelerar si el checkpoint está adelante
        accelerateInput = localDir.z > 0.2f;

        // Frenar si el checkpoint está detrás
        brakeInput = localDir.z < 0f;

        // Activar drift si el ángulo es grande
        float angle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
        driftInput = Mathf.Abs(angle) > driftAngleThreshold;
    }

    // ============================================================
    // ======================= MOVIMIENTO ==========================
    // ============================================================

    void Drive()
    {
        // Giro
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, steerInput * turnSpeed * Time.fixedDeltaTime, 0f));

        // Aceleración
        if (accelerateInput)
        ApplyAccelerationNPC();

        // Frenado
        if (brakeInput)
            ApplyBrakeNPC();

        // Drift
        if (driftInput)
            ApplyDrift(steerInput);
    }


    private void ApplyAccelerationNPC()
    {
        rb.AddForce(transform.forward * accelerationForce, ForceMode.Acceleration);
    }

    private void ApplyBrakeNPC()
    {
        rb.AddForce(-transform.forward * brakeForce, ForceMode.Acceleration);
    }

    private void ApplyDrift(float steer)
    {
        float driftRotation = steer * driftPower;

        hitBox.transform.localRotation = Quaternion.Slerp(
            hitBox.transform.localRotation,
            Quaternion.Euler(0, driftRotation, 0),
            Time.deltaTime * 1.5f
        );

        rb.AddForce(transform.right * steer * driftSideForce, ForceMode.Acceleration);
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
                return true;
        }
        return false;
    }

    public void ApplyGravity()
    {
        if (!CheckGrounded())
        {
            rb.AddForce(Vector3.down * gravityForce * gravityMultiplier, ForceMode.Acceleration);
        }
    }
    public void OnPassedCheckpoint(IASINAPRENDIZAJE npc)
    {
        IaPlayerCar.currentWayPoint++;
    }

    public void InstantiateVisualSO(GameObject npcInstance, PersonajeSO personajeSO)
    {
        if (personajeSO == null)
        {
            Debug.LogError("NPC: PersonajeSO no asignado.");
            return;
        }

        if (personajeSO.visual == null)
        {
            Debug.LogWarning("NPC: El PersonajeSO no tiene modelo visual.");
            return;
        }

        // Crear punto de anclaje si no existe
        Transform visualRoot = npcInstance.transform.Find("VisualRoot");

        if (visualRoot == null)
        {
            GameObject vr = new GameObject("VisualRoot");
            vr.transform.SetParent(npcInstance.transform);
            vr.transform.localPosition = Vector3.zero;
            vr.transform.localRotation = Quaternion.identity;
            visualRoot = vr.transform;
        }

        // Instanciar modelo visual dentro del NPC
        GameObject visualInstance = Instantiate(personajeSO.visual, visualRoot);
        visualInstance.transform.localPosition = Vector3.zero;
        visualInstance.transform.localRotation = Quaternion.identity;

        Transform visualModel = visualInstance.transform;
    }
}


