using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FSMManager : StateMachineFlow {
    [Header("Estados")]
    public Idle idleState;
    public Accelerating acceleratingState;
    public Braking brakingState;
    public Falling fallingState;
    public Drifting driftingState;
    public Boosting boostingState;
    public Tricking trickingState;

    [Header("Referencias")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform visual; // Contenedor del modelo visual (asignar en Inspector)

    private Transform visualModel;
    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private CapsuleCollider hitBox;

    [Header("Configuración Base (sin multiplicadores)")]
    private float baseMaxSpeed = 80f;
    private float baseAcceleration = 40f;
    private float baseBrakePower = 25f;
    private float baseSteerPower = 10f;
    private float baseFriction = 10f;
    private float baseGravity = 100f;

    [Header("Drift Base")]
    private float baseDriftFriction = 2f;
    private float baseDriftPower = 20f;
    private float baseDriftSideForce = 10f;
    private float baseDriftAngle = 40f;
    private float baseDriftEntrySpeed = 120f;

    [Header("Boost Base")]
     private float baseBoostDurationMultiplier = 1f;

    [Header("Referencia al SO de Personaje")]
    [SerializeField] private PersonajeSO personajeSO;

    // Variables finales (base * multiplicador)
    private float maxSpeed;
    private float accelerationPower;
    private float brakePower;
    private float steerPower;
    private float frictionPower;
    private float gravityForce;

    private float driftFrictionPower;
    private float driftPower;
    private float driftSideForce;
    private float driftAngle;
    private float driftEntrySpeed;

    private float boostDurationMultiplier;

    [Header("Drift")]
    public float driftTimer;
    public sbyte driftDirection;
    public bool driftFlag = false;
    private Quaternion driftBaseRotation;
    private float currentDriftAngle;
    bool first, second, third;
    public int driftLevel;// 0, 1, 2, 3
    
    Color driftFirstColor = Color.yellow;
    Color driftSecondColor = Color.red;
    Color driftThirdColor = Color.cyan;

    [Header("Boost")]
    public BoostType currentBoostType;
    public float boostDuration;
    public float trickBoostDuration = 0.5f;
    public enum BoostType { Drift, Trick }

    [Header("Tricks")]
    public bool canTrick;

    [Header("Partículas")]
    public List<ParticleSystem> driftParticles = new List<ParticleSystem>();
    public List<ParticleSystem> turboParticles = new List<ParticleSystem>();

    // Inputs
    public bool accelerateInput;
    public bool brakeInput;
    public float horizontalInput;
    public bool driftInput;
    public bool isGrounded;
    public bool trickInput;

    private void Awake()
    {
        // Inicializar estados
        idleState = new Idle(this);
        acceleratingState = new Accelerating(this);
        brakingState = new Braking(this);
        fallingState = new Falling(this);
        driftingState = new Drifting(this);
        boostingState = new Boosting(this);
        trickingState = new Tricking(this);

        // Componentes
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<CapsuleCollider>();


        // Aplicar multiplicadores desde el SO
        ApplyMultipliersFromSO();

        // Instanciar modelo visual y partículas
        InstantiateVisualSO();
    }

    protected override void GetinitialState(out TemplateStateMachine _stateMachine)
    {
        _stateMachine = idleState;
    }

    // ==================== INICIALIZACIÓN VISUAL ====================
    private void InstantiateVisualSO()
    {
        if (personajeSO == null)
        {
            Debug.LogError("FSMManager: PersonajeSO no asignado.");
            return;
        }

        if (personajeSO.visual == null)
        {
            Debug.LogWarning("FSMManager: El PersonajeSO no tiene modelo visual.");
            return;
        }


        // Instanciar el modelo
        GameObject visualInstance = Instantiate(personajeSO.visual, visual);
        visualInstance.transform.localRotation = Quaternion.identity;
        visualModel = visualInstance.transform;

        // Limpiar listas actuales
        driftParticles.Clear();
        turboParticles.Clear();

        // Buscar partículas de drift
        Transform driftRoot = visualModel.Find(personajeSO.driftParticlesPath);
        if (driftRoot != null)
        {
            for (int i = 0; i < driftRoot.childCount; i++)
            {
                Transform side = driftRoot.GetChild(i);
                for (int j = 0; j < side.childCount; j++)
                {
                    ParticleSystem ps = side.GetChild(j).GetComponent<ParticleSystem>();
                    if (ps != null) driftParticles.Add(ps);
                }
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró driftParticles en la ruta: {personajeSO.driftParticlesPath}");
        }

        // Buscar partículas de turbo
        Transform turboRoot = visualModel.Find(personajeSO.turboParticlesPath);
        if (turboRoot != null)
        {
            for (int i = 0; i < turboRoot.childCount; i++)
            {
                Transform side = turboRoot.GetChild(i);
                for (int j = 0; j < side.childCount; j++)
                {
                    ParticleSystem ps = side.GetChild(j).GetComponent<ParticleSystem>();
                    if (ps != null) turboParticles.Add(ps);
                }
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró turboParticles en la ruta: {personajeSO.turboParticlesPath}");
        }

    }

    // ==================== MULTIPLICADORES ====================
    private void ApplyMultipliersFromSO()
    {
        if (personajeSO == null)
        {
            maxSpeed = baseMaxSpeed;
            accelerationPower = baseAcceleration;
            brakePower = baseBrakePower;
            steerPower = baseSteerPower;
            frictionPower = baseFriction;
            gravityForce = baseGravity;

            driftFrictionPower = baseDriftFriction;
            driftPower = baseDriftPower;
            driftSideForce = baseDriftSideForce;
            driftAngle = baseDriftAngle;
            driftEntrySpeed = baseDriftEntrySpeed;

            boostDurationMultiplier = baseBoostDurationMultiplier;
            return;
        }

        maxSpeed = baseMaxSpeed * personajeSO.maxSpeedMultiplier;
        accelerationPower = baseAcceleration * personajeSO.accelerationMultiplier;
        brakePower = baseBrakePower;
        steerPower = baseSteerPower * personajeSO.steeringMultiplier;

        frictionPower = baseFriction * personajeSO.weightMultiplier;
        gravityForce = baseGravity * personajeSO.weightMultiplier;

        driftPower = baseDriftPower * personajeSO.driftControlMultiplier;
        driftSideForce = baseDriftSideForce * personajeSO.driftControlMultiplier;
        driftFrictionPower = baseDriftFriction;
        driftAngle = baseDriftAngle;
        driftEntrySpeed = baseDriftEntrySpeed;

        boostDurationMultiplier = baseBoostDurationMultiplier * personajeSO.turboMultiplier;
    }

    public void SetPersonajeSO(PersonajeSO so)
    {
        personajeSO = so;
        ApplyMultipliersFromSO();
    }

    // ==================== INPUTS ====================
    public PlayerInputActions GetInputActions() => inputActions;

    // ==================== GROUND CHECK ====================
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
        float rayDistance = 2f;
        foreach (Vector3 offset in GroundRaycasts())
        {
            Vector3 origin = hitBox.transform.TransformPoint(offset);
            if (Physics.Raycast(origin, -hitBox.transform.up, rayDistance, groundLayer))
            {
                isGrounded = true;
                return true;
            }
        }
        isGrounded = false;
        return false;
    }

    public Vector3 GetAverageGroundNormals()
    {
        List<Vector3> normals = new List<Vector3>();
        float rayDistance = 5f;

        foreach (Vector3 offset in GroundRaycasts())
        {
            Vector3 origin = hitBox.transform.TransformPoint(offset);
            if (Physics.Raycast(origin, -hitBox.transform.up, out RaycastHit hit, rayDistance, groundLayer))
                normals.Add(hit.normal);
        }

        if (normals.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (Vector3 n in normals) sum += n;
        Vector3 avg = sum / normals.Count;
        return new Vector3(avg.x, 1f, avg.z).normalized;
    }

    // ==================== MOVIMIENTO ====================
    public void ApplyAcceleration(float power = -1f)
    {
        if (power < 0) power = accelerationPower;
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(hitBox.transform.forward * power, ForceMode.Acceleration);
    }

    public void ApplyBrake(float power = -1f)
    {
        if (power < 0) power = brakePower;
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(-hitBox.transform.forward * power, ForceMode.Acceleration);
    }

    public void ApplySlowDown(float slowDown = 2f)
    {
        if (rb.linearVelocity.magnitude > 0.1f)
            rb.AddForce(-rb.linearVelocity.normalized * slowDown, ForceMode.Acceleration);
    }

    public void ApplyGravity()
    {
        rb.AddForce(gravityForce * Vector3.down, ForceMode.Acceleration);
    }

    public void ApplyLateralFriction(float strength)
    {
        Vector3 forward = hitBox.transform.forward;
        Vector3 lateralVel = Vector3.ProjectOnPlane(rb.linearVelocity, forward);
        rb.AddForce(-lateralVel * strength, ForceMode.Acceleration);
    }

    public void ApplySteer()
    {
        if (Mathf.Abs(horizontalInput) > Mathf.Epsilon && rb.linearVelocity != Vector3.zero)
        {
            float direction = Vector3.Dot(rb.linearVelocity, hitBox.transform.forward) >= 0f ? 1f : -1f;
            float targetAngle = horizontalInput * steerPower * direction;
            Quaternion targetRot = Quaternion.LookRotation(
                Quaternion.Euler(0, targetAngle, 0) * hitBox.transform.forward,
                Vector3.up
            );

            hitBox.transform.rotation = Quaternion.Slerp(hitBox.transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        ApplyLateralFriction(frictionPower);
    }

    public void RotateHitbox()
    {
        Vector3 avgNormal = GetAverageGroundNormals();
        if (avgNormal == Vector3.zero) avgNormal = Vector3.up;

        Vector3 forwardProj = Vector3.ProjectOnPlane(hitBox.transform.forward, avgNormal).normalized;
        Quaternion targetRot = Quaternion.LookRotation(forwardProj, avgNormal);

        hitBox.transform.rotation = Quaternion.Slerp(hitBox.transform.rotation, targetRot, Time.deltaTime * 10f);
    }

    // ==================== DRIFT ====================
    public void StartDrift()
    {
        driftFlag = true;
        driftTimer = 0f;

        driftDirection = (sbyte)Mathf.Sign(horizontalInput);
        if (driftDirection == 0) driftDirection = 1;

        driftBaseRotation = hitBox.transform.rotation;
        currentDriftAngle = 0f;
        SetDriftParticlesColor(driftFirstColor);
    }

    public void ApplyDriftMovement()
    {
        driftTimer += Time.deltaTime;
        if (currentDriftAngle < driftAngle)
        {
            currentDriftAngle += driftEntrySpeed * Time.deltaTime;
            if (currentDriftAngle > driftAngle) currentDriftAngle = driftAngle;
        }

        float adjustAngle = horizontalInput * driftPower;
        float spin = driftDirection * driftPower * driftTimer * 2;
        Quaternion targetRot = driftBaseRotation
            * Quaternion.Euler(0, currentDriftAngle * driftDirection, 0)
            * Quaternion.Euler(0, adjustAngle, 0)
            * Quaternion.Euler(0, spin, 0);

        hitBox.transform.rotation = Quaternion.Slerp(hitBox.transform.rotation, targetRot, Time.deltaTime * 8f);

        Vector3 accelForce = Vector3.zero;
        if (accelerateInput)
            accelForce = hitBox.transform.forward * accelerationPower * 0.8f;

        rb.AddForce(accelForce + (hitBox.transform.right * -driftDirection * driftSideForce), ForceMode.Acceleration);
        ApplyLateralFriction(driftFrictionPower);
    }

    public void EndDrift()
    {
        driftFlag = false;
        first = second = third = false;
        StopDriftParticles();
        ClearDriftParticles();
    }

    public bool CanBoost() => isGrounded && first;

    public void RotateHitboxDrift()
    {
        Vector3 avgNormal = GetAverageGroundNormals();
        if (avgNormal == Vector3.zero) avgNormal = Vector3.up;

        Vector3 forward = hitBox.transform.forward;
        Vector3 projectedForward = Vector3.ProjectOnPlane(forward, avgNormal).normalized;
        Quaternion groundRotation = Quaternion.LookRotation(projectedForward, avgNormal);

        hitBox.transform.rotation = Quaternion.Slerp(hitBox.transform.rotation, groundRotation, Time.deltaTime * 10f);
    }

    public void UpdateDriftLevel()
    {
        bool colorChanged = false;
        Color newColor = Color.clear;

        if (!first && driftTimer > 0.5f)
        {
            driftLevel = 1;
            newColor = driftFirstColor;
            first = true;
            colorChanged = true;
        }
        else if (first && !second && driftTimer > 3f)
        {
            driftLevel = 2;
            newColor = driftSecondColor;
            second = true;
            colorChanged = true;
        }
        else if (first && second && !third && driftTimer > 5f)
        {
            driftLevel = 3;
            newColor = driftThirdColor;
            third = true;
            colorChanged = true;
        }

        if (colorChanged)
        {
            SetDriftParticlesColor(newColor);
            PlayDriftParticles();
        }
    }

    // ==================== BOOST ====================
    public float GetBoostDurationAfterDrift()
    {
        float baseDuration = 0f;

        switch (driftLevel)
        {
            case 3: baseDuration = 1f; break;
            case 2: baseDuration = 0.75f; break;
            case 1: baseDuration = 0.5f; break;
        }

        return baseDuration * boostDurationMultiplier;
    }

    public void ApplyBoostSpeed()
    {
        Vector3 v = rb.linearVelocity;
        Vector3 forward = hitBox.transform.forward * maxSpeed;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity,new Vector3(forward.x, v.y, forward.z), Time.deltaTime * 10f);
    }

    // ==================== PARTÍCULAS ====================
    public void PlayDriftParticles()
    {
        foreach (var p in driftParticles) p.Play();
    }

    public void StopDriftParticles()
    {
        foreach (var p in driftParticles) p.Stop();
    }

    public void ClearDriftParticles()
    {
        foreach (var p in driftParticles) p.Clear();
    }

    public void SetDriftParticlesColor(Color color)
    {
        foreach (var p in driftParticles)
        {
            var main = p.main;
            main.startColor = color;
        }
    }

    public void PlayTurboParticles()
    {
        foreach (var p in turboParticles) p.Play();
    }

    public void StopTurboParticles()
    {
        foreach (var p in turboParticles) p.Stop();
    }

    // ==================== UTILIDADES ====================
    public Transform GetHitboxTransform()
    {
        return hitBox.transform;
    }
    public float GetMaxSpeed() => maxSpeed;
    private void OnDrawGizmos()
    {
        if (hitBox == null) return;

        Gizmos.color = Color.yellow;
        float rayDistance = 5f;
        foreach (Vector3 offset in GroundRaycasts())
        {
            Vector3 origin = hitBox.transform.TransformPoint(offset);
            Gizmos.DrawSphere(origin, 0.1f);

            if (Physics.Raycast(origin, -hitBox.transform.up, out RaycastHit hit, rayDistance, groundLayer))
            {
                Gizmos.DrawLine(origin, hit.point);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.point, 0.1f);
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.DrawLine(origin, origin - hitBox.transform.up * rayDistance);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(origin - hitBox.transform.up * rayDistance, 0.1f);
                Gizmos.color = Color.yellow;
            }
        }
    }
}