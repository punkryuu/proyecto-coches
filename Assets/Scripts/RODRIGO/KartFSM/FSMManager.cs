using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class FSMManager : StateMachineFlow
{
    public Idle idleState;
    public Accelerating acceleratingState;
    public Braking brakingState;

    private PlayerInputActions inputActions;
    public Rigidbody rb;
    public float maxSpeed = 100f;
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
        //fallingingState = new Falling(this);
        Debug.Log($"Estados creados: idle={idleState}, acc={acceleratingState}, brake={brakingState}");
        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }
    protected override void GetinitialState(out TemplateStateMachine _stateMachine)
    {
        _stateMachine = idleState;
    }
    // para leer los valorers del newInputSystem
    private void Update()
    {
        accelerateInput = inputActions.Driving.Accelerate.IsPressed();
        brakeInput = inputActions.Driving.Stop.IsPressed();
        steerInput = inputActions.Driving.Steer.ReadValue<float>();
    }
    public bool CheckGrounded() 
    {
        if (Physics.Raycast(transform.position, Vector3.down, 5f)) 
        {
            isGrounded = true;
        }
        else isGrounded = false;
        return isGrounded;
    }

}
