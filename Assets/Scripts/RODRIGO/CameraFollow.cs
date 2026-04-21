using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.Image;

public class CameraFollow : MonoBehaviour
{
     float minFOV = 80f;
     float maxFOV = 120f;
     float fovSmooth = 5f;
    Camera cam;
    Rigidbody targetRb;
    FSMManager fsm;
    [SerializeField]Transform target;
    Vector3 offset;
    Vector3 rotationOffset;
    float rotSmooth = 10f;
    float moveSmooth = 20f;
    [SerializeField] float collisionRadius = 0.3f;
    [SerializeField] LayerMask collisionMask;
    private void Start()
    {
        cam = GetComponent<Camera>();
        if (target == null)
        {
             fsm = FindAnyObjectByType<FSMManager>();

            if (fsm != null)
            {
                target = GetComponentInParent<FSMManager>().GetHitboxTransform();
                targetRb = target.GetComponentInParent<Rigidbody>();
            }
        }

        offset = new Vector3(0, 4, -8.5f);
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
        if (targetRb != null)
        {
            float speed = targetRb.linearVelocity.magnitude;

            float maxSpeed = fsm.GetMaxSpeed();

            float t = Mathf.InverseLerp(0, maxSpeed, speed);
            float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSmooth * Time.deltaTime);
        }
    }
    
}
