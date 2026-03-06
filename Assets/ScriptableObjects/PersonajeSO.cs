using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PersonajeSO", menuName = "Scriptable Objects/PersonajeSO")]
public class PersonajeSO : ScriptableObject
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
    public float verticalOffset = 0f;
    public float horizontalOffset = 0f;

}
