using UnityEngine;
using System.Collections;
public class FloatTurboTrigger : MonoBehaviour
{
    [SerializeField] private float boostDuration = 2f;

    [Range(0f, 1f)]
    [SerializeField] private float gravityMultiplier = 0.6f;

    [SerializeField] private float gravityDuration = 1.5f;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();
        NPCAgent npc = other.GetComponentInParent<NPCAgent>();

        if (fsm == null && npc == null) return;
        
        if (fsm != null)
        {
            fsm.triggerBoost = true;
            fsm.triggerBoostDuration = boostDuration;

        StartCoroutine(ApplyGravityEffect(fsm));
        }
        if (npc != null)
        {
            npc.triggerBoost = true;
            npc.triggerBoostDuration = boostDuration;
            StartCoroutine(ApplyGravityEffectNPC(npc));
        }

        StartCoroutine(DisableTemporarily());
    }

    private IEnumerator ApplyGravityEffect(FSMManager fsm)
    {
        float original = fsm.gravityMultiplier;

        fsm.gravityMultiplier = gravityMultiplier;

        yield return new WaitForSeconds(gravityDuration);

        fsm.gravityMultiplier = original;
    }

    private IEnumerator ApplyGravityEffectNPC(NPCAgent npc)
    {
        float original = npc.gravityMultiplier;
        npc.gravityMultiplier = gravityMultiplier;

        yield return new WaitForSeconds(gravityDuration);

        npc.gravityMultiplier = original;
    }

    private IEnumerator DisableTemporarily()
    {
        col.enabled = false;

        yield return new WaitForSeconds(0.2f);

        col.enabled = true;
    }


}
