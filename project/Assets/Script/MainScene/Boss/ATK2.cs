using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ATK2 : MonoBehaviour
{
    public float speed = 10.0f; // 투사체 속도
    public float damage = 50.0f; // 데미지

    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {   
        //용사를 향해 날아감
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
