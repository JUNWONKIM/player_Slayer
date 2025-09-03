using UnityEngine;
using System.Collections;
public class Mummy : MonoBehaviour
{
   
    public float normalSpeed = 2f; // �̵� �ӵ�
    public float chaseSpeed = 5f; // ���� �ӵ�
    public float chaseRange = 10f; //���� ���� ����
    public float explodeRange = 1.5f; // ���� ���� ����
    public GameObject explosionPrefab; // ���� ����Ʈ ������
    public float chaseDuration = 3f; // ���� �ð�
    public float damageAmount = 1f; //������
    public float maxHealth = 1; // �ִ� ü��
    public float currentHealth; // ���� ü��
    public float chaseErrorRadius = 10f; //���� ���� ����

    private Rigidbody rb;
    private Transform player;
    private bool isChasing = false; //���� ����
    private bool hasDirectionSet = false; // ������ �����Ǿ����� ����
    private Vector3 chaseDirection; // ���� ����
    private float chaseTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        currentHealth = maxHealth; // ���� �� �ִ� ü������ ����

    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //���� ������ �� ����
        if (distanceToPlayer <= explodeRange)
        {           
            Explode();
        }

        //���� ���� ���� ��
        else if (!isChasing && distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            chaseTimer = chaseDuration;
            hasDirectionSet = false; // ���� �ʱ�ȭ
        }

        //���� ��
        if (isChasing)
        {
            chaseTimer -= Time.deltaTime;

            //���� ����
            if (!hasDirectionSet)
            {
                SetChaseDirection();
            }

            //���� �ð� �� ����
            if (chaseTimer <= 0f)
            {
                Explode();
            }
        }
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            MoveTowardsChaseDirection(chaseSpeed);
            RotateTowardsDirection(chaseDirection); // �����ϴ� ������ �ٶ󺸵��� ȸ��
        }
        else
        {
            Vector3 direction = (player.position - transform.position).normalized;
            MoveTowardsDirection(direction, normalSpeed);
            RotateTowardsDirection(direction); // �÷��̾ ���ϴ� ������ �ٶ󺸵��� ȸ��
        }
    }

    void MoveTowardsDirection(Vector3 direction, float speed) //��縦 ���� �̵�
    {
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    void MoveTowardsChaseDirection(float speed) //��縦 ���� ����
    {
        Vector3 newPosition = transform.position + chaseDirection * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    void RotateTowardsDirection(Vector3 direction) //��縦 ���� ȸ��
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        rb.MoveRotation(targetRotation);
    }

    void SetChaseDirection() //���� ���� ����
    {
        Vector3 randomDirection = Random.insideUnitSphere * chaseErrorRadius; // ���� �ݰ� ����
        randomDirection += player.position;
        randomDirection.y = transform.position.y; // y�� �̵� ����
        chaseDirection = (randomDirection - transform.position).normalized;
        hasDirectionSet = true; // ���� ���� �Ϸ�
    }

    void Explode() //���� ����
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation); //���� ����Ʈ ����
        PlayerLV.IncrementCreatureDeathCount(); //ũ���� ���� ī��Ʈ �߰�
        
        Destroy(gameObject); //�̶� ����
    }



    public void TakeDamage(float amount)
    {
        currentHealth -= amount; // ��������ŭ ü�� ����

        if (currentHealth <= 0)
        {
            Explode(); // ü���� 0�� �Ǹ� ����
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Explode(); // ���� �浹 �� ���� ����
        }
    }
}
