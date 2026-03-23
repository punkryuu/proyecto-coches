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
    public float maxSpeed = 100f;
    public float fordwardSpeed;
    public float acceleration = 10f;
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
        Debug.Log($"Estados creados: idle={idleState}, acc={acceleratingState}, brake={brakingState}");
        inputActions = new PlayerInputActions();
        inputActions.Enable();
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
    public void FordwardMovement() 
    {
        rb.AddForce(fordwardSpeed * transform.forward, ForceMode.Acceleration);
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
