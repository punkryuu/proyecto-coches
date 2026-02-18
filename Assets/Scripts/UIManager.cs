using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] CarController car; 
    [SerializeField] TextMeshProUGUI textoVelocidad;

    public float progreso = 10;   //prueba, cambiar el public a un get set
    float speed = 0;

    

    private void Update()
    {
        if (car == null)
            return;
        speed = car.GetCurrentSpeed(); 
        textoVelocidad.text = $"{speed:F1} km/h";
    }
}

