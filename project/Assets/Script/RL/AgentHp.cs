using UnityEngine;

public class AgentHp : MonoBehaviour
{
    public float hp = 10f; // ���� ü��
    public float max_hp = 10f; // �ִ� ü��

    void Start()
    {
        hp = max_hp;
    }
    public void SetHp(float newHp, float newMax = -1f)
    {
        if (newMax > 0f) max_hp = newMax;
        hp = Mathf.Clamp(newHp, 0f, max_hp);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, max_hp); // ���� ����
        Debug.Log("Player HP: " + hp);
    }
}
