using UnityEngine;
using UnityEngine.SceneManagement;

public class TextoCinematica : MonoBehaviour {
    [SerializeField] GameObject texto;
    [SerializeField] float tiempoVisible = 2f;
    [SerializeField] string nombreEscena;

    private Vector3 ultimaPosicionMouse;
    private float temporizador;

    void Start()
    {
        ultimaPosicionMouse = Input.mousePosition;

        texto.SetActive(false);
    }

    void Update()
    {
        bool actividadDetectada = false;

        // Movimiento del ratón
        if (Input.mousePosition != ultimaPosicionMouse)
        {
            actividadDetectada = true;
            ultimaPosicionMouse = Input.mousePosition;
        }

        // Movimiento de sticks del mando
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.2f || Mathf.Abs(vertical) > 0.2f)
        {
            actividadDetectada = true;
        }

        // Mostrar botón si hay actividad
        if (actividadDetectada)
        {
            texto.SetActive(true);
            temporizador = tiempoVisible;
        }

        // Temporizador para ocultarlo
        if (texto.activeSelf)
        {
            temporizador -= Time.deltaTime;

            if (temporizador <= 0)
            {
                texto.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            CambiarEscena();
        }
    }
    void CambiarEscena()
    {
        SceneManager.LoadScene(nombreEscena);
    }

}
