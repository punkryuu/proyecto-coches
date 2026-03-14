using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModoCarrera : MonoBehaviour
{
    public GameObject NPC;
    public PersonajeSO[] SOOptions;
    public Transform[] NPCpositions;
    [SerializeField] TMP_Text countdownText;
    public float countdown = 3f;

    private GameObject[] spawnedNPC;
    void Start()
    {
        spawnedNPC = new GameObject[NPCpositions.Length];
        SpawnEnemies();
        StartCoroutine(StartCountdown());
    }

    void SpawnEnemies()
    {       
        for (int i = 0; i < NPCpositions.Length; i++)
        {
            spawnedNPC[i] = Instantiate(NPC, NPCpositions[i].position, Quaternion.identity);
            int randomIndex = Random.Range(0, SOOptions.Length);
            PersonajeSO chosen = SOOptions[randomIndex];
            spawnedNPC[i] = NPC;

        }
    }

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        countdownText.gameObject.SetActive(false);
    }
        
}
