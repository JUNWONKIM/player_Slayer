using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 1; // 최대 체력
    private int currentHealth; // 현재 체력

    void Start()
    {
        currentHealth = maxHealth; // 시작할 때 최대 체력으로 설정
    }

    // 데미지를 입었을 때 호출되는 함수
    public void TakeDamage(int amount)
    {
        currentHealth -= amount; // 데미지만큼 체력 감소

        if (currentHealth <= 0)
        {
            Die(); // 체력이 0 이하이면 사망 처리
        }
    }

    void Die()
    {
        // 적 오브젝트를 파괴
        Destroy(gameObject);
    }
}
