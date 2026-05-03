using TMPro;
using UnityEngine;

public class FinishLineCollision : MonoBehaviour
{
    [SerializeField] private ModoCarrera raceController;
    [SerializeField] TMP_Text lapCounterText;
    [SerializeField] private bool isTimeTrialMode = false;
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
        {
            Debug.Log("Trigger con: " + other.name);
            PlayerCar carIdetifier = other.GetComponentInParent<PlayerCar>();
            if (carIdetifier == null) return;

            if (carIdetifier.currentWayPoint >= carIdetifier.totalWaypoints)
            {
                if (carIdetifier.isPlayer)
                {
                    raceController.playerLapCounter++;
                    carIdetifier.currentLap++;
                    lapCounterText.text = raceController.playerLapCounter.ToString();
                    raceController.FinishedRace();
                }
                else
                {
                    raceController.IncrementNPCLapCounter(other.gameObject);
                }
                carIdetifier.currentWayPoint = 0;
            }
        }
    }
}