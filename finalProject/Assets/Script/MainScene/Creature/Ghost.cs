using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동속도
    public float stoppingDistance = 5f; // 공격 사거리
    public float bulletSpeed = 50f; // 투사체 속도
    public GameObject projectilePrefab; // 투사체 프리팹
    public Transform firePoint; // 발사 지점
    public float fireRate = 1f; // 발사 속도
    public float nextFireTime = 0f; // 발사 시간 기록
    public float damageAmount = 1f; // 데미지

    private Transform player;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform; // 용사 위치
    }

    void Update()
    {
        if (player != null && !animator.GetBool("isDie"))
        {
            // 용사와의 거리 계산
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            //공격 사거리까지 이동
            if (distanceToPlayer > stoppingDistance)
            {
                Move();
            }

            //사거리 안으로 접근 시 공격
            else if (distanceToPlayer <= stoppingDistance)
            {
                Attack();
            }
        }

        //죽으면 죽는 이펙트 프리팹 활성화 & 나머지 프리팹 비활성화
        else
        {
            Transform childObject_1 = transform.Find("Ghost");
            Transform childObject_2 = transform.Find("GhostArmature");

            Transform effect = transform.Find("Ghost_die");

            childObject_1.gameObject.SetActive(false);
            childObject_2.gameObject.SetActive(false);

            effect.gameObject.SetActive(true);
        }
    }

    void Move() //이동
    {
        animator.SetBool("isAttack", false); // 이동 애니메이션

        // 용사를 바라보도록 회전
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rb.MoveRotation(rotation);

        //용사로 이동
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        animator.SetBool("isAttack", true); // 공격 애니메이션

        // 투사체 발사
        if (Time.time >= nextFireTime) // 발사 가능 시간 확인
        {
            transform.LookAt(player); //용사를 바라봄
            Vector3 direction = player.position - firePoint.position; //발사 방향
            direction.Normalize();
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity); // 투사체 생성
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed; // 투사체 속도
            Destroy(bullet, 3f); // 일정 시간 뒤 투사체 파괴
            nextFireTime = Time.time + 1f / fireRate; // 다음 발사 시간 설정

            //투사체 충돌 처리
            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;
        }
    }

    public class BulletCollisionHandler : MonoBehaviour //투사체 충돌처리
    {
        public float damageAmount; //투사체 데미지
        void OnTriggerEnter(Collider other)
        {
            PlayerHP playerHP = other.gameObject.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.hp -= damageAmount;
                Destroy(gameObject);
            }
        }
    }
}
