using System.Collections;
using TMPro;
using UnityEngine;

public class TimeTrialMode : MonoBehaviour
{
    public static TimeTrialMode Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    float raceTimer;
    [SerializeField] TMP_Text raceTimerText;
    float lastLapTime;
    [SerializeField] TMP_Text lastLapTimeText;
    float bestLapTime =float.MaxValue;
    [SerializeField] TMP_Text bestLapTimeText;

    float currentLapStartTime;
    public int playerLapCounter = 0;

    bool raceActive;

    int totalLaps = 3;

    [SerializeField] UIManager ui;
    [SerializeField] TMP_Text countdownText;

private void Start()
{
    raceTimerText.text = FormatTime(0f);
    lastLapTimeText.text = FormatTime(0f);
    bestLapTimeText.text = FormatTime(0f);

    StartCoroutine(StartCountdown());
}
    
    private void StartRace()
    {
        raceTimer = 0f;
        currentLapStartTime = 0f;
        raceActive = true;
    }
    void Update()
    {
        if (raceActive)
        {
            raceTimer += Time.deltaTime;
            raceTimerText.text = FormatTime(raceTimer);
        }
    }
    public void OnPlayerLapCompleted() // calcula el tiempo de la vuelta, lo compara con el mejor tiempo, actualiza el contador de vueltas y verifica si se ha completado la carrera
    {
            float lapTime = raceTimer - currentLapStartTime;

        if (lapTime < bestLapTime)
        {
            bestLapTime = lapTime;
            //GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.selectedCharacter.audios[5]);

        }
        //else GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.selectedCharacter.audios[1]);


        lastLapTime = lapTime;
        currentLapStartTime = raceTimer;
        playerLapCounter++;

        lastLapTimeText.text = FormatTime(lastLapTime);
        bestLapTimeText.text = FormatTime(bestLapTime);

        if (playerLapCounter >= totalLaps)
            FinishTrial();
    }

    void FinishTrial()//detiene la carrera, muestra los resultados
    { 
        raceActive = false;
        //mostresultados, etc
    }
    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        countdownText.gameObject.SetActive(false);
        StartRace();
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

        return string.Format("{0:00}:{1:00}:{2:000}",
            minutes,
            seconds,
            milliseconds);
    }
}
