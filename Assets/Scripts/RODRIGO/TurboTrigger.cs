using System.Collections;
using UnityEngine;

public class TurboTrigger : MonoBehaviour {
    [SerializeField] private float boostDuration = .5f;

    [Header("Cooldown")]
    [SerializeField] private float disableTime = 0.2f;

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();

        if (fsm == null) return;

        fsm.triggerBoost = true;
        fsm.triggerBoostDuration = boostDuration;

        StartCoroutine(DisableTemporarily());
    }

    private IEnumerator DisableTemporarily()
    {
        triggerCollider.enabled = false;

        yield return new WaitForSeconds(disableTime);

        triggerCollider.enabled = true;
    }
}