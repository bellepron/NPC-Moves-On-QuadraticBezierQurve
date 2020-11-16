using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBox : MonoBehaviour
{
    private GameObject player;
    private Rigidbody flyingBoxRb;

    void Start()
    {
        player = GameObject.Find("ThirdPersonController");
        flyingBoxRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var heading = player.transform.position - gameObject.transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        flyingBoxRb.AddForce(direction * Time.deltaTime * 100, ForceMode.Impulse);
        Destroy(gameObject, 1);
    }
}
