using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull_RL : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float damageAmount = 1f; // ������
    public float stopDistance = 5f; // ������ �ּ� �Ÿ�

    private Transform player;
    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true; // �������� �� �� �ִ� ���� ����

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator.GetBool("isDie")) // ��� ���� ���
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stopDistance)
            {
                // ��縦 ���� �̵�
                Vector3 moveDirection = (player.position - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }


            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0; // Y�� ȸ�� ����
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f); // �ε巯�� ȸ��

        }
    }

    private IEnumerator DamageCooldown() // ���ظ� ������ ���� �ð� ���ظ� ���� ���ϰ� ����
    {
        canDealDamage = false;
        yield return new WaitForSeconds(0.5f);
        canDealDamage = true;
    }

    // Trigger �浹 ó��
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDealDamage)
        {
            AgentHp AgentHP = other.GetComponent<AgentHp>();
            if (AgentHP != null)
            {
                AgentHP.TakeDamage(damageAmount); 
                StartCoroutine(DamageCooldown());
            }

        }
    }
}
