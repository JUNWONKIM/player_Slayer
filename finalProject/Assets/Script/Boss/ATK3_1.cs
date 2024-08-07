using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK3_1 : MonoBehaviour
{
    public float damagePerSecond = 10.0f; // 1초마다 입힐 데미지
    private Transform playerTransform;
    private PlayerHP playerHP;
    private bool isDamaging = false;

    void Start()
    {
        // 플레이어 오브젝트를 찾음
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
            // 이펙트를 플레이어의 발밑에 위치시킴
            transform.position = playerTransform.position;
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
            yield return new WaitForSeconds(1.0f);
        }
    }
}
