using UnityEngine;
using System.Collections;
using Unity.MLAgents; 

public class Ghost_RL : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float stoppingDistance = 5f;
    public float bulletSpeed = 50f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float nextFireTime = 0f;
    public float damageAmount = 1f;

    public Transform ownerAgent;
    public CreatureSpawner2 spawnerOwner; // 👈 추가

    private Transform player;
    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // 👇 커리큘럼 파라미터 가져오기
        float rate = Academy.Instance.EnvironmentParameters.GetWithDefault("ghostFireRate", fireRate);
        float speed = Academy.Instance.EnvironmentParameters.GetWithDefault("bulletSpeed", bulletSpeed);

        fireRate = rate;
        bulletSpeed = speed;

        player = ownerAgent != null ? ownerAgent : GameObject.FindGameObjectWithTag("Player").transform;
    }


    void Update()
    {
        if (player != null && !animator.GetBool("isDie"))
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
                Move();
            else
                Attack();
        }
        else
        {
            Transform ghost = transform.Find("Ghost");
            Transform armature = transform.Find("GhostArmature");
            Transform effect = transform.Find("Ghost_die");

            ghost?.gameObject.SetActive(false);
            armature?.gameObject.SetActive(false);
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
            Vector3 direction = (player.position - firePoint.position).normalized;

            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed;

            // ✅ 데미지 + 소유 정보 설정
            BulletCollisionHandler handler = bullet.AddComponent<BulletCollisionHandler>();
            handler.damageAmount = damageAmount;

            BulletInfo info = bullet.AddComponent<BulletInfo>();
            info.ownerAgent = this.ownerAgent;

            if (spawnerOwner != null)
                spawnerOwner.spawnedBullets.Add(bullet);

            Destroy(bullet, 3f);
            nextFireTime = Time.time + 1f / fireRate;
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
        if (other.CompareTag("Player") && canDealDamage && ownerAgent != null && other.transform == ownerAgent)
        {
            AgentHp agentHP = other.GetComponent<AgentHp>();
            if (agentHP != null)
            {
                agentHP.TakeDamage(damageAmount);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount = 1f;

        void OnTriggerEnter(Collider other)
        {
            AgentHp agentHp = other.GetComponent<AgentHp>();
            if (agentHp != null)
            {
                agentHp.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
    }
}

// 👇 별도 스크립트 파일로 추가 필요
public class BulletInfo : MonoBehaviour
{
    public Transform ownerAgent;
}
