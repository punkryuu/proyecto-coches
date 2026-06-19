using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class FinishLineCollision : MonoBehaviour 
{
    [SerializeField] private RaceManager raceController;
    [SerializeField] TMP_Text lapCounterText;
    [SerializeField] private bool isTimeTrialMode = false;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip warningSound;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip finalLapMusic;
    public TrackCheck trackCheck;
    [SerializeField] private WayPointsCircuit wayPointsCircuit;
    void OnTriggerEnter(Collider other)
    {
        // --- Modo contrarreloj ---
        if (isTimeTrialMode)
        {
            PlayerCar carIdetifier = other.GetComponentInParent<PlayerCar>();
            if (carIdetifier == null || !carIdetifier.isPlayer) return;

            if (carIdetifier.currentWayPoint >= carIdetifier.totalWaypoints)
            {
                // Avisar al TimeTrialMode
                TimeTrialMode.Instance.OnPlayerLapCompleted();

                // Actualizar UI de vueltas 
                if (lapCounterText != null)
                {
                    lapCounterText.text = (TimeTrialMode.Instance.playerLapCounter).ToString();
                }

                // Reiniciar waypoints 
                carIdetifier.currentWayPoint = 0;
            }
            return; 
        }

        // --- Modo carrera normal
        if (raceController.playerLapCounter < raceController.totalLaps)
        {;
            PlayerCar carIdetifier = other.GetComponentInParent<PlayerCar>();
            if (carIdetifier == null) return;
            Debug.Log("wayPointsCircuit = " + wayPointsCircuit);


            if (carIdetifier.currentWayPoint >= wayPointsCircuit.GetWayPointsCount())
            {
                if (carIdetifier.isPlayer)
                {
                    raceController.playerLapCounter++;
                    carIdetifier.currentLap++;
                    if (carIdetifier.currentLap == raceController.totalLaps - 1)
                    {
                        StartCoroutine(ChangeMusicSequence());
                    }
                    lapCounterText.text = raceController.playerLapCounter.ToString();
                    raceController.FinishedRace();
                    carIdetifier.currentWayPoint = 0;
                    
                    return;
                }
                IASINAPRENDIZAJE ia = other.GetComponentInParent<IASINAPRENDIZAJE>();
                GameObject npcRoot = ia.gameObject;
                if (raceController.npcLapCounter.ContainsKey(npcRoot))
                {
                    raceController.IncrementNPCLapCounter(ia.gameObject);
                    ia.IaPlayerCar.currentWayPoint = 0;
                }
               
            }
            else
            {
                Debug.Log("Player has not completed all waypoints yet.");
            }
        }
    }

    private IEnumerator ChangeMusicSequence()
    {
        sfxSource.PlayOneShot(warningSound);
        yield return new WaitForSeconds(warningSound.length);
        musicSource.clip = finalLapMusic;
        musicSource.Play();
    }
}