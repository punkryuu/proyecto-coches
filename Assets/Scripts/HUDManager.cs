using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    [SerializeField] CarController car; 
    [SerializeField] TextMeshProUGUI textoVelocidad;
    [SerializeField] GameObject contenedorPausa;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] AudioMixer audioMixer;
    MenuUIManager menuUIManager;

    public float progreso = 10;   //prueba, cambiar el public a un get set
    float speed = 0;
   

    private void Start()
    {
        menuUIManager = FindObjectOfType<MenuUIManager>();
        contenedorPausa.SetActive(false);
        countdownText.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (car == null)
            return;
        speed = car.GetCurrentSpeed(); 
        textoVelocidad.text = $"{speed:F1} km/h";
        PauseMenu();
    }

    private void PauseMenu()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
          contenedorPausa.SetActive(true);
            Time.timeScale = 0;
        }   
    }
    public IEnumerator Delay()
    {
        contenedorPausa.SetActive(false);
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;
    }


    public void ResumeButton()
    {
        StartCoroutine(Delay());
    }
    public void MainMenuButton()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1); 
    }
   
}

