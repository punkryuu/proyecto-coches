using UnityEngine;
using System.Collections;

public class StalagmiteController : MonoBehaviour {
    [Header("Tiempos")]
     private float minWaitTime = 1f;
     private float maxWaitTime = 4f;

    private float minDownTime = 1f;
    private float maxDownTime = 3f;

    private float minHiddenTime = 2f;
    private float maxHiddenTime = 5f;

    [Header("Caída")]
    private float fallSpeed = 40f;
    [SerializeField] private float fallDistance = 5f;
    [Header("Aviso")]
    private float warningTime = 3f;
    private float vibrationIntensity = 0.5f;
    private float vibrationSpeed = 25f;
    [Header("Estado")]
    private Vector3 startPos;
    private Vector3 bottomPos;

    private Collider col;
    private bool isFalling;

    private void Start()
    {
        startPos = transform.position;
        bottomPos = startPos - Vector3.up * fallDistance;

        col = GetComponent<Collider>();

        StartCoroutine(Cycle());
    }

    private IEnumerator Cycle()
    {
        while (true)
        {
            // 1. esperar arriba
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            //2.vibracion
            yield return StartCoroutine(Warning());
            // 3. caer
            yield return StartCoroutine(Fall());

            // 4. quedarse abajo
            yield return new WaitForSeconds(Random.Range(minDownTime, maxDownTime));

            // 5. desaparecer
            SetVisible(false);

            yield return new WaitForSeconds(Random.Range(minHiddenTime, maxHiddenTime));

            // 6. volver arriba
            transform.position = startPos;
            SetVisible(true);
        }
    }
    private IEnumerator Warning()
    {
        float t = 0f;
        Vector3 basePos = transform.position;

        while (t < warningTime)
        {
            t += Time.deltaTime;

            float offsetX = Mathf.Sin(Time.time * vibrationSpeed) * vibrationIntensity;
            float offsetZ = Mathf.Cos(Time.time * vibrationSpeed) * vibrationIntensity;

            transform.position = basePos + new Vector3(offsetX, 0f, offsetZ);

            yield return null;
        }

        transform.position = basePos;
    }
    private IEnumerator Fall()
    {
        isFalling = true;
        col.enabled = true;

        while (Vector3.Distance(transform.position, bottomPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                bottomPos,
                fallSpeed * Time.deltaTime
            );

            yield return null;
        }

        col.enabled = false; 
        isFalling = false;
    }

    private void SetVisible(bool visible)
    {
        col.enabled = visible;

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = visible;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFalling) return;

        col.enabled = false;
    }
}