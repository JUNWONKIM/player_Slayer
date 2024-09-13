using System.Collections;
using UnityEngine;

public class ATK0 : MonoBehaviour
{
    public float damage = 100f; // 폭발물이 플레이어에게 줄 피해량
    public float explosionDelay = 2f; // 폭발 후 사라지기까지의 시간

    private void Start()
    {
        Destroy(gameObject, explosionDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 플레이어인지 확인
        PlayerHP playerHP = other.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            // 플레이어에게 피해를 줌
            playerHP.TakeDamage(damage);

          
        }
    }

 
}
