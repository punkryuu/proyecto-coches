using TMPro;
using UnityEngine;

public class FinishLineCollision : MonoBehaviour
{
    [SerializeField] private ModoCarrera raceController;
    [SerializeField] TMP_Text lapCounterText;
    void OnTriggerEnter(Collider other)
    {
        if (raceController.playerLapCounter <= 3)
        {
            Debug.Log("Trigger con: " + other.name);
            CarIdetifier carIdetifier = other.GetComponentInParent<CarIdetifier>();
            if (carIdetifier == null) return;

            if(carIdetifier.currentWayPoint >= carIdetifier.totalWaypoints)
            {
                 
            
            if (carIdetifier.isPlayer )
            {
                raceController.playerLapCounter++;
                lapCounterText.text = raceController.playerLapCounter.ToString();
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