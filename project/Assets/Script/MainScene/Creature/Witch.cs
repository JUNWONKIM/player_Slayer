using UnityEngine;
using System.Collections;

public class Witch : MonoBehaviour
{
    public float moveSpeed = 3.0f;  // �̵� �ӵ�
    public float stopDistance = 2.0f;  // ���� ��Ÿ�
    public GameObject attackParticlePrefab;  // ���� ��ƼŬ ������
    public float attackCooldown = 5.0f;  // ���� ��Ÿ��

    private Transform player;
    private Rigidbody rb;
    private Animator animator;
    private bool isAttacking = false; //���� ���� ����
    private float lastAttackTime; //������ ���� �ð� ����
    private bool initialAttack = true;  // ù ���� ����
    private float distanceToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (animator.GetBool("isDie")) return; //���� ��� ����

        if (player != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > stopDistance && !isAttacking) //��Ÿ� ���� ��� & ���� ������ ������
            {
                MoveTowardsPlayer(); //��縦 ���� ������
            }
            else if (distanceToPlayer <= stopDistance) //��Ÿ� ���̸�
            {
                rb.velocity = Vector3.zero; //�ӵ��� 0���� ����
                LookAtPlayer();  // ��縦 ���� ȸ��

                if (!isAttacking && (initialAttack || Time.time >= lastAttackTime + attackCooldown)) //��Ÿ���� ������ ���
                {
                    Attack();  // ���� ����
                }
                else if (!isAttacking)
                {
                    animator.SetBool("isIdle", true);  //��Ÿ���� ��� idle �ִϸ��̼� ����
                }
            }
        }
    }

    void MoveTowardsPlayer() //��縦 ���� �̵�
    {
        animator.SetBool("isIdle", false); //�̵� �ִϸ��̼� ����

        //���� �̵�
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);
        LookAtPlayer();

    }

    void LookAtPlayer() //��縦 ���� ȸ��
    {
        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;  // y �� ȸ�� ����
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10.0f);
    }

    void Attack() //����
    {
        isAttacking = true;

        // ���� �ִϸ��̼� ����
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", true);

        if (!ParticleExists())
        {
            Instantiate(attackParticlePrefab, player.position, Quaternion.identity);  // ���� ��ƼŬ ����
        }
        lastAttackTime = Time.time;  // ������ ���� �ð��� ����
        initialAttack = false;  // ù ���� �� false�� ����
        Invoke("ResetAttack", 1.0f);  // �ִϸ��̼��� ���� �� isAttacking ���¸� ����
    }

    bool ParticleExists() //��ƼŬ ���� ���� Ȯ��
    {
        return GameObject.FindWithTag(attackParticlePrefab.tag) != null;
    }

    void ResetAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttack", false);  // ���� �ִϸ��̼� ����

        if (distanceToPlayer <= stopDistance)
        {
            animator.SetBool("isIdle", true);  // ��Ÿ�� ���� �� idle �ִϸ��̼� Ȱ��ȭ
        }
    }


}
