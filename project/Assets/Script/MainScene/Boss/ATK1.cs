using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK1 : MonoBehaviour
{
    public float damage = 100f; // ������

    private bool isAttacking = false; // ���� �� ����
    private float attackCooldown = 0.2f; // ���� ��Ÿ��
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

    private void OnTriggerExit(Collider other) //���� ��Ż ��
    {
        if (other.CompareTag("Player"))
        {
            playerHP = null;
            isAttacking = false;
            StopCoroutine(Attack()); //����
        }
    }

    private IEnumerator Attack() //����
    {
        while (isAttacking)
        {
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage); //��翡�� ������ ����
            }
            yield return new WaitForSeconds(attackCooldown); //��Ÿ�� ��ٸ�
        }
    }
}
