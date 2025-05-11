using UnityEngine;

public class AgentHp : MonoBehaviour
{
    public float hp = 10f; // 현재 체력
    public float max_hp = 10f; // 최대 체력

    void Start()
    {
        hp = max_hp;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, max_hp); // 음수 방지
        Debug.Log("Player HP: " + hp);
    }
}
