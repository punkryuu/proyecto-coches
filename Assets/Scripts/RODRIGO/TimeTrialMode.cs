using System.Collections;
using System.Collections.Generic; // <-- Añadir
using TMPro;
using UnityEngine;

public class TimeTrialMode : MonoBehaviour {
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
    float bestLapTime = float.MaxValue;
    [SerializeField] TMP_Text bestLapTimeText;
    [SerializeField] private TMP_Text[] finalLapTimeTexts; 
    [SerializeField] private TMP_Text finalTotalTimeText; 
    [SerializeField] private TMP_Text finalBestLapText;
    [SerializeField] private GameObject interfaz;
    public PlayerCar player;

    float currentLapStartTime;
    public int playerLapCounter = 0;

    bool raceActive;
    int totalLaps = 3;

    [SerializeField] UIManager ui;
    [SerializeField] TMP_Text countdownText;

    private List<float> lapTimes = new List<float>();



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
        lapTimes.Clear(); // Reiniciar lista
    }

    void Update()
    {
        if (raceActive)
        {
            raceTimer += Time.deltaTime;
            raceTimerText.text = FormatTime(raceTimer);
        }
    }

    public void OnPlayerLapCompleted()
    {
        float lapTime = raceTimer - currentLapStartTime;
        lapTimes.Add(lapTime); // Guardar tiempo

        if (lapTime < bestLapTime)
        {
            bestLapTime = lapTime;
            // GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.selectedCharacter.audios[5]);
        }
        // else GetComponent<AudioSource>().PlayOneShot(GameManager.Instance.selectedCharacter.audios[1]);

        lastLapTime = lapTime;
        currentLapStartTime = raceTimer;
        playerLapCounter++;

        lastLapTimeText.text = FormatTime(lastLapTime);
        bestLapTimeText.text = FormatTime(bestLapTime);

        if (playerLapCounter >= totalLaps)
        {
            FinishTrial();
        }
    }

    void FinishTrial()
    {
        interfaz.SetActive(false);
        raceActive = false;
        ui.contenedorFinalizar.SetActive(true);

        int lapsToShow = Mathf.Min(lapTimes.Count, finalLapTimeTexts.Length);

        for (int i = 0; i < lapsToShow; i++)
        {
            if (finalLapTimeTexts[i] != null)
                finalLapTimeTexts[i].text = FormatTime(lapTimes[i]);
        }

        for (int i = lapsToShow; i < finalLapTimeTexts.Length; i++)
        {
            if (finalLapTimeTexts[i] != null)
                finalLapTimeTexts[i].text = "---";
        }

        if (finalTotalTimeText != null)
            finalTotalTimeText.text = "Tiempo total: " + FormatTime(raceTimer);

        if (finalBestLapText != null)
            finalBestLapText.text = "Mejor vuelta: " + FormatTime(bestLapTime);
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        player.canMove = true;

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