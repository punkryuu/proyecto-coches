using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class RotacionObjeto : MonoBehaviour {
    Slider barraPoder;
    UIManager uiManager;
    public AudioMixer mixer;
    public AudioMixerGroup sfxGroup;
    [SerializeField] AudioClip sonidoRecoger;

    public Vector3 velocidadRotacion = new Vector3(50, 200, 0);

    void Awake()
    {
        AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
        if (groups.Length > 0)
            sfxGroup = groups[0];
    }

    void Update()
    {
        transform.Rotate(velocidadRotacion * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           AudioSource audio = this.GetComponent<AudioSource>();
            audio.PlayOneShot(sonidoRecoger);
            barraPoder.value += uiManager.GetProgreso();

            if (barraPoder.value > barraPoder.maxValue)
                barraPoder.value = barraPoder.maxValue;

            Debug.Log("Detectado!");

            StartCoroutine(Reaparecer());
        }
        else
        {
            IASINAPRENDIZAJE ia = other.GetComponentInParent<IASINAPRENDIZAJE>();
            if (ia != null)
            {
                ia.IaPlayerCar.powerCounter++;
                StartCoroutine(Reaparecer());
            }
        }
    }

    IEnumerator Reaparecer()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(5f);

        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }

    public void SetBarraPoder(Slider slider)
    {
        barraPoder = slider;
    }

    public void SetUiManager(UIManager um)
    {
        uiManager = um;
    }
}