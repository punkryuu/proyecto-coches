using System;
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
    public Collider hitBox;


    public float maxSpeed = 100f;
    public float accelerationPower = 50f;
    public float brakePower = 25f;

    public float steerPower = 80f;
    private float currentRotate;
    private float targetRotate;

    public float gravityForce = 50f;

    public bool accelerateInput;
    public bool brakeInput;
    public float steerInput;
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
        hitBox = GetComponentInChildren<Collider>(); 
    }
    protected override void GetinitialState(out TemplateStateMachine _stateMachine)
    {
        _stateMachine = idleState;
    }
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
    public void ApplyAcceleration(float power) 
    {
        rb.AddForce(transform.forward * power, ForceMode.Acceleration);
    }
    public void ApplyBrake(float power)
    {
        rb.AddForce(-transform.forward * power, ForceMode.Acceleration);
    }

        if (steerInput != 0)
        {
            targetRotate = steer * steerInput;
        }
        float currentSpeed = rb.linearVelocity.magnitude;
        targetRotate = steerInput * steerPower;


    }
    public void ApplySteer()
    {
        currentRotate = Mathf.Lerp(currentRotate, targetRotate, Time.deltaTime * 4f);

        Vector3 targetRotation = new Vector3(0, transform.eulerAngles.y + currentRotate, 0);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * 5f);
    }
    public void ApplyGravity()
    {
        rb.AddForce(gravityForce * Vector3.down, ForceMode.Acceleration);
    }
    public void ApplySlowDown(float _slowDown) 
    {
        rb.AddForce(-rb.linearVelocity.normalized * _slowDown, ForceMode.Acceleration);

    }

}
