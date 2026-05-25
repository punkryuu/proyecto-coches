using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();
        NPCAgent npc = other.GetComponentInParent<NPCAgent>();
        if (fsm != null)
        {
            fsm.Respawn();
        }

        if (npc != null)
        {
            npc.Respawn();
        }
    }
}
