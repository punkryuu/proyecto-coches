using UnityEngine;
using UnityEngine.Rendering;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]Transform target;
    Vector3 offset;
    Vector3 rotationOffset;
    float rotSmooth = 10f;
    float moveSmooth = 20f;
    private void Start()
    {
        offset = new Vector3(0,5,-11);
        rotationOffset= new Vector3 (10,0,0);

    }
    private void FixedUpdate()
    {
        if (target == null) return;

        transform.position = target.position + target.rotation * offset;
        Vector3 forward = target.forward;
        Quaternion baseRotation = Quaternion.LookRotation(forward, Vector3.up);

        transform.rotation = baseRotation * Quaternion.Euler(rotationOffset);
    }
}
