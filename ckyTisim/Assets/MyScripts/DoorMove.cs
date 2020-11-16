using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMove : MonoBehaviour
{
    private Animator animator;
    public bool opened;

    // Start is called before the first frame update
    void Start()
    {
        animator=GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Character"))
        {
            animator.SetBool("Open", true);
            opened=true;
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if(other. CompareTag("Character"))
    //     {
    //         animator.SetBool("Open", false);
    //         Debug.Log("closed");
    //     }
    // }
}
