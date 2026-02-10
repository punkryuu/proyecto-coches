
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] Transform kartModel;
    [SerializeField] Rigidbody sphere;
    [SerializeField] InputActionReference accelerateInput;
    [SerializeField] InputActionReference steerInput;
    [SerializeField] InputActionReference driftInput;

    float speed, currentSpeed;
    float rotate, currentRotate;
    bool drifting = false;
    sbyte driftDir;
    float driftPower;

    // estos son los paremetros que pillaria luego de los SO de los personajes
    float maxSpeed = 40;
    float acceleration = 100;
    float steering = 50;
    float gravity = 100;
    float driftControl = 1;
    float turbo =1;
    Vector3 ajustePosicionCoche=new Vector3(0.1f, 0.3f, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //follow Collider
        transform.position = sphere.position- ajustePosicionCoche;

        //acelerar
        if (accelerateInput.action.IsPressed())
            speed = acceleration;


        //girar
        float horizontal = steerInput.action.ReadValue<float>();
        if (horizontal != 0 )
        {
            sbyte dir;
            if (horizontal > 0)
                dir = 1;
            else dir = -1;
            float amount = Mathf.Abs(horizontal);
            Steer(dir, amount);
           
        }
        // derrapar

        if (driftInput.action.IsPressed() && !drifting && horizontal != 0)
        {
            drifting = true;
            if (horizontal > 0)
                driftDir = 1;
            else driftDir = -1;
        }
        if (drifting) 
        {
            //this float value, float from1, float to1, float from2, float to2
            //(value - from1) / (to1 - from1) * (to2 - from2) + from2;
            float control;
            float powerControl;
            if (driftDir == 1)
            {
                control = Remap(horizontal,-1, 1, 0, 2);
                powerControl = Remap(horizontal, -1, 1, .2f, 1);
            }
            else
            {
                control = Remap(horizontal, -1, 1, 2, 0);
                powerControl = Remap(horizontal, -1, 1, 1, .2f);
            }
      

            Steer(driftDir, control);
            driftPower += powerControl*turbo;
        }
        if (!driftInput.action.IsPressed() && drifting)
        {
            drifting = false;
            driftPower = 0;
        }
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;
    }
    private void FixedUpdate()
    {
        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position, Vector3.down, out hitOn, 1.1f);

        //ajustar el valor del final si no se gira el modelo al subir rampas
        Physics.Raycast(transform.position, Vector3.down, out hitNear, 3.0f);

        // palante
        if (sphere.linearVelocity.magnitude < maxSpeed)
        {
            if (!drifting)
                sphere.AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
            else
                sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        }
        Debug.Log(sphere.linearVelocity.magnitude);
        // ahora caigo
        if (!Physics.Raycast(transform.position, Vector3.down, 3))
        {
            sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
        // girar
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);
        //Normal rotation
        kartModel.parent.up = Vector3.Lerp(kartModel.parent.up, hitNear.normal, Time.deltaTime*8f);
        kartModel.parent.Rotate(0, transform.eulerAngles.y, 0);
    }
    public void Steer(sbyte direction, float amount) 
    {
        if (!drifting)
            rotate = (steering * direction) * amount;
        else
            rotate = (driftControl * direction) * amount;
    }
    public float Remap(float value, float from1, float to1, float from2, float to2) 
    { 
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
