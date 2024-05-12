using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Atk_1: MonoBehaviour
{
    public int damageAmount = 1; // 총알이 입히는 데미지 양
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체의 태그가 "Creature"인 경우
        if (collision.gameObject.CompareTag("Creature"))
        {
            // 충돌한 객체의 HP를 감소시킴
            CreatureHealth enemyHealth = collision.gameObject.GetComponent<CreatureHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }

            // 총알을 파괴
            Destroy(gameObject);
        }
    }

}
