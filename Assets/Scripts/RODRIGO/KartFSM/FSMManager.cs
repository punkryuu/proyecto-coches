using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class FSMManager : StateMachineFlow
{
    public Idle idleState;
    public Accelerating acceleratingState;
    public Braking brakingState;
    public Falling fallingState;

    private PlayerInputActions inputActions;
    public Rigidbody rb;
    public CapsuleCollider hitBox;
    public float maxSpeed = 100f;
    public float fordwardSpeed;
    public float acceleration = 50f;
    public float steer = 25f;
    public bool accelerateInput;
    public bool brakeInput;
    public float steerInput;
    private float currentRotate;
    public bool isGrounded = false;

    public void Awake()
    {
        idleState= new Idle(this);
        acceleratingState = new Accelerating(this);
        brakingState = new Braking(this);
        fallingState = new Falling(this);
        inputActions = new PlayerInputActions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody>();
        hitBox = GetComponentInChildren<CapsuleCollider>(); 
    }
    protected override void GetinitialState(out TemplateStateMachine _stateMachine)
    {
        _stateMachine = idleState;
    }
    // para leer los valorers del newInputSystem
    public bool CheckGrounded() 
    {
        if (Physics.Raycast(transform.position, Vector3.down, 3f)) 
        {
            isGrounded = true;
        }
        else isGrounded = false;
        return isGrounded;
    }
    public PlayerInputActions GetInputActions() 
    {
        return inputActions;
    }
    public void FordwardMovement(Vector3 direction) 
    {
        rb.AddForce(fordwardSpeed * direction, ForceMode.Acceleration);
    }
    public void Steer()
    {
        float targetRotate = 0f;

        if (steerInput != 0)
        {
            targetRotate = steer * steerInput;
        }

        currentRotate = Mathf.Lerp(currentRotate, targetRotate, Time.deltaTime * 4f);

        Vector3 targetRotation = new Vector3(0, transform.eulerAngles.y + currentRotate, 0);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * 5f);
    }
    public void Gravity()
    {
        rb.AddForce(maxSpeed * Vector3.down, ForceMode.Acceleration);
    }
    public void SlowDown(float _slowDown) 
    {
        rb.AddForce(-rb.linearVelocity.normalized * _slowDown, ForceMode.Acceleration);

    }

}
