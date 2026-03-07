using UnityEngine;
using UnityEngine.UI;

public class GritoBruja : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] Slider barraPoder;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && (barraPoder.value == barraPoder.maxValue))
        {
            gritoEliminadorBarra();
        }
    }
    void gritoEliminadorBarra()
    {
       foreach (GameObject enemigo in GameObject.FindGameObjectsWithTag("Enemigo"))
        {
            if (enemigo == gameObject) continue; // Ignora el propio jugador
            Slider slider = enemigo.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                slider.value *= 0.75f; // Elimina toda la barra de poder del enemigo
            }
        }
        barraPoder.value = 0f;
    }
}
