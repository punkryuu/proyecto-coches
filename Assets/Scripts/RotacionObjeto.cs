using UnityEngine;

public class RotacionObjeto : MonoBehaviour
{
   public Vector3 velocidadRotacion = new Vector3(0, 100, 0); 
    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime); 
    }
}
