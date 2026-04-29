using UnityEngine;
using UnityEngine.InputSystem;

public class KartInput : MonoBehaviour {
    ///maneja los inputs del jugador
    [SerializeField] InputActionReference accelerateInput;
    [SerializeField] InputActionReference stopInput;
    [SerializeField] InputActionReference steerInput;
    [SerializeField] InputActionReference driftInput;
    public float AccelerateAmount { get; private set; }
    public float SteerAmount { get; private set; }
    public bool IsDriftPressed { get; private set; }

    private void Update()
    {
        if (accelerateInput.action.IsPressed())
            AccelerateAmount = 1f;
        else if (stopInput.action.IsPressed())
            AccelerateAmount = -1f;
        else
            AccelerateAmount = 0f;

        SteerAmount = steerInput.action.ReadValue<float>();
        IsDriftPressed = driftInput.action.IsPressed();
    }

    private void OnEnable()
    {
        accelerateInput.action.Enable();
        stopInput.action.Enable();
        steerInput.action.Enable();
        driftInput.action.Enable();
    }

    private void OnDisable()
    {
        accelerateInput.action.Disable();
        stopInput.action.Disable();
        steerInput.action.Disable();
        driftInput.action.Disable();
    }
}
