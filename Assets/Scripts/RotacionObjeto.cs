using UnityEngine;
using UnityEngine.UI;

public class RotacionObjeto : MonoBehaviour
{
    [SerializeField] Slider barraPoder;
    [SerializeField] UIManager uiManager;
    public Vector3 velocidadRotacion = new Vector3(50, 200, 0); 
    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime); 
    }
    public void OnTriggerEnter(Collider other)
    {
        // --- PLAYER ---
        if (other.CompareTag("Player"))
        {
            barraPoder.value += uiManager.progreso;
            Debug.Log("Detectado!");
        
        if (barraPoder.value > barraPoder.maxValue)
            barraPoder.value = barraPoder.maxValue;
        gameObject.SetActive(false);
        return;
        }
        // --- NPC ---
        NPCAgent npc = other.GetComponentInParent<NPCAgent>();
        if (npc != null)
        {
            npc.power += npc.maxPower * 0.25f; // Por ejemplo +25%
            if (npc.power > npc.maxPower)
                npc.power = npc.maxPower;

            Debug.Log("Power recogido por NPC");
            gameObject.SetActive(false);
            return;
        }
    }



}
