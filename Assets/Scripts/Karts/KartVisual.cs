using System.Collections.Generic;
using UnityEngine;

public class KartVisual : MonoBehaviour {
    [SerializeField] private Transform kartModelParent;
    [SerializeField] private Transform kartModel;
    [SerializeField] private KartInput kartInput;
    [SerializeField] private KartDrift kartDrift;
    [SerializeField] private KartMovement kartMovement;

    private float rotationSmoothTime = 8f;
    private float orientationSmoothTime = 8f;
    private float airOrientationSmoothTime = 8f;

    private List<ParticleSystem> driftParticles = new List<ParticleSystem>();
    private List<ParticleSystem> turboParticles = new List<ParticleSystem>();

    public void SetModel(Transform model, Transform parent)
    {
        kartModel = model;
        kartModelParent = parent;
    }

    public void SetDriftParticles(List<ParticleSystem> particles) => driftParticles = particles;
    public void SetTurboParticles(List<ParticleSystem> particles) => turboParticles = particles;

    private void Start()
    {
        if (kartInput == null) kartInput = GetComponent<KartInput>();
        if (kartDrift == null) kartDrift = GetComponent<KartDrift>();
        if (kartMovement == null) kartMovement = GetComponent<KartMovement>();
    }

    private void Update()
    {
        float horizontal = kartInput.SteerAmount;

        if (!kartDrift.IsDrifting)
        {
            Quaternion targetRotation = Quaternion.Euler(0, horizontal * 15f, 0);
            kartModel.localRotation = Quaternion.Lerp(kartModel.localRotation, targetRotation, Time.deltaTime * rotationSmoothTime);
        }
        else
        {
            float control;
            if (kartDrift.DriftDirection == 1)
                control = Remap(horizontal, -1, 1, 0.25f, 2f);
            else
                control = Remap(horizontal, -1, 1, 2f, 0.25f);

            float targetY = control * 15 * kartDrift.DriftDirection;
            Quaternion targetRotation = Quaternion.Euler(0, targetY, 0);
            kartModel.localRotation = Quaternion.Lerp(kartModel.localRotation, targetRotation, Time.deltaTime * rotationSmoothTime);
        }

        if (kartMovement.Grounded)
        {
            Quaternion targetRot = Quaternion.LookRotation(transform.forward, kartMovement.GroundNormal);
            kartModelParent.rotation = Quaternion.Lerp(kartModelParent.rotation, targetRot, Time.deltaTime * orientationSmoothTime);
        }
        else
        {
            Quaternion targetRot = Quaternion.LookRotation(transform.forward, Vector3.up);
            kartModelParent.rotation = Quaternion.Lerp(kartModelParent.rotation, targetRot, Time.deltaTime * airOrientationSmoothTime);
        }
    }

    public void PlayDriftParticles()
    {
        foreach (var p in driftParticles) p.Play();
    }

    public void StopDriftParticles()
    {
        foreach (var p in driftParticles) p.Stop();
    }

    public void ClearDriftParticles()
    {
        foreach (var p in driftParticles) p.Clear();
    }

    public void SetDriftParticlesColor(Color color)
    {
        foreach (var p in driftParticles)
        {
            var main = p.main;
            main.startColor = color;
        }
    }

    public void PlayTurboParticles()
    {
        foreach (var p in turboParticles) p.Play();
    }

    public void StopTurboParticles()
    {
        foreach (var p in turboParticles) p.Stop();
    }

    private float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}