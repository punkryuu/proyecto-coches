using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PersonajeSO", menuName = "Scriptable Objects/PersonajeSO")]
public class PersonajeSO : ScriptableObject
{
    public GameObject characterPrefab;
    public Sprite selectionImage;
}
