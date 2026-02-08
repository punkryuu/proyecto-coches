using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] Transform KartModel;
    [SerializeField] Rigidbody sphere;
    [SerializeField] InputActionReference accelerateInput;
    [SerializeField] InputActionReference steerInput;

    float speed, currentSpeed;
    float rotate, currentRotate;
   

    // estos son los paremetros que pillaria luego de los SO de los personajes
    float acceleration = 30f;
    float steering = 30f;
    float gravity = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //follow Collider
        transform.position = sphere.position;
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
        currentSpeed = Mathf.SmoothStep(currentSpeed,speed, Time.deltaTime*12f);
        speed = 0f;
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;
    }
    private void FixedUpdate()
    {
        //acelero segun la normal del modelo
        sphere.AddForce(KartModel.transform.forward * currentSpeed, ForceMode.Acceleration);
        //ahora caigo
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        //giro
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, 
            new Vector3(0, transform.eulerAngles.y + currentRotate, 0), Time.deltaTime * 5f);

        RaycastHit hitOn;
        RaycastHit hitNear;

        Physics.Raycast(transform.position, Vector3.down, out hitOn, 1.1f);
        Physics.Raycast(transform.position, Vector3.down, out hitNear, 2.0f);

        //Normal rotation
        KartModel.parent.up = Vector3.Lerp(KartModel.parent.up, hitNear.normal, Time.deltaTime*8f);
        KartModel.parent.Rotate(0, transform.eulerAngles.y, 0);
    }
    public void Steer(int direction, float amount) 
    {
        rotate = (steering * direction) * amount;
    }
}
