using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK3_1 : MonoBehaviour
{
    public float damagePerSecond = 10.0f; // 초당 데미지

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
            transform.position = playerTransform.position; //용사 발 밑에 이펙트 유지
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
            yield return new WaitForSeconds(1.0f); //1초마다 데미지를 가함
        }
    }
}
