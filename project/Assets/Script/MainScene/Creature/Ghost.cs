using UnityEngine;
using System.Collections;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵��ӵ�
    public float stoppingDistance = 5f; // ���� ��Ÿ�
    public float bulletSpeed = 50f; // ����ü �ӵ�
    public GameObject projectilePrefab; // ����ü ������
    public Transform firePoint; // �߻� ����
    public float fireRate = 1f; // �߻� �ӵ�
    public float nextFireTime = 0f; // �߻� �ð� ���
    public float damageAmount = 1f; // ������

    private Transform player;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform; // ��� ��ġ
    }

    void Update()
    {
        if (player != null && !animator.GetBool("isDie"))
        {
            // ������ �Ÿ� ���
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            //���� ��Ÿ����� �̵�
            if (distanceToPlayer > stoppingDistance)
            {
                Move();
            }

            //��Ÿ� ������ ���� �� ����
            else if (distanceToPlayer <= stoppingDistance)
            {
                Attack();
            }
        }

        //������ �״� ����Ʈ ������ Ȱ��ȭ & ������ ������ ��Ȱ��ȭ
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

    void Move() //�̵�
    {
        animator.SetBool("isAttack", false); // �̵� �ִϸ��̼�

        // ��縦 �ٶ󺸵��� ȸ��
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rb.MoveRotation(rotation);

        //���� �̵�
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        animator.SetBool("isAttack", true); // ���� �ִϸ��̼�

        // ����ü �߻�
        if (Time.time >= nextFireTime) // �߻� ���� �ð� Ȯ��
        {
            transform.LookAt(player); //��縦 �ٶ�
            Vector3 direction = player.position - firePoint.position; //�߻� ����
            direction.Normalize();
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity); // ����ü ����
            bullet.GetComponent<Rigidbody>().velocity = direction * bulletSpeed; // ����ü �ӵ�
            Destroy(bullet, 3f); // ���� �ð� �� ����ü �ı�
            nextFireTime = Time.time + 1f / fireRate; // ���� �߻� �ð� ����

            //����ü �浹 ó��
            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;
        }
    }

    public class BulletCollisionHandler : MonoBehaviour //����ü �浹ó��
    {
        public float damageAmount; //����ü ������
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
