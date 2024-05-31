using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player_Atk_1 : MonoBehaviour
{
    public static Player_Atk_1 Instance;

    public float damageAmount = 1f; // 총알이 입히는 데미지 양

    

    void Awake()
    {
        Instance = this;
    }



   void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체의 태그가 "Creature"인 경우
        if (other.gameObject.CompareTag("Creature"))
        {
            // 충돌한 객체의 HP를 감소시킴
            CreatureHealth enemyHealth = other.gameObject.GetComponent<CreatureHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
               
             }

            // 총알을 파괴
            Destroy(gameObject);
        }
    }


    public void IncreaseDamage(float amount)
    {
        
        damageAmount += amount;
        Debug.Log("투사체 데미지 : " + damageAmount);
        
    }

}
