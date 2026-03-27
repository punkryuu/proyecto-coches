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


    public const float maxSpeed = 50f;
    public const float accelerationPower = 20f;
    public const float brakePower = 25f;

    public const float steerPower = 25f;
    public const float frictionStrength = 2f;
    private float currentRotate;
    private float targetRotate;

    public  const float gravityForce = 100f;

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
    public void ApplyAcceleration(float power = accelerationPower) 
    {
        rb.AddForce(hitBox.transform.forward * power, ForceMode.Acceleration);
    }
    public void ApplyBrake(float power = brakePower)
    {
        rb.AddForce(-hitBox.transform.forward * power, ForceMode.Acceleration);
    }
    
    public void ApplySteer()
    {
        float targetRotate = 0f;

        if (steerInput != 0)
        {
            targetRotate = steerPower * steerInput;
        }

        currentRotate = Mathf.Lerp(currentRotate, targetRotate, Time.deltaTime * 4f);
        Vector3 targetRotation = new Vector3(0, hitBox.transform.eulerAngles.y + currentRotate, 0);
        hitBox.transform.eulerAngles = Vector3.Lerp(hitBox.transform.eulerAngles, targetRotation, Time.deltaTime * 5f);
    }
    public void ApplyLateralFriction(float frictionStrength = frictionStrength)
    {
        Vector3 lateralVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, hitBox.transform.forward);
        rb.AddForce(-lateralVelocity * frictionStrength, ForceMode.Acceleration);
    }
    public void ApplyGravity()
    {
        rb.AddForce(gravityForce * Vector3.down, ForceMode.Acceleration);
    }
    public void ApplySlowDown(float _slowDown) 
    {
        rb.AddForce(-rb.linearVelocity * _slowDown, ForceMode.Acceleration);
    }

}
