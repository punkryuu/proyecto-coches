using UnityEngine;

public class HitboxCollision : MonoBehaviour {
    private FSMManager fsm;

    private void Awake()
    {
        fsm = GetComponentInParent<FSMManager>();
        Debug.Log("HitboxCollision script found FSMManager: " + (fsm != null));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (fsm != null)
        {
            Debug.Log("Collision detected with: " + collision.gameObject.name);
            fsm.SetAndPlayAudioClip(3);
        }
    }
}