using UnityEngine;

public class IAController : MonoBehaviour
{
    private PersonajeSO data;
    private GameObject modelInstance;
    private bool canMove = false;
    public float speed = 5f;

    public void SetData(PersonajeSO so)
    {
        data = so;
        speed = data.maxSpeedMultiplier;

        // Instancia el modelo 3D del SO
        if (data.characterPrefab != null)
        {
            modelInstance = Instantiate(data.characterPrefab, transform);
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
      
    }

    public void Activate()
    {
   
    }
}
