using UnityEngine;
using System.Collections;
public class FloatTurboTrigger : MonoBehaviour
{
    [SerializeField] private float boostDuration = 2f;

    [Range(0f, 1f)]
    [SerializeField] private float gravityMultiplier = 0.5f;

    [SerializeField] private float gravityDuration = 1.5f;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entró: " + other.name);


        IASINAPRENDIZAJE racer = other.GetComponentInParent<IASINAPRENDIZAJE>();
        if (racer != null) 
            {
            Debug.Log("IA encontrado");
            //racer.triggerBoost = true;
           // racer.triggerBoostDuration = boostDuration;
            StartCoroutine(ApplyGravityEffectNPC(racer));
            StartCoroutine(DisableTemporarily());
            return;
        }

        NPCAgent npc = other.GetComponentInParent<NPCAgent>();

        if (npc != null)
        {
            Debug.Log("NPC encontrado");
            npc.triggerBoost = true;
            npc.triggerBoostDuration = boostDuration;

            StartCoroutine(ApplyGravityEffectNPC(npc));
            StartCoroutine(DisableTemporarily());
        }
        FSMManager fsm = other.GetComponentInParent<FSMManager>();
        if (fsm != null)
        {
            Debug.Log("FSM encontrado");
            fsm.triggerBoost = true;
            fsm.triggerBoostDuration = boostDuration;

            StartCoroutine(ApplyGravityEffect(fsm));
            StartCoroutine(DisableTemporarily());
            return;
        }

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

    private IEnumerator ApplyGravityEffectNPC(IASINAPRENDIZAJE npc)
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
