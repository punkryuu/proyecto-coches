using TMPro;
using UnityEngine;

public class FinishLineCollision : MonoBehaviour
{
    [SerializeField] private RaceManager raceController;
    //[SerializeField] TMP_Text lapCounterText;
    [SerializeField] private bool isTimeTrialMode = false;
    public TrackCheck trackCheck;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("NPC lap increment called for: " + other.name);
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
                /*if (lapCounterText != null)
                {
                    lapCounterText.text = (TimeTrialMode.Instance.playerLapCounter).ToString();
                }*/

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

            if (carIdetifier.currentWayPoint >= carIdetifier.totalWaypoints)
            {
                if (carIdetifier.isPlayer)
                {
                    raceController.playerLapCounter++;
                    carIdetifier.currentLap++;
                    //lapCounterText.text = raceController.playerLapCounter.ToString();
                    raceController.FinishedRace();
                }
                else
                {
                
                    carIdetifier.currentLap++;
                    
                }
                carIdetifier.currentWayPoint = 0;
            }
        }
    }
}