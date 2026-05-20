using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Personajes/Bruja")]
public class Bruja : PersonajeSO
{
    public override void UsePower(MonoBehaviour ejecutor)
    {
        ejecutor.StartCoroutine(ActivarGrito(ejecutor));
    }

    private IEnumerator ActivarGrito(MonoBehaviour ejecutor)
    {
        // Afecta a todos los NPC
        foreach (NPCAgent npc in GameObject.FindObjectsOfType<NPCAgent>())
        {
            if (npc.gameObject == ejecutor.gameObject) continue;
            npc.power *= 0.75f; // Reduce su barra de poder
        }

        // Afecta al Player
        PlayerCar player = GameObject.FindObjectOfType<PlayerCar>();
        if (player != null && player.gameObject != ejecutor.gameObject)
        {
            Slider slider = player.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                slider.value *= 0.75f; // Elimina toda la barra de poder del enemigo
            }
        }
        yield return null;
    }
}

