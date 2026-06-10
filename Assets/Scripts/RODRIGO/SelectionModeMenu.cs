using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionModeMenu : MonoBehaviour
{
    [SerializeField] private string timetrialScene;
    [SerializeField] private string raceModeScene;

    public void RaceButton()
    {
        SceneManager.LoadScene(raceModeScene);
        GameManager.Instance.gameMode = GameMode.Race;
    }
    public void TimeTrialButton()
    {
        SceneManager.LoadScene(timetrialScene);
        GameManager.Instance.gameMode = GameMode.TimeTrial;

    }
}
