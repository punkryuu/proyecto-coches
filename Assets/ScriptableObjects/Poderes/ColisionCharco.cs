using UnityEngine;
using System.Collections;

//Poner este script al charco, añadir la Tag enemigo 
public class ColisionCharco : MonoBehaviour
{
     float duracion = 10f;
     float factorReduccion = 0.5f;
    public GameObject owner;

    private void Start()
    {
        StartCoroutine(VidaCharco());
    }
    IEnumerator VidaCharco()
    {
        yield return new WaitForSeconds(duracion);
        Destroy(gameObject);

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return; // Ignora colisiones con el propietario del charco

           PersonajeSO enemigo = other.gameObject.GetComponent<PersonajeSO>();
            if (enemigo != null)
            {             
                enemigo.maxSpeedMultiplier = factorReduccion; // Reduce la velocidad a la mitad      
            }              
    }
    private void OnDestroy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach(Collider col in colliders)
        {
            if (col.gameObject == owner) continue; // Ignora el propietario del charco
            {
                PersonajeSO enemigo = col.gameObject.GetComponent<PersonajeSO>();
                if (enemigo != null)
                {
                    enemigo.maxSpeedMultiplier = 1f; // Restaura la velocidad original
                }
            }
        }
    }
}
