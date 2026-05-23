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

    public void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();
        if (fsm == null) return;
        if (fsm.isCurrentlyStunned) return; 

        fsm.stunned = true;
        fsm.triggerStunDuration = stunDuration;
    }


}
