using System.Collections;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public static PlayerAI instance;

    public float moveSpeed = 100f; // 이동 속도
    public float slowSpeed = 2f; // Slow 효과 배수
    public bool isFreezed = false;

    public float avoidanceDistance = 3f; // 총알을 피하는 거리
    public float bulletDetectionRange = 20f;
    private Transform target; // 가장 가까운 적의 위치
    private Transform nearestBullet; // 가장 가까운 총알의 위치
    private Rigidbody rb; // 플레이어의 Rigidbody 컴포넌트

    private enum PlayerState
    {
        MoveTowardsCreature,
        AvoidBullet,
        MoveAwayFromCreature
    }

    private PlayerState currentState = PlayerState.MoveTowardsCreature;
    private float stateChangeTime = 0f;
    private float stateChangeDuration = 0.5f; // 상태 변경 유지 시간

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(FindClosestCreatureOrBossCoroutine());
        StartCoroutine(FindClosestBulletCoroutine());
    }

    void FixedUpdate()
    {
        // 상태에 따라 다른 행동을 수행
        switch (currentState)
        {
            case PlayerState.MoveTowardsCreature:
                MoveTowardsCreature();
                break;
            case PlayerState.AvoidBullet:
                if (nearestBullet != null)
                {
                    AvoidBullet(nearestBullet.position);
                }
                break;
            case PlayerState.MoveAwayFromCreature:
                MoveAwayFromCreature();
                break;
        }

        // 총알이 가까이 있을 때
        if (nearestBullet != null && currentState != PlayerState.AvoidBullet && Vector3.Distance(transform.position, nearestBullet.position) < bulletDetectionRange)
        {
            ChangeState(PlayerState.AvoidBullet);
        }

        // 상태 유지 시간 체크
        if (Time.time - stateChangeTime >= stateChangeDuration)
        {
            if (currentState == PlayerState.AvoidBullet && nearestBullet == null)
            {
                ChangeState(PlayerState.MoveAwayFromCreature);
            }
            else if (currentState == PlayerState.MoveAwayFromCreature && target == null)
            {
                ChangeState(PlayerState.MoveTowardsCreature);
            }
        }

        // 적을 항상 바라보게 설정
        LookAtTarget();

        CheckForSlowObjects();
    }

    void LookAtTarget()
    {
        if (target != null)
        {
            // 적을 바라보는 로직
            Vector3 lookAtDirection = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookAtDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 10f)); // 부드럽게 회전
        }
    }

    void MoveTowardsCreature()
    {
        if (target != null)
        {
            Vector3 moveDirection = (transform.position - target.position).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            Vector3 lookAtDirection = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookAtDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 10f)); // 속도 조절을 위한 값
        }
    }

    void AvoidBullet(Vector3 bulletPosition)
    {
        if (nearestBullet != null)
        {
            Vector3 directionToPlayer = transform.position - bulletPosition;
            Vector3 bulletDirection = nearestBullet.GetComponent<Rigidbody>().velocity.normalized;
            Vector3 perpendicular = Vector3.Cross(bulletDirection, Vector3.up).normalized;

            // 적절한 방향으로 이동
            rb.MovePosition(rb.position + perpendicular * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void MoveAwayFromCreature()
    {
        if (target != null)
        {
            Vector3 moveDirection = (transform.position - target.position).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator FindClosestCreatureOrBossCoroutine()
    {
        while (true)
        {
            FindClosestCreatureOrBoss();
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator FindClosestBulletCoroutine()
    {
        while (true)
        {
            FindClosestBullet();
            yield return new WaitForSeconds(0.2f);
        }
    }

    void FindClosestCreatureOrBoss()
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;

        foreach (GameObject creature in creatures)
        {
            float distance = Vector3.Distance(transform.position, creature.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = creature;
            }
        }

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
            if (closestTarget.transform != target)
            {
                target = closestTarget.transform;

                Vector3 lookAtDirection = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(lookAtDirection);
                rb.MoveRotation(lookRotation);
            }
        }
        else
        {
            target = null;
        }
    }

    void FindClosestBullet()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("C_Bullet");
        float closestDistance = Mathf.Infinity;
        GameObject closestBullet = null;

        foreach (GameObject bullet in bullets)
        {
            float distance = Vector3.Distance(transform.position, bullet.transform.position);
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

    private void ChangeState(PlayerState newState)
    {
        currentState = newState;
        stateChangeTime = Time.time;
    }

    private void CheckForSlowObjects()
    {
        GameObject[] freezeObjects = GameObject.FindGameObjectsWithTag("Freeze");

        if (freezeObjects.Length > 0 && !isFreezed)
        {
            moveSpeed /= slowSpeed;
            isFreezed = true;
        }
        else if (freezeObjects.Length == 0 && isFreezed)
        {
            moveSpeed *= slowSpeed;
            isFreezed = false;
        }
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed *= amount;
        Debug.Log("Move speed increased to: " + moveSpeed);
    }
}
