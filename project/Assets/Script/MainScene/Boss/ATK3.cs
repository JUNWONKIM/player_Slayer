using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK3 : MonoBehaviour
{
    void Start()
    {     
        Destroy(gameObject, 30.0f);  // 30초 뒤에 파괴
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBurn playerBurn = other.GetComponent<PlayerBurn>();
            if (playerBurn != null)
            {
                playerBurn.ApplyBurn();//용사에게 화상 적용
            }
        }
    }
}
