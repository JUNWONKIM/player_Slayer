using System.Collections;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public static PlayerAI instance;

    public float moveSpeed = 100f; // 이동 속도
    public float slowSpeed = 2f;
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
        StartCoroutine(FindClosestCreatureCoroutine());
        StartCoroutine(FindClosestBulletCoroutine());
    }

    void Update()
    {
        // 플레이어의 상태에 따라 다른 행동을 수행
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

        // 만약 가장 가까운 총알이 플레이어 주위 일정 범위에 들어왔을 때
        if (nearestBullet != null && currentState != PlayerState.AvoidBullet && Vector3.Distance(transform.position, nearestBullet.position) < bulletDetectionRange)
        {
            ChangeState(PlayerState.AvoidBullet);
        }

        // 현재 상태가 변경된 후 지정된 시간이 경과하면 상태를 이전으로 되돌림
        if (Time.time - stateChangeTime >= stateChangeDuration)
        {
            if (currentState == PlayerState.AvoidBullet && nearestBullet == null)
            {
                ChangeState(PlayerState.MoveTowardsCreature);
            }
            else if (currentState == PlayerState.MoveAwayFromCreature && target == null)
            {
                ChangeState(PlayerState.MoveTowardsCreature);
            }
        }

        CheckForSlowObjects();
    }

    void MoveTowardsCreature()
    {
        if (target != null)
        {
            // creature의 반대 방향으로 이동
            Vector3 moveDirection = (transform.position - target.position).normalized;
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);

            // creature 바라보기
            Vector3 lookAtDirection = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(lookAtDirection);
        }
    }

    void AvoidBullet(Vector3 bulletPosition)
    {
        // 총알의 위치에서 플레이어의 위치를 뺀 방향 벡터
        Vector3 directionToPlayer = transform.position - bulletPosition;

        // 총알의 이동 방향 벡터
        Vector3 bulletDirection = bulletPosition - nearestBullet.GetComponent<Rigidbody>().velocity.normalized;

        // 총알의 방향과 수직인 벡터 계산 (y 축은 무시)
        Vector3 perpendicular = new Vector3(bulletDirection.z, 0f, -bulletDirection.x).normalized;

        // 플레이어를 해당 방향으로 이동
        rb.MovePosition(transform.position + perpendicular * moveSpeed * Time.deltaTime);
    }

    void MoveAwayFromCreature()
    {
        if (target != null)
        {
            // creature의 반대 방향으로 이동
            Vector3 moveDirection = (transform.position - target.position).normalized;
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator FindClosestCreatureCoroutine()
    {
        while (true)
        {
            FindClosestCreature();
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

    void FindClosestCreature()
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        float closestDistance = Mathf.Infinity;
        GameObject closestCreature = null;

        foreach (GameObject creature in creatures)
        {
            float distance = Vector3.Distance(transform.position, creature.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCreature = creature;
            }
        }

        if (closestCreature != null)
        {
            if (closestCreature.transform != target) // 현재 타겟과 가장 가까운 creature이 다를 때만 타겟을 변경
            {
                target = closestCreature.transform;

                // 변경된 creature을 바라보기
                Vector3 lookAtDirection = (target.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(lookAtDirection);
            }
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
            moveSpeed /= slowSpeed; // 발사 간격을 두 배로 늘림
            isFreezed = true;
        }
        else if (freezeObjects.Length == 0 && isFreezed)
        {
            moveSpeed *= slowSpeed; // 발사 간격을 두 배로 늘림
            isFreezed = false;
        }
    }


    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed *= amount;
        Debug.Log("Move speed increased to: " + moveSpeed);
    }
}
