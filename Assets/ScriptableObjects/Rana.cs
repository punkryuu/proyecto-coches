using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Personajes/Rana")]
public class Rana : PersonajeSO
{
  
    public override void UsePower(MonoBehaviour ejecutor)
    {
        ejecutor.StartCoroutine(SpawnCharco(ejecutor));
    }

    private IEnumerator SpawnCharco(MonoBehaviour ejecutor)
    {
        GameObject charco = Instantiate(
            Resources.Load("CharcoRalentizador") as GameObject,
            ejecutor.transform.position,
            Quaternion.identity
        );

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
    

