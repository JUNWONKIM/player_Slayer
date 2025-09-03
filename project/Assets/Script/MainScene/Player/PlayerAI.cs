using System.Collections;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public static PlayerAI instance; // �̱���

    public float moveSpeed = 100f; // �̵� �ӵ�
    public float slowSpeed = 2f; // ���ο� ���
    public bool isFreezed = false; // ���ο� ���� ����

    public float avoidanceDistance = 3f; // ����ü�� ���ϴ� �Ÿ�
    public float bulletDetectionRange = 20f; // ����ü ���� �Ÿ�

    private Transform target; // ���� ����� ���� ��ġ
    private Transform nearestBullet; // ���� ����� ����ü�� ��ġ
    private Rigidbody rb;
    private Animator animator; // �ִϸ����� �߰�

    private enum PlayerState // ��� ���� ����
    {
        MoveAwayFromCreature, // �����Լ� �־���
        AvoidBullet, // ����ü�� ����
        Idle // ��� ����
    }
    private PlayerState currentState = PlayerState.MoveAwayFromCreature;

    private float stateChangeTime = 0f;
    private float stateChangeDuration = 0.5f; // ���� ���� ���� �ð�

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        StartCoroutine(FindClosestObjectsCoroutine());
    }

    void Update()
    {
        // ���¿� ���� �ٸ� �ൿ�� ����
        switch (currentState)
        {
            case PlayerState.MoveAwayFromCreature:
                MoveAwayFromCreature();
                break;
            case PlayerState.AvoidBullet:
                if (nearestBullet != null)
                {
                    AvoidBullet(nearestBullet.position);
                }
                break;
            case PlayerState.Idle:
                IdleState();
                break;
        }

       
        if (nearestBullet == null && target == null) // ũ���ĳ� ������ ����, ����ü�� ���� ���
        {
            ChangeState(PlayerState.Idle); // Idle ���·� ��ȯ
        }
        else if (nearestBullet != null && Vector3.Distance(transform.position, nearestBullet.position) < bulletDetectionRange)
        {
            ChangeState(PlayerState.AvoidBullet); // ����ü�� ������ AvoidBullet ���·� ��ȯ
        }
        else if (target != null) // ũ���ĳ� ������ ������ MoveAwayFromCreature ���·� ��ȯ
        {
            ChangeState(PlayerState.MoveAwayFromCreature);
        }

        LookAtTarget(); // ����� ũ���ĸ� �ٶ�
        CheckForSlowObjects(); // ���ο� ����Ʈ�� �ִ��� Ȯ��
    }



    void IdleState() //idle ����
    {
        animator.SetBool("isIdle", true); // Idle �ִϸ��̼� ����
    }

    void LookAtTarget() // ũ���ĳ� ������ �ٶ�
    {
        if (target != null)
        {
            Vector3 directionToLook = (target.position - transform.position).normalized;
            directionToLook.y = 0; // y�� ȸ�� ����

            if (directionToLook != Vector3.zero) // ȸ���� ������ ���� ����
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToLook);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // �ε巯�� ȸ��
            }
        }
    }

    void MoveAwayFromCreature() //ũ��ó���Լ� �־���
    {
      
        animator.SetBool("isIdle", false); //walk �ִϸ��̼� ����


        //ũġ�����Լ� �ݴ�� �̵� & ����� ũ��ó�� �ٶ�
        if (target != null)
        {
            Vector3 moveDirection = (transform.position - target.position).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            Vector3 lookAtDirection = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookAtDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 10f)); 
        }
    }

    void AvoidBullet(Vector3 bulletPosition) //����ü ȸ�� 
    {
        
        animator.SetBool("isIdle", false); //walk �ִϸ��̼� ����

        //
        if (nearestBullet != null)
        {
            Vector3 directionToPlayer = transform.position - bulletPosition;
            Vector3 bulletDirection = nearestBullet.GetComponent<Rigidbody>().velocity.normalized;
            Vector3 perpendicular = Vector3.Cross(bulletDirection, Vector3.up).normalized; //����ü�� �������� ���� ����

            
            rb.MovePosition(rb.position + perpendicular * moveSpeed * Time.fixedDeltaTime); 
        }
    }

    private IEnumerator FindClosestObjectsCoroutine() //���� ����� ����, ũ����, ����ü�� ã��
    {
        while (true)
        {
            FindClosestCreatureOrBoss();  // ����� ũ���ĳ� ���� ã��
            FindClosestBullet();          // ����� ����ü ã��
            yield return new WaitForSeconds(0.2f); 
        }
    }

    void FindClosestCreatureOrBoss() //����� ũ���ĳ� ���� ã��
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature"); 
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;
        
        //�� ũ��ó�� ���Ͽ� ���� ����� �Ÿ��� ���� ũ��ó�� ã��
        foreach (GameObject creature in creatures)
        {
            float distance = Vector3.Distance(transform.position, creature.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = creature;
            }
        }

        //������ ã��
        foreach (GameObject boss in bosses)
        {
            float distance = Vector3.Distance(transform.position, boss.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = boss;
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget.transform;
        }
        else
        {
            target = null; 
        }
    }

    void FindClosestBullet() //���� ����� ����ü ã��
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("C_Bullet");
        float closestDistance = Mathf.Infinity;
        GameObject closestBullet = null;

        foreach (GameObject bullet in bullets)
        {
            float distance = Vector3.Distance(transform.position, bullet.transform.position);

            //������ ����ü�� ���� ���� ����� ����ü�� ã��
            if (distance < closestDistance && distance <= bulletDetectionRange)
            {
                closestDistance = distance;
                closestBullet = bullet;
            }
        }

        if (closestBullet != null)
        {
            nearestBullet = closestBullet.transform;
        }
        else
        {
            nearestBullet = null; 
        }
    }

    private void ChangeState(PlayerState newState) //���� ����
    {
        currentState = newState;
        stateChangeTime = Time.time;

      
        if (newState == PlayerState.Idle)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }

    private void CheckForSlowObjects() //������ ���ο� ����Ʈ ���� Ȯ��
    {
        GameObject[] freezeObjects = GameObject.FindGameObjectsWithTag("Freeze");

        //������ ��� �ӵ� ����
        if (freezeObjects.Length > 0 && !isFreezed)
        {
            moveSpeed /= slowSpeed;
            isFreezed = true;
        }

        //���� �� �ӵ� ����
        else if (freezeObjects.Length == 0 && isFreezed)
        {
            moveSpeed *= slowSpeed;
            isFreezed = false;
        }
    }

    public void IncreaseMoveSpeed(float amount) //�ӵ� ���� (������ �� �ʿ�)
    {
        moveSpeed *= amount;
    }
}
