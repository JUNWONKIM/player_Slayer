using UnityEngine;

public class Witch_freeze : MonoBehaviour
{
    public float moveSpeed = 3.0f;  // 마녀의 이동 속도
    public float stopDistance = 2.0f;  // 마녀가 멈추는 거리
    public GameObject attackParticlePrefab;  // 공격 파티클 프리팹
    public float attackCooldown = 5.0f;  // 공격 쿨타임

    private Transform player;  // 플레이어의 Transform
    private Rigidbody rb;
    private Animator animator;
    private bool isAttacking = false;
    private float lastAttackTime;
    private bool initialAttack = true;  // 첫 공격 여부
    private float distanceToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Rigidbody 컴포넌트를 가져옵니다.
        animator = GetComponent<Animator>();  // Animator 컴포넌트를 가져옵니다.
        player = GameObject.FindGameObjectWithTag("Player").transform;  // "Player" 태그를 가진 오브젝트를 찾습니다.
    }

    void FixedUpdate()
    {
        if (animator.GetBool("isDie")) return;

        if (player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stopDistance && !isAttacking)
            {
                MoveTowardsPlayer();
            }
            else if (distanceToPlayer <= stopDistance)
            {
                rb.velocity = Vector3.zero;  // 플레이어와 일정 거리 이내로 다가가면 멈춥니다.
                LookAtPlayer();  // 멈춘 상태에서도 플레이어를 바라보게 합니다.

                if (!isAttacking && (initialAttack || Time.time >= lastAttackTime + attackCooldown))
                {
                    Attack();  // 공격을 시작합니다.
                }
                else if (!isAttacking)
                {
                    animator.SetBool("isIdle", true);  // 쿨타임 중일 때 idle 애니메이션 활성화
                }
            }
        }
    }

    void MoveTowardsPlayer()
    {
        animator.SetBool("isIdle", false);  // 이동 중이므로 idle 애니메이션 비활성화

        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(transform.position + move);
        LookAtPlayer();  // 플레이어를 바라보게 합니다.
    }

    void LookAtPlayer()
    {
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;  // y 축 회전은 방지하여 마녀가 수평으로만 회전하도록 합니다.
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10.0f);
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetBool("isIdle", false);  // 공격 중이므로 idle 애니메이션 비활성화
        animator.SetBool("isAttack", true);  // 공격 애니메이션 시작

        // 파티클이 이미 존재하는지 확인 후 생성
        if (!ParticleExists())
        {
            Instantiate(attackParticlePrefab, player.position, Quaternion.identity);  // 공격 파티클 생성
        }

        lastAttackTime = Time.time;  // 마지막 공격 시간을 갱신
        initialAttack = false;  // 첫 공격을 마쳤으므로 false로 설정
        Invoke("ResetAttack", 1.0f);  // 애니메이션이 끝난 후 isAttacking 상태를 리셋
    }

    bool ParticleExists()
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

    public void DieAnimationComplete()
    {
        // 스크립트를 비활성화하여 이동 및 회전 멈춤
        enabled = false;
    }
}
