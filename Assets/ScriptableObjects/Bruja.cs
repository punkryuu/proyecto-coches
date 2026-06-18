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
       AudioClip grito = this.audios[2];
        foreach (IASINAPRENDIZAJE npc in GameObject.FindObjectsOfType<IASINAPRENDIZAJE>())
        {
            if (npc.gameObject == ejecutor.gameObject) continue;
            npc.IaPlayerCar.powerCounter *= 0.75f; // Reduce su barra de poder
        }

        // Afecta al Player
        PlayerCar player = GameObject.FindObjectOfType<PlayerCar>();
        if (player != null && player.gameObject != ejecutor.gameObject)
        {
         
            if (player.powerCounter != null)
            {
                player.powerCounter *= 0.75f; // Elimina toda la barra de poder del enemigo
            }
        }
        yield return null;
    }
}

