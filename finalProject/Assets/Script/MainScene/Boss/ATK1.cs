using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK1 : MonoBehaviour
{
    public float damage = 100f; // 공격 데미지
    private bool isAttacking = false; // 공격 중 여부
    private float attackCooldown = 0.2f; // 공격 쿨타임
    private PlayerHP playerHP; // 플레이어 체력 스크립트 참조

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHP = null;
            isAttacking = false;
            StopCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        while (isAttacking)
        {
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
            }
            yield return new WaitForSeconds(attackCooldown);
        }
    }
}
