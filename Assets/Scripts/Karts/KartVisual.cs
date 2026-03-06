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
        float tiltAngle;

        if (!kartDrift.IsDrifting)
        {
            tiltAngle = horizontal * 15f;
            /*
            Quaternion targetRotation = Quaternion.Euler(0, horizontal * 15f, 0);
            kartModelParent.transform.localRotation = Quaternion.Lerp(kartModelParent.transform.localRotation, targetRotation, Time.deltaTime * rotationSmoothTime);
            */
        }
        else
        {
            float control;
            if (kartDrift.DriftDirection == 1)
            {
                control = Remap(horizontal, -1, 1, .25f, 2);
            }
            else
            {
                control = Remap(horizontal, -1, 1, 2, .25f);
            }
            tiltAngle = (control * 15f) * kartDrift.DriftDirection;
        }
        Quaternion targetRot;
        if (kartMovement.Grounded)
        {
            Vector3 forwardProjected = Vector3.ProjectOnPlane(transform.forward, kartMovement.GroundNormal).normalized;
            Quaternion groundRotation = Quaternion.LookRotation(forwardProjected, kartMovement.GroundNormal);
            Quaternion tiltRotation = Quaternion.Euler(0, tiltAngle, 0);
            targetRot = groundRotation * tiltRotation;

        }
        else
        {
            targetRot = Quaternion.LookRotation(transform.forward, Vector3.up);
        }
        float smoothTime;
        if (kartMovement.Grounded)
            smoothTime = orientationSmoothTime;
        else
            smoothTime = airOrientationSmoothTime;

        kartModelParent.rotation = Quaternion.Lerp(kartModelParent.rotation, targetRot, Time.deltaTime *smoothTime);
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