using System.Collections;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float speed = 5.0f; // �̵� �ӵ�
    public float rotationSpeed = 5.0f; // ȸ�� �ӵ�

    public GameObject atk0Prefab; // �⺻ ���� ������
    public GameObject atk1Prefab; // Z ���� ������
    public GameObject atk2Prefab; // X ���� ������
    public GameObject atk3Prefab; // C ���� ������

    public float attackRange = 3.0f; // �⺻ ���� ��Ÿ�

    private Transform player; 
    private bool isAttacking = false; // ���� ������ ����
    private bool isControlled = false; //C�� ���� ����
    private Animator animator;
    private Rigidbody rb;
    private float idleStartTime; 
    private float idleTimeToReattack = 2.0f; 
    private bool IsIdle = false; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange) // ��簡 ��Ÿ� ���� ���� ��
        {
            if (!isAttacking)
            {
                StartCoroutine(BasicAttack()); // ����
            }
        }
        else
        {
            IsIdle = false;
            if (!isAttacking) // ��Ÿ� �ۿ� ���� �� & ���� �� �ƴ� ��
            {
                MoveTowardsPlayer(); //��縦 ���� �̵�
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            StartCoroutine(Z_Attack());
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            StartCoroutine(X_Attack());
        }

        if (Input.GetKeyDown(KeyCode.C) && !isAttacking)
        {
            StartCoroutine(C_Attack());
        }
    }

    private void MoveTowardsPlayer() //��縦 ���� �̵�
    {
        if (IsIdle) return; // idle ���¿����� �̵����� ����

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // ȸ��
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // �̵�
        Vector3 moveDirection = direction * speed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);

        ResetAllTriggers(); 
        animator.SetBool("IsWalk", true); // Walk �ִϸ��̼� ����
    }



    private void HandleIdleState(float distanceToPlayer) //�⺻ ���ݰ� idle���� ��ȯ
    {
        if (Time.time - idleStartTime > idleTimeToReattack) //idle ���ӽð� ���� ��
        {
           
            if (distanceToPlayer <= attackRange) //��Ÿ� �ȿ� ��簡 ������
            {
                StartCoroutine(BasicAttack()); //�⺻ ����
            }
        }
    }

    IEnumerator BasicAttack() //�⺻ ����
    {
        isAttacking = true;

        while (true)
        {
            //��縦 ���� ȸ��
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(rb.rotation, lookRotation) < 5.0f)
            {
                break;
            }

            yield return null;
        }

        //�⺻ ���� �ִϸ��̼� ����
        ResetAllTriggers();
        animator.SetBool("IsIdle", false);
        animator.SetBool("ATK0", true);

        // �ӵ� 0, �̵� x
        float originalSpeed = speed;
        speed = 0; 

        // ���� ��� ���� �ִϸ��̼��� ���̸� ���
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // ��� ��ġ�� ���� ������ ����
        Vector3 attackPosition = player.position;
        Instantiate(atk0Prefab, attackPosition, Quaternion.identity);

        // �ִϸ��̼��� ���� ������ ���
        yield return new WaitForSeconds(animationLength / 2);
        animator.SetBool("ATK0", false);

        // ������ ���� �� idle ���·� ��ȯ
        IsIdle = true;
        idleStartTime = Time.time;

        ResetAllTriggers();
        animator.SetBool("IsIdle", true);

        //�̵� �ӵ� ����
        speed = originalSpeed;

        isAttacking = false;
    }

    IEnumerator Z_Attack() //����
    {
        isAttacking = true;

        ResetAllTriggers();
        animator.SetTrigger("ATK1"); //Z ���� �ִϸ��̼� ����

        GameObject atk1 = transform.Find("ATK1").gameObject;
        atk1.SetActive(true);

        yield return new WaitForSeconds(3.0f); //3�� ���

        atk1.SetActive(false);

        animator.ResetTrigger("ATK1");

        isAttacking = false;
    }

    IEnumerator X_Attack()
    {
        isAttacking = true;

        ResetAllTriggers();
        animator.SetTrigger("ATK2"); //X ���� �ִϸ��̼� ����

        //�̵� �� ȸ�� ����
        float originalSpeed = speed;
        float originalRotationSpeed = rotationSpeed;
        speed = 0;
        rotationSpeed = 0;

        //����� Creature�±��� ������Ʈ 10�� ã��
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        if (creatures.Length == 0) yield break;

        Vector3 currentPosition = transform.position;
        var closestCreatures = creatures
            .OrderBy(creature => Vector3.Distance(creature.transform.position, currentPosition))
            .Take(10)
            .ToList();

        //ã�� ������Ʈ���� ���� ���������� ����
        foreach (GameObject closestCreature in closestCreatures)
        {
            Vector3 position = closestCreature.transform.position;
            Quaternion rotation = closestCreature.transform.rotation;
            Destroy(closestCreature);
            Instantiate(atk2Prefab, position, rotation);
        }

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // �̵� �ӵ� ����
        speed = originalSpeed;
        rotationSpeed = originalRotationSpeed;

        animator.ResetTrigger("ATK2");

        isAttacking = false;
    }

    IEnumerator C_Attack()
    {
        isControlled = true;
        isAttacking = true;

        //�⺻ ���� ����
        float originalSpeed = speed;
        float originalRotationSpeed = rotationSpeed;

        //��Ʈ�� �� ���� ����
        float controlSpeed = speed * 2.0f;
        float controlRotationSpeed = 720.0f;

        float lastFireTime = Time.time;
        float controlDuration = 10.0f;
        float controlEndTime = Time.time + controlDuration;

        while (Time.time < controlEndTime) //���� �ð� ����
        {
            //����Ű ���
            float moveHorizontal = Input.GetAxis("Boss_Horizontal");
            float moveVertical = Input.GetAxis("Boss_Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            if (movement != Vector3.zero)
            {
                rb.MovePosition(rb.position + movement * controlSpeed * Time.deltaTime);

                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, controlRotationSpeed * Time.deltaTime);

                ResetAllTriggers();
                animator.SetTrigger("IsRun"); //Run �ִϸ��̼� ����

                if (Time.time - lastFireTime >= 0.1f) //�������� �ٴڿ� �� ����
                {
                    Vector3 firePosition = transform.position - transform.forward * 20f;
                    Instantiate(atk3Prefab, firePosition, Quaternion.identity);
                    lastFireTime = Time.time;
                }
            }
            else
            {
                ResetAllTriggers();
                animator.SetTrigger("IsIdle"); //idle ���·� ����
            }

            yield return null;
        }

        ResetAllTriggers();

        //�⺻ ���·� �ǵ���
        speed = originalSpeed;
        rotationSpeed = originalRotationSpeed;
        isControlled = false;
        isAttacking = false;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void ResetAllTriggers() //��� �ִϸ��̼� �ʱ�ȭ (walk ���·� ����)
    {
        animator.ResetTrigger("IsIdle");
        animator.ResetTrigger("IsRun");
        animator.ResetTrigger("ATK1");
        animator.ResetTrigger("ATK2");
        animator.ResetTrigger("ATK0");
    }
}
