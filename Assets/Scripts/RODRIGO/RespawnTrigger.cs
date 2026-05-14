using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();
        if (fsm != null)
        {
            fsm.Respawn();
        }
    }
}
