using UnityEngine;
using UnityEngine.UI;

public class EstrellaManager : MonoBehaviour {
    [SerializeField] UIManager uiManager;
    [SerializeField] Slider barrapoder;

    private void Awake()
    {
        RotacionObjeto[] objetos = GetComponentsInChildren<RotacionObjeto>();

        foreach (RotacionObjeto r in objetos)
        {
            r.SetBarraPoder(barrapoder);
            r.SetUiManager(uiManager);
        }
    }
}