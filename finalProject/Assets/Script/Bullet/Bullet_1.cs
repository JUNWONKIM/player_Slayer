using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_1 : MonoBehaviour
{
    public int damageAmount = 1; // 총알이 입히는 데미지 양
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체의 태그가 "Enemy"인 경우
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 충돌한 객체의 HP를 감소시킴
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }

            // 총알을 파괴
            Destroy(gameObject);
        }
    }

}
