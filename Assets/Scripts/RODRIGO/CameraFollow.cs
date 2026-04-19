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
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + target.rotation * offset; 

 
        Quaternion baseRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        Quaternion desiredRotation = baseRotation * Quaternion.Euler(rotationOffset);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, moveSmooth * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotSmooth * Time.deltaTime );
    }
}
