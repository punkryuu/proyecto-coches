using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Personajes/Mosquetero")]
public class Mosquetero : PersonajeSO
{
    Material material;

    public override void UsePower(MonoBehaviour ejecutor)
    {
        ejecutor.StartCoroutine(ActivarInvencibilidad(ejecutor));
    }

    private IEnumerator ActivarInvencibilidad(MonoBehaviour ejecutor)
    {
        FSMManager _fsm = ejecutor.GetComponent<FSMManager>();

        Renderer[] renderers = ejecutor.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);

            mpb.SetFloat("_ghost_Activate", 1f);
            r.SetPropertyBlock(mpb);
        }
        _fsm.stunDuration =0;
        _fsm.canBeStunned = false; // El personaje no puede ser aturdido
        yield return new WaitForSeconds(10f);

        foreach (Renderer r in renderers)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            r.GetPropertyBlock(mpb);

            mpb.SetFloat("_ghost_Activate", 0f);
            r.SetPropertyBlock(mpb);
        }
        _fsm.canBeStunned = true;
    }
}

