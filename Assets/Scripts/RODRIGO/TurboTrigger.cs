using UnityEngine;

public class TurboTrigger : MonoBehaviour
{

    [SerializeField] private float boostDuration = .5f;

    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();

        if (fsm == null) return;

        fsm.triggerBoost = true;
        fsm.triggerBoostDuration = boostDuration;
    }
}
