using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Personajes/Huevo")]
public class Huevo : PersonajeSO
{
    public GameObject piruletaPrefab;
    public override void UsePower(MonoBehaviour ejecutor)
    {
        ejecutor.StartCoroutine(LanzarPiruleta(ejecutor));
    }

    private IEnumerator LanzarPiruleta(MonoBehaviour ejecutor)
    {
        FSMManager _fsm = ejecutor.GetComponent<FSMManager>();

        if (piruletaPrefab == null)
        {
            Debug.LogError("PiruletaPrefab no está asignado en Huevo SO");
            yield break;
        }

        GameObject obj = Instantiate(
            piruletaPrefab,
            ejecutor.transform.position,
            ejecutor.transform.rotation
        );

        obj.GetComponent<PiruletaTrigger>().SetOwner(ejecutor.gameObject);
        yield return null;
    }
}

