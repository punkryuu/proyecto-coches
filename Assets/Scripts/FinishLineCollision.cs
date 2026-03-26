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
            CarIdetifier carIdetifier = other.GetComponent<CarIdetifier>();
            if (carIdetifier == null) return;
            if(carIdetifier.isPlayer)
            {
                raceController.playerLapCounter++;
                lapCounterText.text = raceController.playerLapCounter.ToString();
            }
           else
            {
                raceController.IncrementNPCLapCounter(other.gameObject);
            }
        }
    }
}
