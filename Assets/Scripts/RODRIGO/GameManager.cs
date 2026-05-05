using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    public GameMode gameMode;
    public PersonajeSO selectedCharacter;
    public TrackSO selectedTrack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetGameMode(GameMode mode)
    {
        gameMode = mode;
    }

    public void SetCharacter(PersonajeSO character)
    {
        selectedCharacter = character;
    }

    public void SetTrack(TrackSO track)
    {
        selectedTrack = track;
    }
    public void ResetSelections()
    {
        gameMode = GameMode.None; 
        selectedCharacter = null;
        selectedTrack = null;
    }
}

public enum GameMode {
    None,
    TimeTrial,
    Race
}