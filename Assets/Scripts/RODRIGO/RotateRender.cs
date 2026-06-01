using UnityEngine;
using UnityEngine.Rendering;

public class RotateRender : MonoBehaviour {
    [SerializeField] float rotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
