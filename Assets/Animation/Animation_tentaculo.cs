using UnityEngine;

public class Animation_tentaculo : MonoBehaviour
{
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {




        animator.SetBool("golpea", true);

        animator.SetBool("golpea", false);
    }
}
