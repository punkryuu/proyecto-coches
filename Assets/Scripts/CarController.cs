using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] Transform kartModel;
    [SerializeField] Rigidbody sphere;
    [SerializeField] InputActionReference accelerateInput;
    [SerializeField] InputActionReference steerInput;

    float speed, currentSpeed;
    float rotate, currentRotate;


    // estos son los paremetros que pillaria luego de los SO de los personajes
    float maxSpeed = 40;
    float acceleration = 100f;
    float steering = 50f;
    float gravity = 100f;
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
        float s = steerInput.action.ReadValue<float>();
        if (s != 0)
        {
            int dir;
            if (s > 0)
                dir = 1;
            else dir = -1;
            float amount = Mathf.Abs(s);
            Steer(dir, amount);
             
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
            sphere.GetComponent<Rigidbody>().AddForce(kartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
        Debug.Log(sphere.linearVelocity.magnitude);
        // ahora caigo
        if (!Physics.Raycast(transform.position, Vector3.down, 3))
        {
            sphere.GetComponent<Rigidbody>().AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
        // girar
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);
        //Normal rotation
        kartModel.parent.up = Vector3.Lerp(kartModel.parent.up, hitNear.normal, Time.deltaTime*8f);
        kartModel.parent.Rotate(0, transform.eulerAngles.y, 0);
    }
    public void Steer(int direction, float amount) 
    {
        rotate = (steering * direction) * amount;
    }
}
