
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }
    public GameObject NPC;
    public PersonajeSO[] SOOptions;
    public List<PersonajeSO> selectedCharacters = new List<PersonajeSO>();
    public Dictionary<GameObject, int> lastPositions = new Dictionary<GameObject, int>();
    public int characterCount = 3;
    public int playerLapCounter = 0;
    public Dictionary<GameObject, int> npcLapCounter = new Dictionary<GameObject, int>();
    public int raceCounter = 0;
    public Dictionary<GameObject, PersonajeSO> instances = new Dictionary<GameObject, PersonajeSO>();
    [SerializeField] TMP_Text countdownText;
    public float countdown = 3f;
    public int totalLaps = 3;
    private GameObject[] spawnedNPC;
    [SerializeField] Transform[] NPCpositions;
    [SerializeField] UIManager ui;
    [SerializeField] MenuUIManager hud;
    public bool raceStarted = false;
    public PlayerCar player;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip sonidoInicio;





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
        Debug.Log("NPCs instanciados: " + instances.Count);

    }
    void Start()
    {
        player = FindObjectsOfType<PlayerCar>().FirstOrDefault(p => p.isPlayer);
        StartCoroutine(StartCountdown());


    }

    void Update()
    {
        if (!raceStarted) return;
        UpdatePositions();
        FinishedRace();
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
            Vector3 pos = spawn.position;
            RaycastHit hit;
            if (Physics.Raycast(pos + Vector3.up * 2f, Vector3.down, out hit, 10f))
            {
                pos.y = hit.point.y;
            }
            //Debug.LogWarning("Spawn position for " + chosen.name + ": " + spawn.position);

            GameObject npcInstance = Instantiate(chosen.characterPrefab, pos, spawn.rotation);


            IASINAPRENDIZAJE racer = npcInstance.GetComponent<IASINAPRENDIZAJE>();
            racer.InstantiateVisualSO(npcInstance, chosen);
            racer.IaPlayerCar.modelParent = npcInstance.transform.Find("VisualRoot");
            if (racer != null)
            {
                racer.spawnPoint = spawn;
                racer.trackCheck = FindAnyObjectByType<TrackCheck>();
            }


            PlayerCar car = npcInstance.GetComponent<PlayerCar>();
            if (car != null)
            {
                car.circuit = FindAnyObjectByType<WayPointsCircuit>();
                car.personajeData = chosen;
            }


            RegisterNPC(npcInstance);
            instances[npcInstance] = chosen;
        }

    }

    public void FinishedRace()
    {
        if (playerLapCounter >= totalLaps)
        {
            var posiciones = GetAllCarsOrdered();
            string podio = "";

            raceCounter++;
            playerLapCounter = 0;

            ui.contenedorFinalizar.SetActive(true);
            hud.panel.SetActive(false);

            for (int i = 0; i < posiciones.Count; i++)
            {
                var entry = posiciones[i];

                string nombre = "";

                if (entry.so == null)
                {
                    nombre = "Jugador";
                }
                else
                {
                    nombre = entry.so.name;
                }

                podio += (i + 1) + " - " + nombre + "\n";
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
            npc.GetComponent<PlayerCar>().currentLap++;
        }
        else
        {
            npcLapCounter[npc] = 1;// Si el NPC no tiene un contador, lo inicializamos en 1
        }
    }
    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);
        audioSource.PlayOneShot(sonidoInicio);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        countdownText.gameObject.SetActive(false);
        player.canMove = true;
        raceStarted = true;
    }

    public List<(GameObject car, PersonajeSO so)> GetAllCarsOrdered()
    {
        List<(GameObject car, PersonajeSO so )> lista = new();

        lista.Add((player.gameObject, player.personajeData));
        foreach (var kvp in instances)
        {
            lista.Add((kvp.Key, kvp.Value));
        }
        lista = lista.OrderByDescending(x => CalculateProgress(x.car)).ToList();

        return lista;
    }
    float CalculateProgress(GameObject car) //Calcula el progreso de un coche en la carrera, teniendo en cuenta las vueltas completadas, los waypoints y la distancia al siguiente waypoint
    {
        float progress = 0f;
        PlayerCar data = car.GetComponent<PlayerCar>();
        progress += data.currentLap * 100000f;
        progress += data.currentWayPoint * 1000f;
        progress -= data.distanceToNextWayPoint;
        return progress;
    }

    void UpdatePositions()
    {
        var orderedCars = GetAllCarsOrdered();

        for (int i = 0; i < orderedCars.Count; i++)
        {
            var entry = orderedCars[i];
            int position = i + 1;

            GameObject car = entry.car;

            if (lastPositions.ContainsKey(car))
            {
                int oldPosition = lastPositions[car];
                if (position < oldPosition)
                {
                    Debug.Log(car.name + " ha cambiado a la posición " + position);
                }
            }

            lastPositions[car] = position;
        }

    }
}
