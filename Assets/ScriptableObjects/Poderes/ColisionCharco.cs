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
        IASINAPRENDIZAJE ownerIA = owner.GetComponent<IASINAPRENDIZAJE>();
        FSMManager ownerFSM = owner.GetComponent<FSMManager>();
        if (ownerIA == null || ownerFSM == null)
            yield break;
        yield return new WaitForSeconds(duracion);
        Destroy(gameObject);

    }
    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colision con enemigo: ");
        if (other.gameObject == owner) return; // Ignora colisiones con el propietario del charco

        PlayerCar enemigo = other.GetComponentInParent<PlayerCar>();
        if (enemigo != null)
            {             
                Debug.Log("Colision con enemigo: " + enemigo.name);
                enemigo.personajeData.maxSpeedMultiplier = factorReduccion; // Reduce la velocidad a la mitad      
            }              
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == owner) return;

        PlayerCar enemigo = other.GetComponentInParent<PlayerCar>();
        if (enemigo != null)
        {
            enemigo.personajeData.maxSpeedMultiplier = 1f; // Restaura al salir
        }
    }
    private void OnDestroy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach(Collider col in colliders)
        {
            if (col.gameObject == owner) continue; // Ignora el propietario del charco
            {
                PlayerCar enemigo = col.GetComponentInParent<PlayerCar>();
                if (enemigo != null)
                {
                    enemigo.personajeData.maxSpeedMultiplier = 1f; // Restaura la velocidad original
                }
            }
        }
    }
}
