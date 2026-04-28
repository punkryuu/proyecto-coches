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
        StartCoroutine(StartCountdown());

    }
    private void StartRace() //reinicia el timer, activa la carrera y muestra la cuenta regresiva antes de empezar
    {
        raceTimer = 0f;
        raceActive = true;

    }
    private void Update() 
    {
        if (raceActive) raceTimer += Time.deltaTime;
    }
    public void OnPlayerLapCompleted() // calcula el tiempo de la vuelta, lo compara con el mejor tiempo, actualiza el contador de vueltas y verifica si se ha completado la carrera
    {
        float lapTime = raceTimer - currentLapStartTime;
        if (lapTime < bestLapTime) bestLapTime = lapTime;
        lastLapTime = lapTime;
        currentLapStartTime = raceTimer;
        playerLapCounter++;
        if (playerLapCounter >= totalLaps) FinishTrial();
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
}
