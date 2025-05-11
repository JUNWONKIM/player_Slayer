using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull_RL : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float damageAmount = 1f; // 데미지
    public float stopDistance = 5f; // 용사와의 최소 거리

    private Transform player;
    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true; // 데미지를 줄 수 있는 상태 여부

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator.GetBool("isDie")) // 살아 있을 경우
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stopDistance)
            {
                // 용사를 향해 이동
                Vector3 moveDirection = (player.position - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }


            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0; // Y축 회전 고정
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f); // 부드러운 회전

        }
    }

    private IEnumerator DamageCooldown() // 피해를 입히면 일정 시간 피해를 주지 못하게 막음
    {
        canDealDamage = false;
        yield return new WaitForSeconds(0.5f);
        canDealDamage = true;
    }

    // Trigger 충돌 처리
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
