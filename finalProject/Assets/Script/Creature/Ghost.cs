using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 5f; // 적의 이동 속도
    public float stoppingDistance = 5f; // 플레이어와의 멈추는 거리
    public float retreatDistance = 5f; // 플레이어로부터 후퇴하는 거리

    public float bulletSpeed = 50f;
    public GameObject projectile; // 발사할 투사체
    public Transform firePoint; // 발사 지점
    public float fireRate = 1f; // 발사 속도 (1초당 한 발)
    public float nextFireTime = 0f;

    private Transform player; // 플레이어의 위치
    private Rigidbody rb; // 고스트의 Rigidbody 컴포넌트
    private Animator animator; // 고스트의 애니메이터 컴포넌트

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 위치 찾기
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
    }

    void Update()
    {
        if (player != null && !animator.GetBool("isDie"))
        {
            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                // 플레이어를 향해 이동
                Move();
            }
            else if (distanceToPlayer <= stoppingDistance && distanceToPlayer > retreatDistance)
            {
                // 공격 상태로 전환
                Attack();
            }
         }
        else
        {
            Transform childObject_1 = transform.Find("Ghost");
            Transform childObject_2 = transform.Find("GhostArmature");

            Transform effect = transform.Find("Holy hit");

            childObject_1.gameObject.SetActive(false);
            childObject_2.gameObject.SetActive(false);

            effect.gameObject.SetActive(true);
        }
    }

    void Move()
    {
        // 이동 애니메이션
        animator.SetBool("isAttack", false);

        // 플레이어를 바라보도록 회전
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rb.MoveRotation(rotation);

        // 플레이어 방향으로 이동
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        // 공격 애니메이션
        animator.SetBool("isAttack", true);

        // 투사체 발사
        if (Time.time >= nextFireTime)
        {
            transform.LookAt(player);
            Vector3 direction = player.position - firePoint.position;
            direction.Normalize();
            GameObject bullet = Instantiate(projectile, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed; // 투사체 속도
            nextFireTime = Time.time + 1f / fireRate; // 다음 발사 시간 설정
        }
    }

 

    public void DieAnimationComplete()
    {
        // 스크립트를 비활성화하여 이동 및 회전 멈춤
        enabled = false;
    }


}
