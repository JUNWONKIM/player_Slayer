using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK3 : MonoBehaviour
{
    void Start()
    {     
        Destroy(gameObject, 30.0f);  // 30�� �ڿ� �ı�
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBurn playerBurn = other.GetComponent<PlayerBurn>();
            if (playerBurn != null)
            {
                playerBurn.ApplyBurn();//��翡�� ȭ�� ����
            }
        }
    }
}
