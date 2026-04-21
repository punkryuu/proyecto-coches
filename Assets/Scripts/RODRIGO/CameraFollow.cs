using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.Image;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]Transform target;
    Vector3 offset;
    Vector3 rotationOffset;
    float rotSmooth = 10f;
    float moveSmooth = 20f;
    [SerializeField] float collisionRadius = 0.3f;
    [SerializeField] LayerMask collisionMask;
    private void Start()
    {

        if (target == null)
        {
            FSMManager fsm = FindAnyObjectByType<FSMManager>();

            if (fsm != null)
            {
                target = GetComponentInParent<FSMManager>().GetHitboxTransform();
            }
        }

        offset = new Vector3(0, 5, -11);
        rotationOffset = new Vector3(10, 0, 0);

        if (target != null)
        {
            Vector3 desiredPosition = target.position + target.rotation * offset;
            transform.position = desiredPosition;
        }
    

}


    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.rotation * offset;

        Vector3 direction = (desiredPosition - target.position).normalized;
        float distance = Vector3.Distance(target.position, desiredPosition);

        RaycastHit hit;

        if (Physics.SphereCast(target.position, collisionRadius, direction, out hit, distance, collisionMask))
        {
            desiredPosition = target.position + direction * hit.distance;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, moveSmooth * Time.deltaTime);

        Quaternion baseRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        Quaternion desiredRotation = baseRotation * Quaternion.Euler(rotationOffset);

        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotSmooth * Time.deltaTime);
        Debug.DrawLine(target.position , desiredPosition, Color.red);
    }
    
}
