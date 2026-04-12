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
    public Dictionary<GameObject, int> lastPositions = new Dictionary<GameObject, int>();
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
      
    public List <PersonajeSO> GetPositions() //Bubble sort para ordenar los personajes según su progreso en la carrera, el que tenga más progreso va primero
    {
        List<PersonajeSO> order = new List<PersonajeSO>(selectedCharacters);
        for (int i = 0; i < order.Count; i++)
        {
            for (int j = i + 1; j < order.Count; j++)
            {
                float progressI = CalculateProgress(order[i].characterPrefab);
                float progressJ = CalculateProgress(order[j].characterPrefab);
                if (progressJ > progressI)
                {
                    PersonajeSO temp = order[i];
                    order[i] = order[j];
                    order[j] = temp;
                }
            }
        }
        return order;
    }
    float CalculateProgress(GameObject car) //Calcula el progreso de un coche en la carrera, teniendo en cuenta las vueltas completadas, los waypoints y la distancia al siguiente waypoint
    {
      float progress = 0f;
     CarIdetifier data = car.GetComponent<CarIdetifier>();
     progress += data.currentLap * 100000f;
     progress += data.currentLap * 1000f;
     progress -=  data.distanceToNextWayPoint;
     return progress;
    }

    void UpdatePositions()
    {
         List<PersonajeSO> orderedCharacters = GetPositions();
        for (int i = 0; i < orderedCharacters.Count; i++)
        {
            PersonajeSO character = orderedCharacters[i];
            int position = i + 1;

            if (lastPositions.ContainsKey(character.characterPrefab))
            {
                int oldPosition = lastPositions[character.characterPrefab];
                if (position < oldPosition)
                {
                    // Aquí puedes actualizar la UI o realizar otras acciones basadas en el cambio de posición
                    Debug.Log(character.characterPrefab.name + " ha cambiado a la posición " + position);
                    lastPositions[character.characterPrefab] = position;
                }
            }
            lastPositions[character.characterPrefab] = position;

        }

    }
}



