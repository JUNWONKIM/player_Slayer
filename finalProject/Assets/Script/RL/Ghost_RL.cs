using UnityEngine;
using System.Collections;

public class Ghost_RL : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵��ӵ�
    public float stoppingDistance = 5f; // ���� ��Ÿ�
    public float bulletSpeed = 50f; // ����ü �ӵ�
    public GameObject projectilePrefab; // ����ü ������
    public Transform firePoint; // �߻� ����
    public float fireRate = 1f; // �߻� �ӵ�
    public float nextFireTime = 0f; // �߻� �ð� ���
    public float damageAmount = 1f; // ������

    public Transform ownerAgent; // �ܺο��� ������ ���� ���
    private Transform player; // ���� Ÿ�� ����
    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true; // �������� �� �� �ִ� ���� ����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // �ܺο��� ������ ownerAgent�� ������ �װ� ����, ������ �±׷� ã��
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
    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount = 1f;

        void Start()
        {
            Destroy(gameObject, 3f); // 3�� �� �ڵ� ����
        }

        void OnTriggerEnter(Collider other)
        {
            AgentHp agentHp = other.GetComponent<AgentHp>();
            if (agentHp != null)
            {
                agentHp.TakeDamage(damageAmount);
                Destroy(gameObject); // ��� ����
            }
        }
    }
    }
