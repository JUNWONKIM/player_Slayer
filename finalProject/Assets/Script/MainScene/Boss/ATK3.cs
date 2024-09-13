using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK3 : MonoBehaviour
{
    void Start()
    {
        // 30초 뒤에 오브젝트를 파괴
        Destroy(gameObject, 30.0f);
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌하면 화상 효과 적용
        if (other.CompareTag("Player"))
        {
            // PlayerBurn 스크립트를 찾아서 화상 효과 적용
            PlayerBurn playerBurn = other.GetComponent<PlayerBurn>();
            if (playerBurn != null)
            {
                playerBurn.ApplyBurn();
            }
        }
    }
}
