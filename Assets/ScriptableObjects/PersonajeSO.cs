using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PersonajeSO", menuName = "Scriptable Objects/PersonajeSO")]
public abstract class PersonajeSO : ScriptableObject
{
    public GameObject visual;
    public GameObject characterPrefab;
    public string driftParticlesPath = "driftParticles";
    public string turboParticlesPath = "turboParticles";
    public Sprite selectionImage;
    public float maxSpeedMultiplier = 1f;
    public float accelerationMultiplier = 1f;
    public float steeringMultiplier = 1f;
    public float weightMultiplier = 1f;
    public float driftControlMultiplier = 1f;
    public float turboMultiplier = 1f;
    public float airControlMultiplier = 1f;
    public float HitBoxRadius = 1.5f;//por defecto
    public float poderDuracion = 10f; // Duración del poder
    public string name;
    public string historia;

    [Header("0: selección, 1: Derrota, 2: Poder, 3: Golpe, 4: Truco, 5: Victoria, 6: Turbo")]

    public AudioClip[] audios;

    abstract public void UsePower(MonoBehaviour ejecutor);
    

}
