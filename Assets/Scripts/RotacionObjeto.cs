using UnityEngine;
using UnityEngine.UI;

public class RotacionObjeto : MonoBehaviour
{
    [SerializeField] Slider barraPoder;
    [SerializeField] UIManager uiManager;
    public Vector3 velocidadRotacion = new Vector3(0, 100, 0); 
    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime); 
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            barraPoder.value += uiManager.progreso;
            Debug.Log("Detectado!");
        }
        if (barraPoder.value > barraPoder.maxValue)
            barraPoder.value = barraPoder.maxValue;
    }
}
