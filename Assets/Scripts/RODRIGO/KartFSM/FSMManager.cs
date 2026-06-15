using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class FSMManager : StateMachineFlow {
    [Header("Estados")]
    public Idle idleState;
    public Accelerating acceleratingState;
    public Braking brakingState;
    public Falling fallingState;
    public Drifting driftingState;
    public Boosting boostingState;
    public Stunned stunnedState;

    [Header("Referencias")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform visual; // Contenedor del modelo visual (asignar en Inspector)
    [SerializeField] private Transform stunStars;
    [SerializeField] private GameObject minimapPlane;

    private AudioSource audioSource; // Fuente de audio para efectos y música
    private Transform visualModel;
    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private CapsuleCollider hitBox;
    private UIManager uiManager;

    [Header("Configuración Base (sin multiplicadores)")]
    private float baseMaxSpeed = 80f;
    private float baseAcceleration = 40f;
    private float baseBrakePower = 25f;
    private float baseSteerPower = 10f;
    private float baseFriction = 10f;
    private float baseGravity = 100f;

    [Header("Drift Base")]
    private float baseDriftFriction = 8f;
    private float baseDriftPower = 20f;
    private float baseDriftSideForce = 3f;
    private float baseDriftAngle = 40f;
    private float baseDriftEntrySpeed = 120f;

    [Header("Boost Base")]
    private float baseBoostDurationMultiplier = 1f;

    [Header("Referencia al SO de Personaje")]
    [SerializeField] public PlayerCar playerCar;
    PersonajeSO personajeSO;

    // Variables finales (base * multiplicador)
    private float maxSpeed;
    private float accelerationPower;
    private float brakePower;
    private float steerPower;
    private float frictionPower;
    private float gravityForce;
    private float airControlPower;
    private float driftFrictionPower;
    private float driftPower;
    private float driftSideForce;
    private float driftAngle;
    private float driftEntrySpeed;

    private float boostDurationMultiplier;

    [Header("Drift")]
    private float driftTimer;
    private sbyte driftDirection;
    public bool driftFlag = false;
    private Quaternion driftBaseRotation;
    private float currentDriftAngle;
    bool first, second, third;
    private int driftLevel;// 0, 1, 2, 3

    [Header("Control Aéreo")]
    // de 0 a 1 , cuánto control tiene el jugador en el aire (0 = sin control, 1 = control total)
    [SerializeField] private float airControlMultiplier = 0.5f;

    [Header("Boost")]
    public BoostType currentBoostType;
    public float boostDuration;
    public float trickBoostDuration = 0.5f;
    public enum BoostType { Drift, Trick, Trigger }

    public bool triggerBoost;
    public float triggerBoostDuration;
    private bool isBoosting;
    public float gravityMultiplier = 1f;

    [Header("Tricks")]
    public bool canTrick;
    public bool isTricking;

    [Header("Stun")]
    public bool stunned;
    public float stunDuration = 1f;
    public float triggerStunDuration;
    private float stunSpinSpeed = 720f;
    public bool canBeStunned = true;
    public bool isCurrentlyStunned;
    private Quaternion originalVisualRotation;
    private bool restoringRotation;
    private Coroutine restoreCoroutine;

    [Header("Partículas")]
    private List<ParticleSystem> driftParticles = new List<ParticleSystem>();
    private List<ParticleSystem> turboParticles = new List<ParticleSystem>();
    [SerializeField]private Color drift1Start, drift1End;
    [SerializeField] private Color drift2Start, drift2End;
    [SerializeField] private Color drift3Start, drift3End;

    [Header("Ruedas (rotación X)")]
    public List<Transform> wheels = new List<Transform>();

    [Header("Pivotes (rotación Y)")]
    public List<Transform> pivots = new List<Transform>();
    // Inputs
    public bool accelerateInput;
    public bool brakeInput;

    public float horizontalInput;
    public bool driftInput;
    private float rayDistance = 2f;
    public bool trickInput;
    public bool powerInput;

    private void Awake()
    {
        // Inicializar estados
        idleState = new Idle(this);
        acceleratingState = new Accelerating(this);
        brakingState = new Braking(this);
        fallingState = new Falling(this);
        driftingState = new Drifting(this);
        boostingState = new Boosting(this);
        stunnedState = new Stunned(this);

        // Componentes
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
        SetStars(false);
        personajeSO = playerCar.personajeData;
        // Aplicar multiplicadores desde el SO
        ApplyMultipliersFromSO();

        // Instanciar modelo visual y partículas
        InstantiateVisualSO();
        uiManager = FindAnyObjectByType<UIManager>();
        SetMinimapImage();

    }



    protected override void GetinitialState(out TemplateStateMachine _stateMachine)
    {
        _stateMachine = idleState;
    }
    //
    public void SetCharacterSO() 
    {
        if (GameManager.Instance != null)
        {
            PersonajeSO selectedCharacter = GameManager.Instance.selectedCharacter;
            if (selectedCharacter != null) { personajeSO = selectedCharacter; }
        }
        else
        {
            Debug.LogError("GAMEMANAGER INSTANCE IS NULL. Make sure a GameManager object exists in the scene.");
        }

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

        wheels.Clear();
        pivots.Clear();

        foreach (Transform t in visualModel.GetComponentsInChildren<Transform>())
        {
            if (t.name.Contains("wheel") && !t.name.Contains("pivot"))
            {
                wheels.Add(t);
            }

            if (t.name.Contains("pivot"))
            {
                pivots.Add(t);
            }
        }
    }
    void SetMinimapImage() 
    {
        if (personajeSO == null)
        {
            Debug.LogError("FSMManager: PersonajeSO no asignado.");
            return;
        }
        minimapPlane.GetComponent<MeshRenderer>().material.mainTexture = personajeSO.imagenMinimapa.texture;
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
            airControlPower = airControlMultiplier;
            boostDurationMultiplier = baseBoostDurationMultiplier;
            return;
        }

        maxSpeed = baseMaxSpeed * personajeSO.maxSpeedMultiplier;
        accelerationPower = baseAcceleration * personajeSO.accelerationMultiplier;
        brakePower = baseBrakePower;
        steerPower = baseSteerPower * personajeSO.steeringMultiplier;

        frictionPower = baseFriction * personajeSO.weightMultiplier;
        gravityForce = baseGravity * personajeSO.weightMultiplier;
        airControlPower = personajeSO.airControlMultiplier;

        driftPower = baseDriftPower * personajeSO.driftControlMultiplier;
        driftSideForce = baseDriftSideForce * personajeSO.driftControlMultiplier;
        driftFrictionPower = baseDriftFriction;
        driftAngle = baseDriftAngle;
        driftEntrySpeed = baseDriftEntrySpeed;

        boostDurationMultiplier = baseBoostDurationMultiplier * personajeSO.turboMultiplier;

    if (hitBox != null )
    {
         if (hitBox.radius != personajeSO.HitBoxRadius)
            hitBox.radius = personajeSO.HitBoxRadius;
    }
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
        return avg.normalized;
    }
    // ==================== COLISIONES ====================

    private void OnCollisionEnter(Collision collision)
    {
        int groundLayerIndex = LayerMask.NameToLayer("Ground");

        if (collision.gameObject.layer != groundLayerIndex)
        {
            SetAndPlayAudioClip(3);
        }
    }
    // ==================== RESPAWN ====================
    public void Respawn()
    {
        Vector3 respawnPos = CheckPoints.GetActiveCheckPointPosition();

        Debug.Log("Respawning at: " + respawnPos);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = respawnPos;
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
        rb.AddForce(gravityForce * gravityMultiplier * Vector3.down, ForceMode.Acceleration);
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
            float steerMultiplier = CheckGrounded() ? 1f : airControlMultiplier;
            float direction = Vector3.Dot(rb.linearVelocity, hitBox.transform.forward) >= 0f ? 1f : -1f;
            float targetAngle = horizontalInput * steerPower * steerMultiplier * direction;
            Quaternion targetRot = Quaternion.LookRotation(
                Quaternion.Euler(0, targetAngle, 0) * hitBox.transform.forward,
                Vector3.up
            );

            hitBox.transform.rotation = Quaternion.Slerp(hitBox.transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        if (CheckGrounded()) ApplyLateralFriction(frictionPower);
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
        SetDriftParticlesColor(drift1Start,drift1End);
    }

    public void ApplyDriftMovement()
    {
        driftTimer += Time.deltaTime;

        currentDriftAngle = Mathf.Lerp(currentDriftAngle,driftAngle, Time.deltaTime * 8f );

        float steerInfluence = horizontalInput * driftPower;

        Quaternion targetRot = driftBaseRotation * Quaternion.Euler(0, currentDriftAngle * driftDirection, 0) *Quaternion.Euler(0, steerInfluence, 0);

        hitBox.transform.rotation = Quaternion.Slerp(hitBox.transform.rotation,targetRot, Time.deltaTime * 10f );

        Vector3 forwardVel = hitBox.transform.forward * rb.linearVelocity.magnitude;

        rb.linearVelocity = Vector3.Lerp( rb.linearVelocity,forwardVel,Time.deltaTime * 3f );

        if (accelerateInput && rb.linearVelocity.magnitude <= maxSpeed * 0.8f)
        {
            rb.AddForce(hitBox.transform.forward * accelerationPower * 0.7f,ForceMode.Acceleration);
        }

        rb.AddForce(-hitBox.transform.right * driftDirection * driftSideForce,ForceMode.Acceleration );

        ApplyLateralFriction(driftFrictionPower * 2f);
        RotateHitboxDrift();
    }


    public void EndDrift()
    {
        driftFlag = false;
        first = second = third = false;
        StopDriftParticles();
        ClearDriftParticles();
    }

    public bool CanBoost() => CheckGrounded() && first;

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
        Color newColorStart = Color.clear;
        Color newColorEnd = Color.clear;


        if (!first && driftTimer > 0.5f)
        {
            driftLevel = 1;
            newColorStart = drift1Start;
            newColorEnd= drift1End;
            first = true;
            colorChanged = true;
        }
        else if (first && !second && driftTimer > 1.5f)
        {
            driftLevel = 2;
            newColorStart = drift2Start;
            newColorEnd = drift2End;
            second = true;
            colorChanged = true;
        }
        else if (first && second && !third && driftTimer > 3f)
        {
            driftLevel = 3;
            newColorStart = drift3Start;

            newColorEnd = drift3End;

            third = true;
            colorChanged = true;
        }

        if (colorChanged)
        {
                StopDriftParticles();
                ClearDriftParticles(); 

                SetDriftParticlesColor(newColorStart, newColorEnd);

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
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(forward.x, v.y, forward.z), Time.deltaTime * 10f);
    }
    // ==================== PODERES ====================
    public void UsePower()
    {
        if (!uiManager.PuedeUsarPoder())
            return;

        uiManager.ConsumirPoder();

        SetAndPlayAudioClip(2);

        if (personajeSO != null)
        {
            personajeSO.UsePower(this);
        }
    }

    // ==================== STUN ====================
    public void StartStun()
    {
        originalVisualRotation = visualModel.localRotation;
    }

    public void StayStunned()
    {
        visualModel.parent.Rotate(
            0f,
            stunSpinSpeed * Time.deltaTime,
            0f,
            Space.Self
        );
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
        Quaternion startRot = visualModel.parent.localRotation;

        Quaternion targetRot = Quaternion.identity;

        float time = 0f;
        float duration = 0.25f;

        while (time < duration)
        {
            time += Time.deltaTime;

            visualModel.parent.localRotation = Quaternion.Slerp(
                startRot,
                targetRot,
                time / duration
            );

            yield return null;
        }

        visualModel.parent.localRotation = Quaternion.identity;
    }
    public void SetStars(bool active)
    {
        stunStars.gameObject.SetActive(active);
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

    public void SetDriftParticlesColor(Color startColor, Color endColor)
    {
        foreach (var p in driftParticles)
        {
            var col = p.colorOverLifetime;
            col.enabled = true;

            Gradient gradient = new Gradient();

            gradient.SetKeys(
                new GradientColorKey[]
                {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
                },
                new GradientAlphaKey[]
                {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
                }
            );

            col.color = new ParticleSystem.MinMaxGradient(gradient);

            var main = p.main;
            main.startColor = Color.white;
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
    //================RUEDAS=====================
    public void RotateX()
    {
        float speed = rb.linearVelocity.magnitude;
        float rot = speed * 5f;

        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right * rot * Time.deltaTime);
        }
    }
    public void RotateY()
    {
        float steer = horizontalInput * 30f;

        foreach (Transform pivot in pivots)
        {
            Quaternion target = Quaternion.Euler(0f, steer, 0f);

            pivot.localRotation = Quaternion.Slerp(
                pivot.localRotation,
                target,
                Time.deltaTime * 10f
            );
        }
    }
    // ==================== UTILIDADES ====================
    public float GetCurrentSpeed() => rb.linearVelocity.magnitude;
    public Transform GetHitboxTransform()
    {
        return hitBox.transform;
    }
    public float GetMaxSpeed() => maxSpeed;
    public void SetAndPlayAudioClip(int index) 
    {
        if (personajeSO == null || personajeSO.audios == null || index < 0 || index >= personajeSO.audios.Length)
        {
            Debug.LogError("FSMManager: Audio clip index out of range or PersonajeSO/audios not assigned.");
            return;
        }
        audioSource.clip = personajeSO.audios[index];
        audioSource.Play();
    }
    /*
    private void OnDrawGizmos()
    {
        if (hitBox == null) return;

        Gizmos.color = Color.yellow;
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
    }*/
}