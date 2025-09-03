using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK3_1 : MonoBehaviour
{
    public float damagePerSecond = 10.0f; // �ʴ� ������

    private Transform playerTransform;
    private PlayerHP playerHP;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHP = player.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                StartCoroutine(DamagePlayer());
            }
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position; //��� �� �ؿ� ����Ʈ ����
        }
    }

    IEnumerator DamagePlayer()
    {
        while (true)
        {
            if (playerHP != null)
            {
                playerHP.TakeDamage(damagePerSecond);
            }
            yield return new WaitForSeconds(1.0f); //1�ʸ��� �������� ����
        }
    }
}
