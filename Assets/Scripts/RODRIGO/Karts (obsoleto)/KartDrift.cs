using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//majena el derrape y el turbo
public class KartDrift : MonoBehaviour {
    //referencias
    [SerializeField] private KartInput kartInput;
    [SerializeField] private KartMovement kartMovement;
    [SerializeField] private KartVisual kartVisual;

    //stats
    float baseTurboPower = 2f;
    float baseTurboChargePW1 = 100f;
    float baseTurboChargePW2 = 200f;
    float baseTurboChargePW3 = 300f;

    float baseTurboDuration = 0.5f;

    //multiplicadores
    float turboMultiplier = 1f;
    public bool IsDrifting { get; private set; }
    public bool IsBoosting { get; private set; }
    public sbyte DriftDirection { get; private set; }
    public float CurrentDriftControl { get; private set; }
    public float TurboSpeedMultiplier { get; private set; } = 1f;

    private float driftPower;
    private byte driftMode;
    private bool first, second, third;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (kartInput == null) kartInput = GetComponent<KartInput>();
        if (kartMovement == null) kartMovement = GetComponent<KartMovement>();
        if (kartVisual == null) kartVisual = GetComponent<KartVisual>();
    }
    public void Initialize(PersonajeSO so)
    {
        turboMultiplier = so.turboMultiplier;
    }
    private void Update()
    {
        if (kartInput.IsDriftPressed && !IsDrifting && kartInput.SteerAmount != 0 && kartMovement.Grounded)
            StartDrift(kartInput.SteerAmount);

        if (IsDrifting)
            ProcessDrift(kartInput.SteerAmount);

        if (!kartInput.IsDriftPressed && IsDrifting)
            EndDrift();
    }

    private void StartDrift(float horizontalInput)
    {
        IsDrifting = true;
        DriftDirection = (sbyte)(horizontalInput > 0 ? 1 : -1);
        driftPower = 0f;
        CurrentDriftControl = 1f;

        kartVisual.ClearDriftParticles();
        kartVisual.SetDriftParticlesColor(Color.yellow);
    }

    private void ProcessDrift(float horizontalInput)
    {
        float control;
        float powerControl;
        if (DriftDirection == 1)
        {
            control = Remap(horizontalInput, -1, 1, .5f, 2);
            powerControl = Remap(horizontalInput, -1, 1, .2f, 1);
        }
        else
        {
            control = Remap(horizontalInput, -1, 1, 2, .5f);
            powerControl = Remap(horizontalInput, -1, 1, 1, .2f);
        }

        CurrentDriftControl = control;
        driftPower += powerControl * baseTurboPower * turboMultiplier;
        UpdateDriftLevel();
    }

    public void EndDrift()
    {
        IsDrifting = false;
        Boost();
        kartVisual.StopDriftParticles();
        driftPower = 0;
    }
    public void Boost()
    {
        if (driftMode > 0)
        {
            float duration = baseTurboDuration * driftMode * turboMultiplier; // 0.5, 1, 1.5 segundos por defecto
            TurboSpeedMultiplier = 1f + (0.2f*turboMultiplier);
            StartCoroutine(BoostRoutine(duration));
            driftMode = 0;
        }
    }
    private IEnumerator BoostRoutine(float duration)
    {
        kartVisual.PlayTurboParticles();
        IsBoosting = true;
        yield return new WaitForSeconds(duration);
        kartVisual.StopTurboParticles();
        IsBoosting = false;
        TurboSpeedMultiplier = 1f;
        first = second = third = false;
    }
    private void UpdateDriftLevel()
    {
        bool colorChanged = false;
        Color newColor = Color.clear;

        if (!first && driftPower > baseTurboChargePW1)
        {
            newColor = Color.yellow;
            driftMode = 1;
            first = true;
            colorChanged = true;
        }
        else if (first && !second && driftPower > baseTurboChargePW2)
        {
            newColor = Color.red;
            driftMode = 2;
            second = true;
            colorChanged = true;
        }
        else if (first && second && !third && driftPower > baseTurboChargePW3)
        {
            newColor = Color.cyan;
            driftMode = 3;
            third = true;
            colorChanged = true;
        }

        if (colorChanged)
        {
            kartVisual.SetDriftParticlesColor(newColor);
            kartVisual.PlayDriftParticles();
        }
    }
    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
   
}
