using System.Collections;
using UnityEngine;

public class Animation_tentaculo : MonoBehaviour {
    Animator animator;

    [SerializeField] Collider golpeCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("golpea", false);

        golpeCollider.enabled = false;

        StartCoroutine(AtaqueAutomatico());
    }

    IEnumerator AtaqueAutomatico()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            animator.SetBool("golpea", true);
            yield return new WaitForSeconds(4.25f);
             golpeCollider.enabled = true;

            yield return new WaitForSeconds(1.75f);
            golpeCollider.enabled = false;

            yield return new WaitForSeconds(2f);

            animator.SetBool("golpea", false);



        }
    }
}