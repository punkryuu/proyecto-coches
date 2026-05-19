using UnityEngine;
using System.Collections;

public class SpikeTrapController : TrapBase {
    [SerializeField] private float spikeHeight = 2f;

    private Vector3 hiddenPos;
    private Vector3 upPos;

    protected override void InitializePositions()
    {
        upPos = transform.position;
        hiddenPos = upPos - Vector3.up * spikeHeight;

        transform.position = hiddenPos;

        col.enabled = false;
    }

    protected override IEnumerator Warning()
    {
        Vector3 warningPos = hiddenPos + Vector3.up * 0.4f;

        yield return MoveTo(warningPos);

        yield return new WaitForSeconds(0.3f);

        yield return MoveTo(hiddenPos);

        yield return new WaitForSeconds(0.2f);
    }

    protected override IEnumerator ActivateTrap()
    {
        yield return MoveTo(upPos);

        isActive = true;
        col.enabled = true;

    }

    protected override IEnumerator DeactivateTrap()
    {

        col.enabled = false;
        isActive = false;
        yield return MoveTo(hiddenPos);

    }
}