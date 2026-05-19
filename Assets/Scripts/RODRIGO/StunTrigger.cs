using UnityEngine;

public class StunTrigger : MonoBehaviour
{
    [SerializeField] private float stunDuration;

    [Header("Cooldown")]

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();

        if (fsm == null) return;

        fsm.stunned = true;
        fsm.triggerStunDuration = stunDuration;

    }


}
