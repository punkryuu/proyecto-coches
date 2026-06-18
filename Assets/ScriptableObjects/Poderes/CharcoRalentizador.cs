using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharcoRalentizador : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] Slider barraPoder;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && (barraPoder.value == barraPoder.maxValue))
        {
            CrearCharco();
        }
    }
    void CrearCharco()
    {
        Vector3 spawnPos = transform.position - transform.forward * 1.5f;
        RaycastHit hit;

        if (Physics.Raycast(spawnPos + Vector3.up * 2f, Vector3.down, out hit, 10f))
        {
            spawnPos = hit.point;
        }
        GameObject charco = Instantiate(Resources.Load("CharcoRalentizador") as GameObject, spawnPos, Quaternion.identity); // Instanciamos el charco en la posición del jugador
       
        charco.GetComponent<ColisionCharco>().owner = gameObject; // Asignamos el propietario del charco para que no afecte al jugador que lo creódime
        StartCoroutine(DuracionPoder(charco));  
        barraPoder.value = 0f; 
    }

    public IEnumerator DuracionPoder(GameObject charco)
    {
        float tiempo = 10f;
        while (tiempo > 0)
        {
            tiempo -= Time.deltaTime;
            charco.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, 1 - (tiempo / 10f)); //Reducimos el tamaño del charco
        }
        yield return null;
        Destroy(charco); 
    }

}
