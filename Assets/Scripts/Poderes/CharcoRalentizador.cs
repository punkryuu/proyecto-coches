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
        GameObject charco = Instantiate(Resources.Load("CharcoRalentizador") as GameObject, transform.position, Quaternion.identity); // Instanciamos el charco en la posición del jugador
        StartCoroutine(DuracionPoder(charco));  
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
