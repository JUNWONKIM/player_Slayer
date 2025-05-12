using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull_RL : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float damageAmount = 1f; // ������
    public float stopDistance = 5f; // ������ �ּ� �Ÿ�
    public Transform ownerAgent; // Ÿ���� �� ��ȯ�� ������Ʈ

    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true; // �������� �� �� �ִ� ���� ����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator.GetBool("isDie") && ownerAgent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, ownerAgent.position);

            if (distanceToPlayer > stopDistance)
            {
                Vector3 moveDirection = (ownerAgent.position - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }

            Vector3 lookDirection = ownerAgent.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(0.5f);
        canDealDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDealDamage)
        {
            // ���ʿ� �浹�� ��츸 ������ ����
            if (ownerAgent != null && other.transform == ownerAgent)
            {
                AgentHp agentHP = other.GetComponent<AgentHp>();
                if (agentHP != null)
                {
                    agentHP.TakeDamage(damageAmount);
                    StartCoroutine(DamageCooldown());
                }
            }
        }
    }
}