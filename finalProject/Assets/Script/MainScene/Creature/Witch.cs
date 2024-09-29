using UnityEngine;
using System.Collections;

public class Witch : MonoBehaviour
{
    public float moveSpeed = 3.0f;  // 이동 속도
    public float stopDistance = 2.0f;  // 공격 사거리
    public GameObject attackParticlePrefab;  // 공격 파티클 프리팹
    public float attackCooldown = 5.0f;  // 공격 쿨타임

    private Transform player;
    private Rigidbody rb;
    private Animator animator;
    private bool isAttacking = false; //공격 상태 여부
    private float lastAttackTime; //마지막 공격 시간 저장
    private bool initialAttack = true;  // 첫 공격 여부
    private float distanceToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (animator.GetBool("isDie")) return; //죽을 경우 멈춤

        if (player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stopDistance && !isAttacking) //사거리 밖일 경우 & 공격 중이지 않으면
            {
                MoveTowardsPlayer(); //용사를 향해 움직임
            }
            else if (distanceToPlayer <= stopDistance) //사거리 안이면
            {
                rb.velocity = Vector3.zero; //속도를 0으로 변경
                LookAtPlayer();  // 용사를 향해 회전

                if (!isAttacking && (initialAttack || Time.time >= lastAttackTime + attackCooldown)) //쿨타임이 돌았을 경우
                {
                    Attack();  // 공격 시작
                }
                else if (!isAttacking)
                {
                    animator.SetBool("isIdle", true);  //쿨타임일 경우 idle 애니메이션 실행
                }
            }
        }
    }

    void MoveTowardsPlayer() //용사를 향해 이동
    {
        animator.SetBool("isIdle", false); //이동 애니메이션 실행

        //마녀 이동
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);
        LookAtPlayer();

    }

    void LookAtPlayer() //용사를 향해 회전
    {
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;  // y 축 회전 방지
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10.0f);
    }

    void Attack() //공격
    {
        isAttacking = true;

        // 공격 애니메이션 실행
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", true);

        if (!ParticleExists())
        {
            Instantiate(attackParticlePrefab, player.position, Quaternion.identity);  // 공격 파티클 생성
        }
        lastAttackTime = Time.time;  // 마지막 공격 시간을 갱신
        initialAttack = false;  // 첫 공격 뒤 false로 수정
        Invoke("ResetAttack", 1.0f);  // 애니메이션이 끝난 후 isAttacking 상태를 리셋
    }

    bool ParticleExists() //파티클 존재 여부 확인
    {
        return GameObject.FindWithTag(attackParticlePrefab.tag) != null;
    }

    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttack", false);  // 공격 애니메이션 종료

        if (distanceToPlayer <= stopDistance)
        {
            animator.SetBool("isIdle", true);  // 쿨타임 중일 때 idle 애니메이션 활성화
        }
    }


}
