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
    [SerializeField] GameObject contenedorOpciones;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Slider volumenGeneral, volumenMusica, volumenSFX;

    [SerializeField] AudioMixer audioMixer;

    public float progreso = 10;   //prueba, cambiar el public a un get set
    float speed = 0;
    Resolution[] resolutions;
    List<string> options = new List<string>();

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        contenedorPausa.SetActive(false);
        contenedorOpciones.SetActive(false);
        countdownText.gameObject.SetActive(false);
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();//limpiar el dropdown para llenarlo con las resoluciones disponibles
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)//Si la resolución actual del juego coincide con la resolución en el array, guardamos ese índice para mostrarlo como opción seleccionada en el dropdown
            {                 
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);//Solo toma strings como opciones, por eso el for
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();//Para realmente mostrar la resolución actual en el dropdown
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
          contenedorOpciones.SetActive(false);
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
    public void PlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); 
    }

    public void OptionButton()
    {    
        contenedorPausa.SetActive(false);
        contenedorOpciones.SetActive(true);
    }
    public void SetQuality(int qualityIndex)
    {
       QualitySettings.SetQualityLevel(qualityIndex);
       
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ChangeGeneralVolume()
    {
               audioMixer.SetFloat("GeneralVolume", volumenGeneral.value);
    }

    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("MusicaVolume", volumenMusica.value);
    }

    public void ChangeSFXVolume() 
    {
        audioMixer.SetFloat("SFXVolume", volumenSFX.value);
    }
}

