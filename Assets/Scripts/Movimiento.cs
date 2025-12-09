using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Movimiento: MonoBehaviour
{
    public Rigidbody rb;
    public float aceleracionFrontal = 10f;
    public float aceleracionTrasera = 5f;
    public float velocidadMaxima = 50f;
    public float fuerzaGiro = 180f;
    public float gravedad = 10f;
    public float TamanoRayoSuelo = 0.5f;
    public float agarre = 3f;

    public Transform RayoSuelo;
    public LayerMask capaSuelo;
    private float velocidadInput;
    private float giroInput;

    private bool suelo;


void Start()
{
    rb.transform.parent = null; //Quitamos el parentesco del Rigidbody con el objeto padre pero seguimos siguiendo al objeto padre
}  
    private void Update()
    {
      velocidadInput = 0f;
      float vertical = 0f;
      float horizontal = 0f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical = 1f;
       
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical = -1f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontal = -1f;
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontal = 1f;

        velocidadInput = 0f;

        if (vertical > 0)
        {
            velocidadInput = vertical * aceleracionFrontal * 1000f;
        }
        else if (vertical < 0)
        {
            velocidadInput = vertical * aceleracionTrasera * 1000f;
        }

        giroInput = horizontal;

        if (suelo)
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles +
                new Vector3(0f, giroInput * fuerzaGiro * Time.deltaTime * vertical, 0f)
            );
        }
    }

    private void FixedUpdate()
    {
        suelo = false;
        RaycastHit hit;

        if(Physics.Raycast(RayoSuelo.position, -transform.up, out hit, TamanoRayoSuelo, capaSuelo))//Si el rayo choca con el suelo y lo que toca tiene la capa suelo, la variable suelo es cierta
        {
            suelo = true;
        }

        if(suelo)
        { 
            rb.linearDamping = agarre; //Si est� en el suelo, el drag es igual al agarre
            if (Mathf.Abs(velocidadInput) > 0)
            {
                rb.AddForce(transform.forward * velocidadInput);
             }
        }
        else
        {
            rb.linearDamping = 0.1f; //Si no est� en el suelo, el drag es muy bajo
            rb.AddForce(Vector3.up * -gravedad * 100);
        }
    }
}


