
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

    // estos son los paremetros que pillaria luego de los SO de los personajes
    float maxSpeed = 40; //velocidad maxima que puede alcanzar el coche (supongo que se podria igualar a los km mas o menos)
    float acceleration = 100;// cuanto tarda en alcanzar la velocidad maxima
    float steering = 50;//cuanto gira
    float gravity = 100;//como de rápido cae cuando esta en el aire
    float driftControl = 50;//cuanto gira cuando derrapa
    float turbo =1;//vgelocidad de carga y potencia del turbo
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
        if (accelerateInput.action.IsPressed())
            speed = acceleration;
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
        if (driftInput.action.IsPressed() && !isDrifting && horizontal != 0)
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
        driftPower += powerControl * turbo;
    }
    public void EndDrift() 
    {
        isDrifting = false;
        driftPower = 0;
    }
    public void UpdateValues()
    {
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;
    }
    
    //funciones del FixedUpdate
    public void Movement() 
    {
        if (sphere.linearVelocity.magnitude < maxSpeed)
        {
            if (!isDrifting)
                sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
            else
                sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        }
        Debug.Log(sphere.linearVelocity.magnitude);
    }
    public void Gravity() 
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 3))
        {
            sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
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
    public void Steer(sbyte direction, float amount) 
    {
        float steeringForce;
        if (!isDrifting) { steeringForce = steering; }
        else { steeringForce = driftControl; }
        rotate = (steeringForce * direction) * amount;
    }
    public float Remap(float value, float from1, float to1, float from2, float to2) 
    { 
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
