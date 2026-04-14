using UnityEngine;

public class TrickTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        FSMManager fsm = other.GetComponentInParent<FSMManager>();
        if (fsm != null)
        {
            fsm.canTrick = true;
            Debug.Log("¡Puedes hacer un truco!");
        }
    }
}
