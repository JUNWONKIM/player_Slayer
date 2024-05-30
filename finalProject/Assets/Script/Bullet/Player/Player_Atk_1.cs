
using UnityEngine;

public class Player_Atk_1 : MonoBehaviour
{
    public static Player_Atk_1 Instance;
    public float damageAmount = 1f; // 총알이 입히는 데미지 양

    private bool isIncrease = false;
    

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

    // 데미지 증가 메서드
    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("Damage increased to: " + damageAmount);
    }
}
