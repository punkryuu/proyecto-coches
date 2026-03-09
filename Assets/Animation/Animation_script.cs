using UnityEngine;

public class Animation_script : MonoBehaviour
{
   Animator animator;   
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");

        float y = Input.GetAxis("Vertical");

        animator.SetFloat("x",x, 0.1f,Time.deltaTime);

        animator.SetFloat("y", y, 0.1f, Time.deltaTime);
    }
}
