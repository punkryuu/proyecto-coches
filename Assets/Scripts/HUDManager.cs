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
    [SerializeField] Slider barraPoder;

    MenuUIManager menuUIManager;
    public RaceManager modoCarrera;

     float progreso = 0.2f;
    float speed = 0;

    private void Start()
    {
        menuUIManager = FindAnyObjectByType<MenuUIManager>();

        contenedorPausa.SetActive(false);
        contenedorFinalizar.SetActive(false);
        countdownText.gameObject.SetActive(false);

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
            contenedorPausa.SetActive(true);
            Time.timeScale = 0;
        }
    }
        public bool PuedeUsarPoder()
    {
        return barraPoder != null && barraPoder.value >= barraPoder.maxValue;
    }

    public void ConsumirPoder()
    {
        if (barraPoder != null)
            barraPoder.value = 0;
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
        SceneManager.LoadScene(1);
    }

    public void NextRace()
    {
        SceneManager.LoadScene(modoCarrera.raceCounter);
    }
    public float GetProgreso()
    {
        return progreso;
    }
}