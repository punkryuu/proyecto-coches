using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectionModeMenu : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] GameObject firtObject;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip modoCarrera;
    [SerializeField] AudioClip modoContrarreloj;
    [SerializeField] AudioSource audioSource;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firtObject);
    }
    public void RaceButton()
    {
        audioSource.PlayOneShot(modoCarrera);
        SceneManager.LoadScene(nextScene);
        GameManager.Instance.gameMode = GameMode.Race;
    }
    public void TimeTrialButton()
    {
        audioSource.PlayOneShot(modoContrarreloj);
        SceneManager.LoadScene(nextScene);
        GameManager.Instance.gameMode = GameMode.TimeTrial;

    }
}
