using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] CarController car; 
    [SerializeField] TextMeshProUGUI textoVelocidad;
    [SerializeField] Slider barraPoder;
    float progreso = 10;   
    float speed = 0;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            barraPoder.value+= progreso;
            Debug.Log("Detectado!");
        }
        if(barraPoder.value > barraPoder.maxValue)
            barraPoder.value = barraPoder.maxValue;
    }

    private void Update()
    {
        if (car == null)
            return;
        speed = car.GetCurrentSpeed(); 
        textoVelocidad.text = $"{speed:F1} km/h";
    }
}

