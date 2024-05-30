using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Atk_2_1 : MonoBehaviour
{
    public static Player_Atk_2_1 Instance;
    public float damageAmount = 1f; // 총알이 입히는 데미지 양

    private bool isIncrease = false;

    public float lifetime = 0.5f; // 오브젝트가 사라지기까지의 시간

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 오브젝트를 lifetime 시간 후에 사라지게 함
        StartCoroutine(DestroyAfterLifetime());
    }

    private IEnumerator DestroyAfterLifetime()
    {
        // lifetime 시간 동안 대기
        yield return new WaitForSeconds(lifetime);

        // 현재 오브젝트를 파괴
        Destroy(gameObject);
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

        }
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("Damage increased to: " + damageAmount);
    }
}
