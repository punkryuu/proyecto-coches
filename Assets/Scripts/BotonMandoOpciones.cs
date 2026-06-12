using UnityEngine;
using UnityEngine.EventSystems;

public class BotonMandoOpciones : MonoBehaviour {
    [SerializeField] GameObject firtObject;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firtObject);
    }
}
