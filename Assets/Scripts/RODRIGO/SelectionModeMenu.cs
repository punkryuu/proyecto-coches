using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectionModeMenu : MonoBehaviour
{
    [SerializeField] private string nextScene;
    [SerializeField] GameObject firtObject;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firtObject);
    }
    public void RaceButton()
    {
        SceneManager.LoadScene(nextScene);
        GameManager.Instance.gameMode = GameMode.Race;
    }
    public void TimeTrialButton()
    {
        SceneManager.LoadScene(nextScene);
        GameManager.Instance.gameMode = GameMode.TimeTrial;

    }
}
