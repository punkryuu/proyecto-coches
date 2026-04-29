using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
    public int raceCounter = 0;
    public Dictionary<PersonajeSO, GameObject> instances = new Dictionary<PersonajeSO, GameObject>();
    [SerializeField] TMP_Text countdownText;
    public float countdown = 3f;
    public int totalLaps = 3;
    private GameObject[] spawnedNPC;
    [SerializeField] Transform[] NPCpositions;
    [SerializeField]UIManager ui;
    

 
   // GameObject npc = Instantiate(NPC, Vector3.zero, Quaternion.identity);

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

        if (selectedCharacters.Count != characterCount || instances.Count != characterCount)
        {
            selectedCharacters.Clear();
            instances.Clear();
            SpawnEnemies();
        }
    }
    void Start()
    {
        StartCoroutine(StartCountdown());
       
    }

    void SpawnEnemies()
    {
        Debug.Log("Spawning NPCs...");
        selectedCharacters.Clear();
       instances.Clear();
        for (int i = 0; i < characterCount; i++)
        {
            int randomIndex = Random.Range(0, SOOptions.Length);
            PersonajeSO chosen = SOOptions[randomIndex];
            selectedCharacters.Add(chosen);
            Debug.Log("Instanciando: " + chosen.name);
            Transform spawn = NPCpositions[i];

            GameObject npcInstance = Instantiate(chosen.characterPrefab, spawn.position, spawn.rotation);

            NPCAgent agent = npcInstance.GetComponent<NPCAgent>();
            if(agent != null)
            {
                agent.SetTrackCheckpoints(FindObjectOfType<TrackCheck>());
                agent.SetStartPosition(spawn);
            }
            RegisterNPC(npcInstance);
            instances[chosen] = npcInstance;
        }

    }
 
    public void FinishedRace()
    {
        if(playerLapCounter >= totalLaps)
        {
            Debug.Log("FinishedRace ejecutado");
            List<PersonajeSO> posiciones = GetPositions();
            string podio = "";
            raceCounter++;
            playerLapCounter = 0;
            ui.contenedorFinalizar.SetActive(true);
            for (int i = 0; i <posiciones.Count; i++)
            {
               podio += (i + 1) + posiciones[i].characterPrefab.name + "\n";
                //ui.contenedorFinalizar.GetComponentInChildren<TMP_Text>().text = podio;
            }
            ui.contenedorFinalizar.GetComponentInChildren<TMP_Text>().text = podio;
        
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

        for (int i = 0; i < selectedCharacters.Count; i++)
        {
            for (int j = i + 1; j < selectedCharacters.Count; j++)
            {
                float progressI = CalculateProgress(instances[selectedCharacters[i]].gameObject);
                float progressJ = CalculateProgress(instances[selectedCharacters[j]]);
                if (progressJ > progressI)
                {
                    PersonajeSO temp = selectedCharacters[i];
                    selectedCharacters[i] = selectedCharacters[j];
                    selectedCharacters[j] = temp;
                }
            }
        }
        return selectedCharacters;
    }
    float CalculateProgress(GameObject car) //Calcula el progreso de un coche en la carrera, teniendo en cuenta las vueltas completadas, los waypoints y la distancia al siguiente waypoint
    {
     float progress = 0f;
     PlayerCar data = car.GetComponentInParent<PlayerCar>();
     if (data == null) 
      {
          return 0f;  
      }
     progress += data.currentLap * 100000f; // Cada vuelta completa vale 100000 puntos de progreso
     progress += data.currentWayPoint * 1000f;
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



