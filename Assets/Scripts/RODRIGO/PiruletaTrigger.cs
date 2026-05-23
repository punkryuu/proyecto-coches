using System.Collections;
using UnityEngine;

public class PiruletaTrigger : MonoBehaviour {
    private float speed = 200f;
     private float returnSpeed = 250f;

    private float timeBeforeReturn = 2f;
     private float destroyTime = 2f;

    private Rigidbody rb;
    private GameObject owner;
    private bool returning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(PiruletaRoutine());
    }

    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
        Debug.LogWarning(owner);

    }

    private IEnumerator PiruletaRoutine()
    {
        if (owner == null) yield break;

        FSMManager ownerFSM = owner.GetComponent<FSMManager>();
        if (ownerFSM == null) yield break;

        // Avanza hacia delante
        rb.linearVelocity = ownerFSM.GetHitboxTransform().forward * ownerFSM.GetCurrentSpeed()*1.5f;

        // Espera antes de volver
        yield return new WaitForSeconds(timeBeforeReturn);

        // Vuelve al dueño
        returning = true;

        // Después de empezar a volver, se destruye al cabo de destroyTime
        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (!returning || owner == null) return;

        FSMManager ownerFSM = owner.GetComponent<FSMManager>();
        //if (ownerFSM == null) return;

        Vector3 dir = (ownerFSM.GetHitboxTransform().position - transform.position).normalized;
        transform.forward = dir;

        // Usamos la velocidad de retorno definida
        rb.linearVelocity = dir * returnSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignoramos triggers
        if (collision.collider.isTrigger) return;

        // No nos destruimos al chocar con el dueño
        if (collision.gameObject == owner) return;

        // Cualquier otra colisión sólida → destrucción inmediata
        Destroy(gameObject);
    }
}