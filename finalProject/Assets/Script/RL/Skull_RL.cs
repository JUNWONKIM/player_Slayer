using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull_RL : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float damageAmount = 1f; // 데미지
    public float stopDistance = 5f; // 용사와의 최소 거리
    public Transform ownerAgent; // 타겟이 될 소환자 에이전트

    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true; // 데미지를 줄 수 있는 상태 여부

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
            // 오너와 충돌한 경우만 데미지 적용
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