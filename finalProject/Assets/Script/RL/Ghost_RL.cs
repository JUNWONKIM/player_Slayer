using UnityEngine;
using System.Collections;

public class Ghost_RL : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동속도
    public float stoppingDistance = 5f; // 공격 사거리
    public float bulletSpeed = 50f; // 투사체 속도
    public GameObject projectilePrefab; // 투사체 프리팹
    public Transform firePoint; // 발사 지점
    public float fireRate = 1f; // 발사 속도
    public float nextFireTime = 0f; // 발사 시간 기록
    public float damageAmount = 1f; // 데미지

    public Transform ownerAgent; // 외부에서 설정할 추적 대상
    private Transform player; // 내부 타겟 참조
    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true; // 데미지를 줄 수 있는 상태 여부

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 외부에서 설정된 ownerAgent가 있으면 그걸 추적, 없으면 태그로 찾기
        if (ownerAgent == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        else
            player = ownerAgent;
    }

    void Update()
    {
        if (player != null && !animator.GetBool("isDie"))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                Move();
            }
            else if (distanceToPlayer <= stoppingDistance)
            {
                Attack();
            }
        }
        else
        {
            Transform childObject_1 = transform.Find("Ghost");
            Transform childObject_2 = transform.Find("GhostArmature");
            Transform effect = transform.Find("Ghost_die");

            childObject_1?.gameObject.SetActive(false);
            childObject_2?.gameObject.SetActive(false);
            effect?.gameObject.SetActive(true);
        }
    }

    void Move()
    {
        animator.SetBool("isAttack", false);

        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rb.MoveRotation(rotation);

        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        animator.SetBool("isAttack", true);

        if (Time.time >= nextFireTime)
        {
            transform.LookAt(player);
            Vector3 direction = player.position - firePoint.position;
            direction.Normalize();
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
            Destroy(bullet, 3f);
            nextFireTime = Time.time + 1f / fireRate;

            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;
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
    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount = 1f;

        void Start()
        {
            Destroy(gameObject, 3f); // 3초 뒤 자동 제거
        }

        void OnTriggerEnter(Collider other)
        {
            AgentHp agentHp = other.GetComponent<AgentHp>();
            if (agentHp != null)
            {
                agentHp.TakeDamage(damageAmount);
                Destroy(gameObject); // 즉시 제거
            }
        }
    }
    }
