using System.Collections;
using UnityEngine;

public class IASINAPRENDIZAJE : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public CapsuleCollider hitBox;
    public SphereCollider sphereCollider;
    public TrackCheck trackCheck;
    public PlayerCar IaPlayerCar;
    public WayPointsCircuit circuit;


    [Header("Driving")]
    public float baseMaxSpeed = 800f;
    public float baseTurnSpeed = 80f;
    public float baseAcceleration = 50f;
    public float baseBrakeForce = 20f;
    public float baseDriftPower = 20f;
    public float baseDriftSideForce = 15f;

    public float maxSpeed;
    public float turnSpeed;
    public float accelerationForce;
    public float brakeForce;
    public float driftPower;
    public float driftSideForce;

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
    
    public float powerCost = 3f;
    public float gravityMultiplier = 1f;
    public float gravityForce = 20f;
    public float rayDistance = 2f;
    public LayerMask groundLayer;
    private Vector3 lastPosition;
    private float stuckTimer; 
    public GameObject minimapPlane;


    [Header("Randomization")]
    [Range(0f, 1f)] public float aggressiveness;
    [Range(0f, 1f)] public float mistakeChance;
    [Range(0f, 1f)] public float steeringNoise;

    [Header("Stun System")]
    public Transform visualModel;
    public Transform visualRoot;
    private Quaternion originalVisualRotation;
    public float stunSpinSpeed = 360f;
    public GameObject stunStars;
    private Coroutine restoreCoroutine;
    private bool isStunned = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<CapsuleCollider>();
        sphereCollider = GetComponentInChildren<SphereCollider>();
        IaPlayerCar = GetComponent<PlayerCar>();
        trackCheck = FindFirstObjectByType<TrackCheck>();
        circuit = FindFirstObjectByType<WayPointsCircuit>();

        currentCheckpoint = trackCheck.GetNextCheckpoint(this);
        trackCheck.OnCorrectCheckPointRacer += OnPassedCheckpoint;
        ApplyPersonajeSO();

        aggressiveness = Random.Range(0.4f, 1f);
        mistakeChance = Random.Range(0.0f, 0.3f);
        steeringNoise = Random.Range(0.0f, 0.15f);
        SetMinimapImage();
    }

    void FixedUpdate()
    {
        if (!RaceManager.Instance.raceStarted)
            return;
        bool grounded = CheckGrounded();

        if (!grounded)
            ApplyGravity();
        UpdateAI();
        Drive();


        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        float moved = Vector3.Distance(transform.position, lastPosition);

        float speed = rb.linearVelocity.magnitude;

        if (speed < 5f)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer > 1.5f)
        {
            FixStuck();
            return;
        }

    }


    void UpdateAI()
    {
        // Obtener checkpoint actual
        currentCheckpoint = trackCheck.GetNextCheckpoint(this);

        Vector3 toCheckpoint = currentCheckpoint.position - transform.position;
        targetDirection = toCheckpoint.normalized;

        // Convertir dirección a espacio local
        Vector3 localDir = transform.InverseTransformDirection(targetDirection);

        // Dirección de giro
        float noise = Random.Range(-steeringNoise, steeringNoise);
        float angle = Vector3.SignedAngle(transform.forward, targetDirection + new Vector3(noise, 0, 0), Vector3.up);
        steerInput = Mathf.Clamp(angle / 45f, -1f, 1f);

        // Acelerar si el checkpoint está adelante
        accelerateInput = localDir.z > (0.2f - aggressiveness * 0.1f);

        // Frenar si el checkpoint está detrás
        brakeInput = localDir.z < 0f;

        // Activar drift si el ángulo es grande
        float bigangle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
        driftInput = Mathf.Abs(bigangle) > driftAngleThreshold && Random.value < (0.5f + aggressiveness * 0.5f); ;

        if (Random.value < mistakeChance * Time.fixedDeltaTime)
        {
            accelerateInput = false;
            brakeInput = true;
        }
    }

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

    void ApplyPersonajeSO()
{
    if (IaPlayerCar.personajeData == null) return;

    maxSpeed = baseMaxSpeed * IaPlayerCar.personajeData.maxSpeedMultiplier;
    accelerationForce = baseAcceleration * IaPlayerCar.personajeData.accelerationMultiplier;
    turnSpeed = baseTurnSpeed * IaPlayerCar.personajeData.steeringMultiplier;
    brakeForce = baseBrakeForce * IaPlayerCar.personajeData.weightMultiplier;
    driftPower = baseDriftPower * IaPlayerCar.personajeData.driftControlMultiplier;
    driftSideForce = baseDriftSideForce * IaPlayerCar.personajeData.driftControlMultiplier;
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
         visualRoot = npcInstance.transform.Find("VisualRoot");

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
        GetComponentInParent<FSMManager>().visualModel = visualModel;
    }

    public void Power()
    {
        if (IaPlayerCar.personajeData != null)
        {
            if(IaPlayerCar.powerCounter >= powerCost)
            {
                IaPlayerCar.personajeData.UsePower(this);
                IaPlayerCar.powerCounter = 0;
            }
        }
    }

    public void Respawn()
    {
        Vector3 respawnPos = CheckPoints.GetActiveCheckPointPosition();

        Debug.Log("Respawning at: " + respawnPos);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = respawnPos;
    }

    void SetMinimapImage()
    {
        if (IaPlayerCar.personajeData == null)
        {
           Debug.Log("NPC: PersonajeSO no asignado, no se puede establecer la imagen del minimapa.");
            return;
        }
        minimapPlane.GetComponent<MeshRenderer>().material.mainTexture = IaPlayerCar.personajeData.imagenMinimapa.texture;
    }

    public void StartStun()
    {
        if (visualModel == null) return;

        isStunned = true;
        originalVisualRotation = visualModel.localRotation;

        if (stunStars != null)
            stunStars.SetActive(true);
    }

    public void StayStunned()
    {
        if (!isStunned || visualModel == null) return;

        visualModel.Rotate(0f, stunSpinSpeed * Time.deltaTime, 0f, Space.Self);
    }

    public void StartRestoreRotation()
    {
        if (restoreCoroutine != null)
        {
            StopCoroutine(restoreCoroutine);
        }

        restoreCoroutine = StartCoroutine(RestoreRotationCoroutine());
    }
    public IEnumerator RestoreRotationCoroutine()
    {
        if (visualModel == null) yield break;

        isStunned = false;

        Quaternion startRot = visualModel.localRotation;
        Quaternion targetRot = originalVisualRotation;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.deltaTime;
            visualModel.localRotation = Quaternion.Slerp(startRot, targetRot, time / duration);
            yield return null;
        }

        visualModel.localRotation = targetRot;

        if (stunStars != null)
            stunStars.SetActive(false);
    }

    void FixStuck()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Transform cp = circuit.waypoints[IaPlayerCar.currentWayPoint].transform;
        Vector3 backPos = cp.position - cp.forward * 3f;
        backPos.y += 0.5f;
        transform.position = backPos;
        transform.rotation = Quaternion.LookRotation(cp.forward, Vector3.up);
        rb.AddForce(transform.forward * accelerationForce * 2f, ForceMode.VelocityChange);

        stuckTimer = 0f;
    }


}


