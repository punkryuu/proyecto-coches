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

    public void Start()
    {
        continueButton.interactable = false;
    }
    public void SelectedCharacter(int buttonIndex)
    {
        selectedChar = personajes[buttonIndex].characterPrefab;
        selectedImage.sprite = personajes[buttonIndex].selectionImage;
        continueButton.interactable = true;
    }

    public void Continue()
    {         
        if (selectedChar != null)
        {
            SceneManager.LoadScene(0);
        }
    }
}
