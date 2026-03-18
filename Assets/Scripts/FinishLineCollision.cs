using TMPro;
using UnityEngine;

public class FinishLineCollision : MonoBehaviour
{
    [SerializeField] private ModoCarrera raceController;
    [SerializeField] TMP_Text lapCounterText;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("finish") && raceController.lapCounter <= 3)
        {
            raceController.lapCounter++;
            lapCounterText.text = raceController.lapCounter.ToString();
        }
    }
}
