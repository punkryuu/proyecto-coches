
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class CarController : MonoBehaviour
{
    [SerializeField] Transform kartModel;
    [SerializeField] Rigidbody sphere;
    [SerializeField] InputActionReference accelerateInput;
    [SerializeField] InputActionReference steerInput;
    [SerializeField] InputActionReference driftInput;

    float speed, currentSpeed;
    float rotate, currentRotate;
    bool isDrifting = false;
    sbyte driftDir;
    float driftPower;
    byte groundCheckDistance = 3;

    float baseMaxSpeed = 50f; //velocidad máxima que puede alcanzar el coche
    float baseAcceleration = 100f;//cuanto tarda en alcanzar la velocidad máxima
    float baseSteering = 25f; // cuanto gira normal
    float baseWeight = 100f;// cuanto tarda en caer (gravedad artificial)
    float baseDriftControl = 30f;// cuanto gira derrapando
    float baseTurbo = 1f; // cuanto tarda en cargar el turbo (por hacer)
    float baseAirControl = 0.25f;// redducción de control en el aire


    // estos son los paremetros que pillaria luego de los SO de los personajes
    float maxSpeedMultiplier = 1f;
    float accelerationMultiplier = 1f;
    float steeringMultiplier = 1f;
    float weightMultiplier = 1f;
    float driftControlMultiplier = 1f;
    float turboMultiplier = 1f;
    float airControlMultiplier = 1f;
    Vector3 ajustePosicionCoche=new Vector3(0.1f, 0.3f, 0);//cambiar segun el  mmodleo para que no quede flotando al bajar rampas ni se clipee en el suelo

    RaycastHit hitOn;
    RaycastHit hitNear;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowCollider();
        AccelerationInput();
        SteerInput();
        DriftInput();
        UpdateValues();
        CarAnimations();
    }
    private void FixedUpdate()
    {
        Movement();
        Gravity();
        VisualRotation();
        VisualOrientation();
    }
    //funciones del update
    public void FollowCollider()
    {
        transform.position = sphere.position - ajustePosicionCoche;
    }
    public void AccelerationInput() 
    {
        if (accelerateInput.action.IsPressed() && IsTouchingFloor())
            speed = baseAcceleration*accelerationMultiplier;
    }
    public void SteerInput() 
    {
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
    }
    public void DriftInput() 
    {
        float horizontal = steerInput.action.ReadValue<float>();
        //iniciar derrape
        if (driftInput.action.IsPressed() && !isDrifting && horizontal != 0 && IsTouchingFloor())
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
    }
    private void StartDrift(float horizontalInput)
    {
        isDrifting = true;
        if (horizontalInput > 0)
            driftDir = 1;
        else driftDir = -1;
        driftPower = 0f;
    }
    private void ProcessDrift(float horizontalInput) 
    {
        float control;
        float powerControl;
        if (driftDir == 1)
        {
            control = Remap(horizontalInput, -1, 1, 0, 2);
            powerControl = Remap(horizontalInput, -1, 1, .2f, 1);
        }
        else
        {
            control = Remap(horizontalInput, -1, 1, 2, 0);
            powerControl = Remap(horizontalInput, -1, 1, 1, .2f);
        }


        Steer(driftDir, control);
        driftPower += powerControl * baseTurbo*turboMultiplier;
    }
    public void EndDrift() 
    {
        isDrifting = false;
        driftPower = 0;
    }
    public void UpdateValues()
    {
        if (IsTouchingFloor())
            { currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f); }
        speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;
    }
    public void CarAnimations()
    {
        float horizontal = steerInput.action.ReadValue<float>();
        if (!isDrifting)
        {
            Quaternion targetRotation = Quaternion.Euler(0,horizontal * 15f,0);
            kartModel.localRotation = Quaternion.Lerp(kartModel.localRotation, targetRotation, Time.deltaTime*8f);
        }
        else 
        {
            float control;
            if (driftDir == 1)
            {
                control = Remap(horizontal, -1, 1, .5f, 2);
            }
            else
            {
                control = Remap(horizontal, -1, 1, 2, .5f);
            }
            kartModel.parent.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(kartModel.parent.localEulerAngles.y, (control * 15) * driftDir, .2f), 0);
        }
    }


    //funciones del FixedUpdate
    public void Movement() 
    {
        if (sphere.linearVelocity.magnitude < baseMaxSpeed*maxSpeedMultiplier && IsTouchingFloor())
        {
            if (!isDrifting)
                { sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration); }
            else
                { sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration); }

        }
        Debug.Log(sphere.linearVelocity.magnitude);
    }
    public void Gravity() 
    {
        if (!IsTouchingFloor())
        {
            sphere.AddForce(Vector3.down * baseWeight*weightMultiplier, ForceMode.Acceleration);
        }
    }
    public void VisualRotation() 
    {
        Vector3 targetRotation = new Vector3(0, transform.eulerAngles.y + currentRotate, 0);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * 5f);

    }
    public void VisualOrientation()
    {
        Physics.Raycast(transform.position, Vector3.down, out hitOn, 1.1f);
        //ajustar el valor del final si no se gira el modelo al subir rampas
        Physics.Raycast(transform.position, Vector3.down, out hitNear, 3.0f);
        kartModel.parent.up = Vector3.Lerp(kartModel.parent.up, hitNear.normal, Time.deltaTime * 8f);
        kartModel.parent.Rotate(0, transform.eulerAngles.y, 0);
    }
    public bool IsTouchingFloor() 
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
    public void Steer(sbyte direction, float amount) 
    {
        float steeringForce;
        if (!isDrifting) { steeringForce = baseSteering*steeringMultiplier; }
        else { steeringForce = baseDriftControl*driftControlMultiplier; }
        if (!IsTouchingFloor()){ steeringForce *= baseAirControl*airControlMultiplier; }
        rotate = (steeringForce * direction) * amount;
    }
    public float Remap(float value, float from1, float to1, float from2, float to2) 
    { 
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
