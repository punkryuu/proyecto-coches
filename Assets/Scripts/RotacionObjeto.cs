using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RotacionObjeto : MonoBehaviour {
    Slider barraPoder;
    UIManager uiManager;

    public Vector3 velocidadRotacion = new Vector3(50, 200, 0);

    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC"))
        {
            if(other.CompareTag("Player"))
            { 
            barraPoder.value += uiManager.GetProgreso();

            if (barraPoder.value > barraPoder.maxValue)
                barraPoder.value = barraPoder.maxValue;

            Debug.Log("Detectado!");
            }
            else
            {
                Debug.Log("NPC detectado!");
            }
            StartCoroutine(Reaparecer());

        }
    }

    IEnumerator Reaparecer()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(5f);

        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }

    public void SetBarraPoder(Slider slider)
    {
        barraPoder = slider;
    }

    public void SetUiManager(UIManager um)
    {
        uiManager = um;
    }
}