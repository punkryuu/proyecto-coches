using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Personajes/Mosquetero")]
public class Mosquetero : PersonajeSO
{
    public override void UsePower(MonoBehaviour ejecutor)
    {
        ejecutor.StartCoroutine(ActivarInvencibilidad(ejecutor));
    }

    private IEnumerator ActivarInvencibilidad(MonoBehaviour ejecutor)
    {
        FSMManager _fsm = ejecutor.GetComponent<FSMManager>();
        //modificar shader
        _fsm.canBeStunned = false; // El personaje no puede ser aturdido
        yield return new WaitForSeconds(10f);
        //modificar shader

        _fsm.canBeStunned = true;
    }
}

