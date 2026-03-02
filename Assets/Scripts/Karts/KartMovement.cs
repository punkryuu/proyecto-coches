using UnityEngine;

public class KartMovement : MonoBehaviour
{
    [SerializeField] Rigidbody sphereRb;
    [SerializeField] private KartInput kartInput;
    [SerializeField] private KartDrift kartDrift;
    [SerializeField] private Transform carVisualParent;

    //stats
    float baseMaxSpeed = 50f; //velocidad máxima que puede alcanzar el coche
    float baseAcceleration = 100f;//cuanto tarda en alcanzar la velocidad m�xima
    float baseSteering = 25f; // cuanto gira normal
    float baseWeight = 100f;// cuanto tarda en caer (gravedad artificial)
    float baseDriftControl = 30f;// cuanto gira derrapando
    float baseAirControl = 0.25f;// redducción de control en el aire

    // multiplicadores que pilla  de los SO de los personajes
    float maxSpeedMultiplier = 1f;
    float accelerationMultiplier = 1f;
    float steeringMultiplier = 1f;
    float weightMultiplier = 1f;
    float driftControlMultiplier = 1f;
    float airControlMultiplier = 1f;

    //offset
    float verticalOffset;
    float horizontalOffset;

    //groundCheck
    byte groundCheckDistance = 3;

    float speed, currentSpeed;
    float rotate, currentRotate;
    bool grounded;
    RaycastHit goundHit;
    public bool Grounded => grounded;
    public Vector3 GroundNormal => goundHit.normal;
    public float CurrentAcceleration => currentSpeed;

    private void Start()
    {
        if (kartInput == null) kartInput = GetComponent<KartInput>();
        if (kartDrift == null) kartDrift = GetComponent<KartDrift>();
    }
    private void Update()
    {
        grounded = Physics.Raycast(sphereRb.position, Vector3.down, out goundHit, groundCheckDistance);

        Vector3 desiredHorizontalOffset = transform.rotation * new Vector3(0f, 0f, -horizontalOffset);
        Vector3 horizontalOffsetOnPlane = Vector3.ProjectOnPlane(desiredHorizontalOffset, GroundNormal);
        transform.position = sphereRb.position - horizontalOffsetOnPlane - GroundNormal * verticalOffset;
        
    }
    private void FixedUpdate()
    {
        float targetSpeed = 0f;
        if (kartInput.AccelerateAmount > 0 && grounded)
            targetSpeed = baseAcceleration * accelerationMultiplier * kartInput.AccelerateAmount;
        else if (kartInput.AccelerateAmount < 0 && grounded)
            targetSpeed = -baseAcceleration / 2f * accelerationMultiplier * Mathf.Abs(kartInput.AccelerateAmount);

        if (grounded && !kartDrift.IsBoosting)
            currentSpeed = Mathf.SmoothStep(currentSpeed, targetSpeed, Time.deltaTime * 12f);
        //calcular giro
        float targetRotate = 0f;
        if (kartDrift.IsDrifting)
        {
            targetRotate = CalculateSteering(kartDrift.DriftDirection, kartDrift.CurrentDriftControl);
        }
        else if (kartInput.SteerAmount != 0)
        {
            sbyte dir;
            if ((sbyte)(kartInput.SteerAmount) > 0)
            {
                dir = 1;
            }
            else dir = -1;
            float amount = Mathf.Abs(kartInput.SteerAmount);
            targetRotate = CalculateSteering(dir, amount);
        }
        currentRotate = Mathf.Lerp(currentRotate, targetRotate, Time.deltaTime * 4f);

        //aplicar movimienmto
        if (grounded)
        {
            if (kartDrift.IsBoosting)
            {
                float turboMaxSpeed = baseMaxSpeed * maxSpeedMultiplier * kartDrift.TurboSpeedMultiplier;
                Vector3 targetDirection;
                if (kartDrift.IsDrifting) { targetDirection = transform.forward; }
                else { targetDirection = carVisualParent.forward; }
                targetDirection.Normalize();
                float currentVerticalSpeed = sphereRb.linearVelocity.y;
                sphereRb.linearVelocity = new Vector3(targetDirection.x * turboMaxSpeed, currentVerticalSpeed, targetDirection.z * turboMaxSpeed);
            }
            else
            {
                float currentMaxSpeed = baseMaxSpeed * maxSpeedMultiplier;
                if (sphereRb.linearVelocity.magnitude < currentMaxSpeed)
                {
                    Vector3 direction = kartDrift.IsDrifting ? transform.forward : carVisualParent.forward;
                    sphereRb.AddForce(direction * currentSpeed, ForceMode.Acceleration);
                }
            }


            
        }
        //gravedad
            if (!grounded)
            sphereRb.AddForce(Vector3.down * baseWeight * weightMultiplier, ForceMode.Acceleration);

        Vector3 targetRotation = new Vector3(0, transform.eulerAngles.y + currentRotate, 0);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * 5f);
    }
    private float CalculateSteering(sbyte direction, float amount)
    {
        float steeringForce;
        if (!kartDrift.IsDrifting)
        {
            steeringForce = baseSteering * steeringMultiplier;
        }
        else
        {
            steeringForce = baseDriftControl * driftControlMultiplier;
        }
        if (!grounded)
        {
            steeringForce *= baseAirControl * airControlMultiplier;
        }
        return steeringForce * direction * amount;
    }
    public void SetOffsets(float vertical, float horizontal)
    {
        verticalOffset = vertical;
        horizontalOffset = horizontal;
    }
    public float GetCurrentSpeed()
    {
        if (sphereRb == null || !sphereRb.gameObject.activeInHierarchy)
            return 0;
        return sphereRb.linearVelocity.magnitude * 3.6f;// Convertir de m/s a km/h
    }
    public void Initialize(PersonajeSO so)
    {
        maxSpeedMultiplier = so.maxSpeedMultiplier;
        accelerationMultiplier = so.accelerationMultiplier;
        steeringMultiplier = so.steeringMultiplier;
        weightMultiplier = so.weightMultiplier;
        driftControlMultiplier = so.driftControlMultiplier;
        airControlMultiplier = so.airControlMultiplier;

        verticalOffset = so.verticalOffset;
        horizontalOffset = so.horizontalOffset;
    }
}
