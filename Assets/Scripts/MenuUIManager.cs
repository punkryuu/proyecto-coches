using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuUIManager: MonoBehaviour
{
    [SerializeField] private GameObject contenedorOpciones;
    [SerializeField] private GameObject panel;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider volumenGeneral, volumenMusica, volumenSFX;

    Resolution[] resolutions;
    List<string> options = new List<string>();

    bool isOptionsActive = false;
    public void Start()
    {
        contenedorOpciones.SetActive(false);
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
    public void Update()
    {
        if ((isOptionsActive && Input.GetKeyDown(KeyCode.Escape)))
        {
            panel.SetActive(true);
            contenedorOpciones.SetActive(false);
            isOptionsActive = false;
        }
    }
    public void PlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    public void OptionButton()
    {
        panel.SetActive(false);
        contenedorOpciones.SetActive(true);
        isOptionsActive = true;

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
