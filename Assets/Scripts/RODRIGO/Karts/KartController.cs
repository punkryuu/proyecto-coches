
using System.Collections.Generic;
using UnityEngine;

/// inicializa el coche a partir de un SO
public class KartController : MonoBehaviour {
    //referencias
    [SerializeField] PersonajeSO personajeSO;//luego se quita el serialize es para probar\
    [SerializeField] private KartInput kartInput;
    [SerializeField] private KartMovement kartMovement;
    [SerializeField] private KartDrift kartDrift;
    [SerializeField] private KartVisual kartVisual;

    private void Awake()
    {
        if (kartInput == null) kartInput = GetComponent<KartInput>();
        if (kartMovement == null) kartMovement = GetComponent<KartMovement>();
        if (kartDrift == null) kartDrift = GetComponent<KartDrift>();
        if (kartVisual == null) kartVisual = GetComponent<KartVisual>();
    }

    private void Start()
    {
        if (personajeSO == null)
        {
            Debug.LogError("SO no asignado");
            return;

        }
        if (kartMovement != null)
        {
            kartMovement.Initialize(personajeSO);
        }
        if (kartDrift != null)
        {
            kartDrift.Initialize(personajeSO);
        }
        Transform visualContainer = transform.Find("VisualContainer");
        if (visualContainer == null)
        {
            visualContainer = new GameObject("VisualContainer").transform;
            visualContainer.SetParent(transform);
            visualContainer.localPosition = Vector3.zero;
            visualContainer.localRotation = Quaternion.identity;
        }

        if (personajeSO.visual != null)
        {
            GameObject visualInstance = Instantiate(personajeSO.visual, visualContainer);
            visualInstance.transform.localPosition = Vector3.zero;

            if (kartVisual != null)
            {
                kartVisual.SetModel(visualInstance.transform, visualContainer);

                // Buscar partículas
                Transform driftParticles = visualInstance.transform.Find(personajeSO.driftParticlesPath);
                Transform turboParticles = visualInstance.transform.Find(personajeSO.turboParticlesPath);

                List<ParticleSystem> driftList = new List<ParticleSystem>();
                List<ParticleSystem> turboList = new List<ParticleSystem>();

                if (driftParticles != null)
                {
                    for (int i = 0; i < driftParticles.GetChild(0).childCount; i++)
                        driftList.Add(driftParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
                    for (int i = 0; i < driftParticles.GetChild(1).childCount; i++)
                        driftList.Add(driftParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
                }

                if (turboParticles != null)
                {
                    for (int i = 0; i < turboParticles.GetChild(0).childCount; i++)
                        turboList.Add(turboParticles.GetChild(0).GetChild(i).GetComponent<ParticleSystem>());
                    for (int i = 0; i < turboParticles.GetChild(1).childCount; i++)
                        turboList.Add(turboParticles.GetChild(1).GetChild(i).GetComponent<ParticleSystem>());
                }

                kartVisual.SetDriftParticles(driftList);
                kartVisual.SetTurboParticles(turboList);
            }
        }
        else
        {
            Debug.LogError("no hay modelo visual ");
        }
    }
}



