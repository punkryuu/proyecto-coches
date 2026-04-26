using TMPro;
using UnityEngine;

public class FinishLineCollision : MonoBehaviour
{
    [SerializeField] private ModoCarrera raceController;
    [SerializeField] TMP_Text lapCounterText;
  
    void OnTriggerEnter(Collider other)
    {
        if (raceController.playerLapCounter < raceController.totalLaps)
        {
            Debug.Log("Trigger con: " + other.name);
            PlayerCar carIdetifier = other.GetComponentInParent<PlayerCar>();
            if (carIdetifier == null) return;

            if(carIdetifier.currentWayPoint >= carIdetifier.totalWaypoints)
            {
                 
            
            if (carIdetifier.isPlayer )
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