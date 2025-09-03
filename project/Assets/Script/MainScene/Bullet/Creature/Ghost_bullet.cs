using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_bullet : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {      
            Destroy(gameObject);
        }
    }
}
