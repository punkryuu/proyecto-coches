using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Text valueText;
    int progress = 0;
    public Slider slider;

    public void UpdatePorgress()
    {
        progress++;
        slider.value = progress;
    }

}
