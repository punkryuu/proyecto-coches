using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class FSMManager : StateMachineFlow {
    [Header("Estados")]
    public Idle idleState;
    public Accelerating acceleratingState;
    public Braking brakingState;
    public Falling fallingState;
    public Drifting driftingState;

    [Header("Referencias")]
    [SerializeField] private Transform cameraFollow;
    [SerializeField] private LayerMask groundLayer;
    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private CapsuleCollider hitBox;

    [Header("Movimiento")]
    private float maxSpeed = 50f;
     private float accelerationPower = 20f;
    private float brakePower = 25f;
    private float steerPower = 10f;
    private float frictionPower = 10f;
    private float gravityForce = 100f;

    [Header("Drift")]
    public float driftFrictionPower = 2f;
    public float driftPower = 20f;
    public float driftTimer;
    public float driftSideForce = 10f;
    public float driftAngle = 40f;
    public float driftEntrySpeed = 120f;   
    private float currentDriftAngle;
    public sbyte driftDirection; // 0 = ninguno, 1 = derecha, 2 = izquierda
    public bool driftFlag = false;
    private Quaternion driftBaseRotation;
    bool first, second, third;

    [Header("particles")]
    public List<ParticleSystem> driftParticles;
    public List<ParticleSystem> turboParticles;

    // Inputs
    public bool accelerateInput;
    public bool brakeInput;
    public float horizontalInput;
    public bool driftInput;
    public bool isGrounded;

    private void Awake()
    {
        // Inicializar estados
        idleState = new Idle(this);
        acceleratingState = new Accelerating(this);
        brakingState = new Braking(this);
        fallingState = new Falling(this);
        driftingState = new Drifting(this);

        // Componentes
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<CapsuleCollider>();

        // Si no se asigna cámara, buscar una por defecto
        if (cameraFollow == null && Camera.main != null)
            cameraFollow = Camera.main.transform;
        InstanceParticles();
    }

    protected override void GetinitialState(out TemplateStateMachine _stateMachine)
    {
        _stateMachine = idleState;
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
        // Forzar componente Y a 1 para evitar inclinaciones extremas
        return new Vector3(avg.x, 1f, avg.z).normalized;
    }

    // ==================== MOVIMIENTO BÁSICO ====================
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
            //1 = adelante, -1 = atrás
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
        if (driftFlag)
        {
            RotateHitboxDrift();
            return;
        }

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
        PlayDriftParticles();
    }




    public void ApplyDriftMovement()
    {
        if (currentDriftAngle < driftAngle)
        {
            currentDriftAngle += driftEntrySpeed * Time.deltaTime;
            if (currentDriftAngle > driftAngle) currentDriftAngle = driftAngle;
        }
        float adjustAngle = horizontalInput * driftPower;
        Quaternion adjustRot = Quaternion.Euler(0, adjustAngle, 0);
        float spin = driftDirection * driftPower * driftTimer *2;
        Quaternion spinRot = Quaternion.Euler(0, spin, 0);

        Quaternion targetRot = driftBaseRotation
            * Quaternion.Euler(0, currentDriftAngle * driftDirection, 0)
            * adjustRot
            * spinRot;
        hitBox.transform.transform.rotation = Quaternion.Slerp(
            hitBox.transform.transform.rotation,
            targetRot,
            Time.deltaTime * 8f
        );

        //movimiento
        Vector3 accelForce = Vector3.zero;
        if (accelerateInput)
        {
            accelForce = hitBox.transform.forward * accelerationPower * 0.8f;
        }
        rb.AddForce(accelForce + (hitBox.transform.right * -driftDirection * driftSideForce), ForceMode.Acceleration);
        ApplyLateralFriction(driftFrictionPower);
        driftTimer += Time.deltaTime;
    }




    public void EndDrift()
    {
        driftFlag = false;
        StopDriftParticles();
    }
    public void RotateHitboxDrift()
    {
        Vector3 avgNormal = GetAverageGroundNormals();
        if (avgNormal == Vector3.zero) avgNormal = Vector3.up;

        Vector3 forward = hitBox.transform.forward;

        Vector3 projectedForward = Vector3.ProjectOnPlane(forward, avgNormal).normalized;

        Quaternion groundRotation = Quaternion.LookRotation(projectedForward, avgNormal);

        hitBox.transform.rotation = Quaternion.Slerp(
            hitBox.transform.rotation,
            groundRotation,
            Time.deltaTime * 10f
        );
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
    public void SetDriftParticles(List<ParticleSystem> particles) => driftParticles = particles;
    public void SetTurboParticles(List<ParticleSystem> particles) => turboParticles = particles;
    public void InstanceParticles()
    {
        // Buscar partículas
        Transform driftParticles = hitBox.transform.Find("Visual/RanaCoche/driftParticles");
        Transform turboParticles = hitBox.transform.Find("Visual/RanaCoche/turboParticles");

        List<ParticleSystem> driftList = new List<ParticleSystem>();
        List<ParticleSystem> turboList = new List<ParticleSystem>();

        if (driftParticles != null)
        {
            for (int i = 0; i < driftParticles.GetChild(0).childCount; i++)
                driftList.Add(driftParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
            for (int i = 0; i < driftParticles.GetChild(1).childCount; i++)
                driftList.Add(driftParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }

        if (turboParticles != null)
        {
            for (int i = 0; i < turboParticles.GetChild(0).childCount; i++)
                turboList.Add(turboParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
            for (int i = 0; i < turboParticles.GetChild(1).childCount; i++)
                turboList.Add(turboParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }

        SetDriftParticles(driftList);
        SetTurboParticles(turboList);
    }
    public void UpdateDriftLevel()
    {
        bool colorChanged = false;
        Color newColor = Color.clear;

        if (!first && driftTimer > 2)
        {
            newColor = Color.yellow;
            first = true;
            colorChanged = true;
        }
        else if (first && !second && driftTimer > 4)
        {
            newColor = Color.red;
            second = true;
            colorChanged = true;
        }
        else if (first && second && !third && driftTimer > 10)
        {
            newColor = Color.cyan;
            third = true;
            colorChanged = true;
        }

        if (colorChanged)
        {
            SetDriftParticlesColor(newColor);
            PlayDriftParticles();
        }
    }


    // ==================== ????? ====================
    private float Remap(float val, float from1, float to1, float from2, float to2)
    {
        return (val - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

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