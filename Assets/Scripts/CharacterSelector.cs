using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
public class CharacterSelector : MonoBehaviour
{
    [SerializeField] Image selectedImage;
    [SerializeField] List<PersonajeSO> personajes = new List<PersonajeSO>();
    GameObject selectedChar;
    [SerializeField] Button continueButton;
    [SerializeField] Button historyButton;
    [SerializeField] GameObject panelHistoria;
    [SerializeField] TMP_Text nombre;
    [SerializeField] TMP_Text historia;
    [SerializeField] private Transform visualSpawn;

    private GameObject visualActual;

    [SerializeField] private string nextScene;
    public void Start()
    {
        continueButton.interactable = false;
        historyButton.interactable = false;
        visualSpawn.gameObject.SetActive(false);

    }
    public void SelectedCharacter(int buttonIndex)
    {
        selectedChar = personajes[buttonIndex].characterPrefab;

        //selectedImage.sprite = personajes[buttonIndex].selectionImage;

        // Destruir visual anterior
        if (visualActual != null)
        {
            Destroy(visualActual);
        }

        // Crear nuevo visual
        visualActual = Instantiate( personajes[buttonIndex].visual,visualSpawn.position,visualSpawn.rotation,visualSpawn);

        continueButton.interactable = true;
        historyButton.interactable = true;
        visualSpawn.gameObject.SetActive(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCharacter(personajes[buttonIndex]);
            nombre.text = personajes[buttonIndex].name;
            historia.text = personajes[buttonIndex].historia;
        }
        else {
            Debug.LogError("GAMEMANAGER INSTANCE IS NULL. Make sure a GameManager object exists in the scene.");
        }
    }

    public void Continue()
    {         
        if (selectedChar != null)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
