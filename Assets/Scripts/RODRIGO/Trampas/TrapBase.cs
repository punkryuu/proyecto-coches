using UnityEngine;
using System.Collections;

public abstract class TrapBase : MonoBehaviour {
    [Header("Tiempos")]
    [SerializeField] protected float minWaitTime = 1f;
    [SerializeField] protected float maxWaitTime = 3f;

    [SerializeField] protected float activeTime = 2f;

    [Header("Movimiento")]
    [SerializeField] protected float moveSpeed = 10f;

    protected Collider col;

    protected Vector3 startPos;
    protected Vector3 targetPos;

    protected bool isActive;

    protected virtual void Start()
    {
        col = GetComponent<Collider>();

        InitializePositions();

        StartCoroutine(Cycle());
    }

    protected virtual IEnumerator Cycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            yield return StartCoroutine(Warning());

            yield return StartCoroutine(ActivateTrap());

            yield return new WaitForSeconds(activeTime);

            yield return StartCoroutine(DeactivateTrap());
        }
    }

    protected IEnumerator MoveTo(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    protected abstract void InitializePositions();

    protected abstract IEnumerator Warning();

    protected abstract IEnumerator ActivateTrap();

    protected abstract IEnumerator DeactivateTrap();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        Debug.Log("Jugador golpeado");
    }
}