using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class SpikeTrapController : TrapBase {
    [SerializeField] private float spikeHeight = 2f;

    private Vector3 hiddenPos;
    private Vector3 upPos;
    [SerializeField]private DecalProjector[] decals;

    protected override void InitializePositions()
    {
        upPos = transform.position;
        hiddenPos = upPos - Vector3.up * spikeHeight;

        transform.position = hiddenPos;

        col.enabled = false;
    }

    protected override IEnumerator Warning()
    {
        Vector3 warningPos = hiddenPos + Vector3.up * 5f;

        yield return MoveTo(warningPos);
        ActivateDeactivateDecals(false);
        yield return new WaitForSeconds(0.3f);
        yield return MoveTo(hiddenPos);
        ActivateDeactivateDecals(true);
        yield return new WaitForSeconds(0.2f);

    }

    protected override IEnumerator ActivateTrap()
    {
        ActivateDeactivateDecals(false);

        yield return MoveTo(upPos);
        
        isActive = true;
        col.enabled = true;

    }

    protected override IEnumerator DeactivateTrap()
    {
        yield return MoveTo(upPos);
         col.enabled = false;
        isActive = false;
        ActivateDeactivateDecals(true);

        yield return MoveTo(hiddenPos);

    }
    void ActivateDeactivateDecals(bool state) 
    {
        foreach (var decal in decals)
        {
            decal.enabled = state;
        }
    }
}