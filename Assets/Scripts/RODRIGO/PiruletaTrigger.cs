using System.Collections;
using UnityEngine;

public class PiruletaTrigger : MonoBehaviour {
    [Header("Movimiento")]
    private float forwardSpeed = 50f; 
    private float returnSpeed = 75f;
    private float timeBeforeReturn = 2f;
    private float destroyTime = 2f;

    [Header("Ataque")]
    private float stunDuration = 1.5f;

    [Header("Visual")]
    [SerializeField] private float spinSpeed = 720f; 

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
    }

    private IEnumerator PiruletaRoutine()
    {
        if (owner == null) yield break;

        FSMManager ownerFSM = owner.GetComponent<FSMManager>();
        if (ownerFSM == null) yield break;

        float playerMinSpeed = ownerFSM.GetCurrentSpeed() * 1.2f;
        float launchSpeed = Mathf.Max(forwardSpeed, playerMinSpeed);

        rb.linearVelocity = ownerFSM.GetHitboxTransform().forward * launchSpeed;

        yield return new WaitForSeconds(timeBeforeReturn);

        returning = true;

        yield return new WaitForSeconds(destroyTime);

        Destroy(gameObject);
    }

    private void Update()
    {
        // Giro constante sobre sí misma
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime, Space.Self);
    }

    private void FixedUpdate()
    {
        if (!returning || owner == null) return;

        FSMManager ownerFSM = owner.GetComponent<FSMManager>();
        if (ownerFSM == null) return;

        Vector3 dir = (ownerFSM.GetHitboxTransform().position - transform.position).normalized;

        rb.linearVelocity = dir * returnSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (PerteneceAlDueño(other)) return;

        StunTarget(other);
        Destroy(gameObject);
    }

    private bool PerteneceAlDueño(Collider other)
    {
        if (owner == null) return false;

        return other.transform.IsChildOf(owner.transform) ||
               other.gameObject == owner;
    }

    private void StunTarget(Collider other)
    {
        IASINAPRENDIZAJE ia = other.GetComponentInParent<IASINAPRENDIZAJE>();
        if (ia != null) 
         StartCoroutine(StunRoutineIA(ia));
        
            FSMManager fsm = other.GetComponentInParent<FSMManager>();
        if (fsm == null) return;
        if (fsm.isCurrentlyStunned) return;

        fsm.stunned = true;
        fsm.triggerStunDuration = stunDuration;
    }
    private IEnumerator StunRoutineIA(IASINAPRENDIZAJE ia)
    {
        Debug.Log("StunRoutineIA started for " + ia.name);
        ia.StartStun();

        float t = 0f;
        while (t < stunDuration)
        {
            ia.StayStunned();
            t += Time.deltaTime;
            yield return null;
        }

        ia.StartRestoreRotation();
    }
}