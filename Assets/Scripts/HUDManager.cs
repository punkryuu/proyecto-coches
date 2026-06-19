using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    [SerializeField] FSMManager car;
    [SerializeField] TextMeshProUGUI textoVelocidad;
    [SerializeField] GameObject contenedorPausa;
    [SerializeField] public GameObject contenedorFinalizar;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip sonidoPausa;
    [SerializeField] AudioClip sonidoCarga;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Slider barraPoder;
    private bool sonidoCargaActivo = false;
    MenuUIManager menuUIManager;

     float progreso = 0.2f;
    float speed = 0;

    private void Start()
    {
        menuUIManager = FindAnyObjectByType<MenuUIManager>();

        contenedorPausa.SetActive(false);
        contenedorFinalizar.SetActive(false);
        //countdownText.gameObject.SetActive(false);

        if (barraPoder != null)
            barraPoder.value = 0;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            audioSource.PlayOneShot(sonidoPausa);
            contenedorPausa.SetActive(true);
            Time.timeScale = 0;
            
        }
    }
        public bool PuedeUsarPoder()
    {
        if (barraPoder == null)
            return false;

        bool estaLlena = barraPoder.value >= barraPoder.maxValue;

        if (estaLlena && !sonidoCargaActivo)
        {
            // Activar loop del sonido de carga
            audioSource.clip = sonidoCarga;
            audioSource.loop = true;
            audioSource.Play();
            sonidoCargaActivo = true;
        }
        else if (!estaLlena && sonidoCargaActivo)
        {
            // Detener el loop si deja de estar llena
            audioSource.loop = false;
            audioSource.Stop();
            sonidoCargaActivo = false;
        }

        return estaLlena;
    }

    public void ConsumirPoder()
    {
        if (barraPoder != null)
            barraPoder.value = 0;
    }

    public IEnumerator Delay()
    {
        //Time.timeScale = 0;

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
        audioSource.PlayOneShot(sonidoPausa);
        SceneManager.LoadScene(1);
    }


    public float GetProgreso()
    {
        return progreso;
    }
}