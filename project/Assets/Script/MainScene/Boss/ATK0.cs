using System.Collections;
using UnityEngine;

public class ATK0 : MonoBehaviour
{
    public float damage = 100f; // �⺻ ���� ������
    public float explosionDelay = 2f; // ���߱��� ������

    private void Start()
    {
        Destroy(gameObject, explosionDelay); //������ �� ����
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
