using UnityEngine;

public class GeneradorDePersonajes : MonoBehaviour
{

    public Transform[] NPCpositions;
    void Start()
    {
        var characters = ModoCarrera.Instance.selectedCharacters;
        for (int i = 0; i < NPCpositions.Length && i < characters.Count; i++)
        {
            Instantiate(characters[i].characterPrefab, NPCpositions[i].position, NPCpositions[i].rotation);
           
        }
    }
  
}
