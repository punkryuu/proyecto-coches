using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Personajes/Rana")]
public class Rana : PersonajeSO
{
    public GameObject charcoRalentizador;

    public override void UsePower(MonoBehaviour ejecutor)
    {
        ejecutor.StartCoroutine(SpawnCharco(ejecutor));
    }

    private IEnumerator SpawnCharco(MonoBehaviour ejecutor)
    {
        FSMManager _fsm = ejecutor.GetComponent<FSMManager>();

        if (charcoRalentizador == null)
        {
            Debug.LogError("charcoRalentizador no está asignado en Rana SO");
            yield break;
        }

        GameObject charco = Instantiate(
            charcoRalentizador,
            ejecutor.transform.position,
            Quaternion.Euler(0f,90f,0f)
        );
        charco.transform.localScale = new Vector3(10f, 1f, 10f);
        charco.GetComponent<ColisionCharco>().owner = ejecutor.gameObject;

        float tiempo = poderDuracion;

        while (tiempo > 0)
        {
            tiempo -= Time.deltaTime;

            charco.transform.localScale = Vector3.Lerp(
                Vector3.one,
                Vector3.zero,
                1 - (tiempo / poderDuracion)
            );

            yield return null;
        }

        Destroy(charco);
    }
}


