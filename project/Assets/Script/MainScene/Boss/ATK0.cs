using System.Collections;
using UnityEngine;

public class ATK0 : MonoBehaviour
{
    public float damage = 100f; // 기본 공격 데미지
    public float explosionDelay = 2f; // 폭발까지 딜레이

    private void Start()
    {
        Destroy(gameObject, explosionDelay); //딜레이 뒤 삭제
    }

    private void OnTriggerEnter(Collider other)
    {
      
        PlayerHP playerHP = other.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(damage);
        }
    }
     
}
