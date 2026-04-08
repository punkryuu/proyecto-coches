using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class ModoCarrera : MonoBehaviour
{
    public static ModoCarrera Instance { get; private set; }
    public GameObject NPC;
    public PersonajeSO[] SOOptions;
    public  List <PersonajeSO> selectedCharacters = new List<PersonajeSO>();
    public int characterCount = 3;
    public int playerLapCounter = 0;
    public Dictionary<GameObject,int> npcLapCounter = new Dictionary<GameObject,int>();
    int raceCounter = 0;

    [SerializeField] TMP_Text countdownText;
    public float countdown = 3f;

    private GameObject[] spawnedNPC;

    public void Awake()
     {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;

        }
        else
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (selectedCharacters.Count == 0)
        {
            SpawnEnemies();
        }
    }
    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    void SpawnEnemies()
    {
       selectedCharacters.Clear();
        for (int i = 0; i < characterCount; i++)
        {
            int randomIndex = Random.Range(0, SOOptions.Length);
            PersonajeSO chosen = SOOptions[randomIndex];
            selectedCharacters.Add(chosen);                
        }

    }

 
    void NextRace()
    {
        if(playerLapCounter > 3)
        {
            raceCounter++;
            UnityEngine.SceneManagement.SceneManager.LoadScene(raceCounter);//cambiar cuando esté el siguiente escenario
        }
    }

    public void RegisterNPC(GameObject npc)
    {
        if (!npcLapCounter.ContainsKey(npc))
            npcLapCounter.Add(npc, 0);
    }
    public void IncrementNPCLapCounter(GameObject npc)
    {
        if (npcLapCounter.ContainsKey(npc))
        {
            npcLapCounter[npc]++;
        }
        else
        {
            npcLapCounter[npc] = 1;// Si el NPC no tiene un contador, lo inicializamos en 1
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
