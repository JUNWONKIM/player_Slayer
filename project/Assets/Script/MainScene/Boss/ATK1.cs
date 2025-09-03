using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK1 : MonoBehaviour
{
    public float damage = 100f; // 데미지

    private bool isAttacking = false; // 공격 중 여부
    private float attackCooldown = 0.2f; // 공격 쿨타임
    private PlayerHP playerHP;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHP = other.GetComponent<PlayerHP>();
            if (playerHP != null && !isAttacking) 
            {
                isAttacking = true;
                StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerExit(Collider other) //범위 이탈 시
    {
        if (other.CompareTag("Player"))
        {
            playerHP = null;
            isAttacking = false;
            StopCoroutine(Attack()); //멈춤
        }
    }

    private IEnumerator Attack() //공격
    {
        while (isAttacking)
        {
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage); //용사에게 데미지 가함
            }
            yield return new WaitForSeconds(attackCooldown); //쿨타임 기다림
        }
    }
}
