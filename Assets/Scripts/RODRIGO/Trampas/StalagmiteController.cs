using UnityEngine;
using System.Collections;

public class StalagmiteController : TrapBase {
    [SerializeField] private float fallDistance = 5f;

    [Header("Vibración")]
    [SerializeField] private float warningTime = 2f;
    [SerializeField] private float vibrationIntensity = 0.2f;
    [SerializeField] private float vibrationSpeed = 20f;

    private Vector3 bottomPos;

    protected override void InitializePositions()
    {
        startPos = transform.position;
        bottomPos = startPos - Vector3.up * fallDistance;

        targetPos = bottomPos;
    }

    protected override IEnumerator Warning()
    {
        float t = 0f;
        Vector3 basePos = transform.position;

        while (t < warningTime)
        {
            t += Time.deltaTime;

            float x = Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity;
            float z = Mathf.Cos(Time.time * vibrationSpeed) * vibrationIntensity;

            transform.position = basePos + new Vector3(x, 0f, z);

            yield return null;
        }

        transform.position = basePos;
    }

    protected override IEnumerator ActivateTrap()
    {
        isActive = true;
        col.enabled = true;

        yield return MoveTo(bottomPos);
    }

    protected override IEnumerator DeactivateTrap()
    {
        col.enabled = false;
        isActive = false;

        yield return new WaitForSeconds(2f);

        transform.position = startPos;
    }
}