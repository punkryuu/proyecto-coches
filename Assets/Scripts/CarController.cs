using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class CarController : MonoBehaviour {
    [SerializeField] Transform visual;//contiene el modelo del coche y las particulas
    [SerializeField] Rigidbody sphereRb;
    [SerializeField] InputActionReference accelerateInput;
    [SerializeField] InputActionReference stopInput;
    [SerializeField] InputActionReference steerInput;
    [SerializeField] InputActionReference driftInput;
    [SerializeField] Transform driftParticles;
    [SerializeField] Transform turboParticles;
    float speed, currentSpeed;
    float rotate, currentRotate;
    bool isDrifting = false;
    bool isBoosting = false;
    bool first, second, third;
    sbyte driftDir;
    float driftPower;
    byte driftMode;
    byte groundCheckDistance = 3;
    List<ParticleSystem> driftParticlesList = new List<ParticleSystem>();
    Color c;
    List<ParticleSystem> turboParticlesList = new List<ParticleSystem>();
    float baseMaxSpeed = 50f; //velocidad máxima que puede alcanzar el coche
    float baseAcceleration = 100f;//cuanto tarda en alcanzar la velocidad máxima
    float baseSteering = 25f; // cuanto gira normal
    float baseWeight = 100f;// cuanto tarda en caer (gravedad artificial)
    float baseDriftControl = 30f;// cuanto gira derrapando
    float baseTurboPower = 2f;
    float baseTurboCharge = 50f;
    float baseTurboDuration = 0.5f; // cuanto tarda en cargar el turbo
    float baseAirControl = 0.25f;// redducción de control en el aire

    // estos son los paremetros que pillaria luego de los SO de los personajes
    float maxSpeedMultiplier = 1f;
    float accelerationMultiplier = 1f;
    float steeringMultiplier = 1f;
    float weightMultiplier = 1f;
    float driftControlMultiplier = 1f;
    float turboMultiplier = 1f;
    float airControlMultiplier = 1f;
    bool grounded;



    Vector3 ajustePosicionCoche = new Vector3(.1f, .3f, 0);//cambiar segun el  mmodleo para que no quede flotando al bajar rampas ni se clipee en el suelo


    RaycastHit hitNear;

    // Start is called before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < driftParticles.GetChild(0).childCount; i++)
        {
            driftParticlesList.Add(driftParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < driftParticles.GetChild(1).childCount; i++)
        {
            driftParticlesList.Add(driftParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }
        for (int i = 0; i < turboParticles.GetChild(0).childCount; i++)
        {
            turboParticlesList.Add(turboParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
        }

        for (int i = 0; i < turboParticles.GetChild(1).childCount; i++)
        {
            turboParticlesList.Add(turboParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
        }


    }

    // Update is called once per frame
    void Update()
    {
        grounded = IsTouchingFloor();
        // FollowCollider
        transform.position = sphereRb.position - ajustePosicionCoche;

        // AccelerationInput
        speed = 0f;
        if (accelerateInput.action.IsPressed() && grounded)
        {
            speed = baseAcceleration * accelerationMultiplier;
        }
        else if (stopInput.action.IsPressed() && grounded)
        {
            speed = -baseAcceleration / 2 * accelerationMultiplier;
        }

        // SteerInput
        float horizontal = steerInput.action.ReadValue<float>();
        if (horizontal != 0)
        {
            sbyte dir;
            if (horizontal > 0)
                dir = 1;
            else dir = -1;
            float amount = Mathf.Abs(horizontal);
            Steer(dir, amount);
        }

        // DriftInput
        //iniciar derrape
        if (driftInput.action.IsPressed() && !isDrifting && horizontal != 0 && grounded)
        {
            StartDrift(horizontal);
        }
        if (isDrifting)
        {
            ProcessDrift(horizontal);
        }
        if (!driftInput.action.IsPressed() && isDrifting)
        {
            EndDrift();
        }

        // UpdateValues
        if (grounded && !isBoosting)
        { currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); }
        speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;

        // CarAnimations
        if (!isDrifting)
        {
            Quaternion targetRotation = Quaternion.Euler(0, horizontal * 15f, 0);
            visual.localRotation = Quaternion.Lerp(visual.localRotation, targetRotation, Time.deltaTime * 8f);
        }
        else
        {
            float control;
            if (driftDir == 1)
            {
                control = Remap(horizontal, -1, 1, .25f, 2);
            }
            else
            {
                control = Remap(horizontal, -1, 1, 2, .25f);
            }
            float targetY = (control * 15) * driftDir;
            Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);
            visual.localRotation = Quaternion.Lerp(visual.localRotation, targetRotation, Time.deltaTime * 8f);
        }
    }

    private void FixedUpdate()
    {
        // Movement
        if (sphereRb.linearVelocity.magnitude < baseMaxSpeed * maxSpeedMultiplier && grounded)
        {
            if (!isDrifting)
            { sphereRb.AddForce(visual.transform.forward * currentSpeed, ForceMode.Acceleration); }
            else
            { sphereRb.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration); }
        }

        // Gravity
        if (!grounded)
        {
            sphereRb.AddForce(Vector3.down * baseWeight * weightMultiplier, ForceMode.Acceleration);
        }

        // VisualRotation
        Vector3 targetRotation = new Vector3(0, transform.eulerAngles.y + currentRotate, 0);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * 5f);

        // VisualOrientation
        if (Physics.Raycast(transform.position, Vector3.down, out hitNear, groundCheckDistance))
        {
            visual.parent.up = Vector3.Lerp(visual.parent.up, hitNear.normal, Time.deltaTime * 8f);
            visual.parent.Rotate(0, transform.eulerAngles.y, 0);
        }
        else
        {
            Quaternion targetRot = Quaternion.LookRotation(transform.forward, Vector3.up);
            visual.parent.rotation = Quaternion.Lerp(visual.parent.rotation, targetRot, Time.deltaTime * 4f);
        }
    }

    // Resto de funciones auxiliares (se mantienen igual)
    private void StartDrift(float horizontalInput)
    {
        isDrifting = true;
        driftDir = (sbyte)(horizontalInput > 0 ? 1 : -1);
        driftPower = 0f;
        foreach (ParticleSystem p in driftParticlesList)
        {
            Debug.Log("Playing particle: " + p.name);
            var main = p.main;
            p.Play();
        }
    }

    private void ProcessDrift(float horizontalInput)
    {
        float control;
        float powerControl;
        if (driftDir == 1)
        {
            control = Remap(horizontalInput, -1, 1, .5f, 2);//EL .5 ES EL GIRO MINIMO QUE PUEDE HACER Y EL 2 EL MAXIMO
            powerControl = Remap(horizontalInput, -1, 1, .2f, 1);//Cuanto mas derrapes más rapido carga el turbo
        }
        else
        {
            control = Remap(horizontalInput, -1, 1, 2, .5f);
            powerControl = Remap(horizontalInput, -1, 1, 1, .2f);
        }

        Steer(driftDir, control);
        driftPower += powerControl * baseTurboPower * turboMultiplier;
        UpdateDriftlevel();
    }

    public void EndDrift()
    {
        isDrifting = false;
        Boost();
        Color c = Color.clear;
        foreach (ParticleSystem p in driftParticlesList)
        {
            var main = p.main;
            main.startColor = c;
        }

        foreach (ParticleSystem p in driftParticlesList)
        {
            p.Stop();
        }
        driftPower = 0;
    }

    public void Boost()
    {
        if (driftMode > 0)
        {
            float duration = baseTurboDuration * driftMode * turboMultiplier; // 0.5, 1, 1.5 segundos por defecto
            float startSpeed = currentSpeed;
            float boostedSpeed = startSpeed * 3f;
            StartCoroutine(BoostRoutine(startSpeed, boostedSpeed, duration));
            driftMode = 0;
        }
    }

    IEnumerator BoostRoutine(float _startSpeed, float _boostedSpeed, float _duration)
    {
        foreach (ParticleSystem p in turboParticlesList)
        {
            Debug.Log("Playing particle: " + p.name);
            var main = p.main;
            p.Play();
        }
        isBoosting = true;
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            float t = elapsed / _duration;
            currentSpeed = Mathf.Lerp(_boostedSpeed, _startSpeed, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        foreach (ParticleSystem p in turboParticlesList)
        {
            var main = p.main;
            p.Stop();
        }
        currentSpeed = _startSpeed;
        first = second = third = false;
        isBoosting = false;
    }

    public void UpdateDriftlevel()
    {
        bool colorChanged = false;
        if (!first && driftPower > baseTurboCharge)
        {
            c = Color.yellow;
            driftMode = 1;
            first = true;
            colorChanged = true;
        }
        else if (first && !second && driftPower > baseTurboCharge * 3f)
        {
            c = Color.red;
            driftMode = 2;
            second = true;
            colorChanged = true;

        }
        else if (first && second && !third && driftPower > baseTurboCharge * 5f)
        {
            c = Color.cyan;
            driftMode = 3;
            third = true;
            colorChanged = true;

        }
        if (colorChanged) 
        {
            foreach (ParticleSystem p in driftParticlesList)
            {
                var main = p.main;
                main.startColor = c;
                p.Stop();
                p.Play();
            }
        }
       
    }

    public bool IsTouchingFloor()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    public void Steer(sbyte direction, float amount)
    {
        float steeringForce;
        if (!isDrifting) { steeringForce = baseSteering * steeringMultiplier; }
        else { steeringForce = baseDriftControl * driftControlMultiplier; }
        if (!grounded) { steeringForce *= baseAirControl * airControlMultiplier; }
        rotate = (steeringForce * direction) * amount;
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}