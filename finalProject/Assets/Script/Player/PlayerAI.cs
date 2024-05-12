using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float avoidanceDistance = 3f; // 총알을 피하는 거리
    public float bulletDetectionRange = 20f;
    private Transform target; // 가장 가까운 적의 위치
    private Transform nearestBullet; // 가장 가까운 총알의 위치
    private Rigidbody rb; // 플레이어의 Rigidbody 컴포넌트
    private float timeSinceLastFind = 0f; // 마지막으로 적을 찾은 시간
    private float timeSinceLastBulletFind = 0f; // 마지막으로 총알을 찾은 시간

    private LineRenderer detectionRangeVisual; // 적 탐지 범위를 시각적으로 표시할 라인 렌더러

    private enum PlayerState
    {
        MoveTowardsCreature,
        AvoidBullet,
        MoveAwayFromCreature
    }

    private PlayerState currentState = PlayerState.MoveTowardsCreature;
    private float stateChangeTime = 0f;
    private float stateChangeDuration = 0.5f; // 상태 변경 유지 시간

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        detectionRangeVisual = gameObject.AddComponent<LineRenderer>();

        // 라인 렌더러 설정
        detectionRangeVisual.material = new Material(Shader.Find("Sprites/Default"));
        detectionRangeVisual.startColor = Color.green;
        detectionRangeVisual.endColor = Color.green;
        detectionRangeVisual.startWidth = 0.1f;
        detectionRangeVisual.endWidth = 0.1f;
        detectionRangeVisual.positionCount = 37; // 원의 꼭지점 개수
    }

    void Update()
    {
        // 0.2초마다 가장 가까운 creature을 찾음
        timeSinceLastFind += Time.deltaTime;
        if (timeSinceLastFind >= 0.2f)
        {
            FindClosestCreature();
            timeSinceLastFind = 0f;
        }

        // 0.2초마다 가장 가까운 총알을 찾음
        timeSinceLastBulletFind += Time.deltaTime;
        if (timeSinceLastBulletFind >= 0.2f)
        {
            FindClosestBullet();
            timeSinceLastBulletFind = 0f;
        }

        // 플레이어의 상태에 따라 다른 행동을 수행
        switch (currentState)
        {
            case PlayerState.MoveTowardsCreature:
                MoveTowardsCreature();
                break;
            case PlayerState.AvoidBullet:
                // 가장 가까운 creature과 총알 중에서 더 가까운 것을 피하도록 결정
                if (target != null && nearestBullet != null)
                {
                    float creatureDistance = Vector3.Distance(transform.position, target.position);
                    float bulletDistance = Vector3.Distance(transform.position, nearestBullet.position);
                    if (bulletDistance < creatureDistance)
                    {
                        AvoidBullet(nearestBullet.position);
                    }
                    else
                    {
                        MoveAwayFromCreature();
                    }
                }
                break;
            case PlayerState.MoveAwayFromCreature:
                MoveAwayFromCreature();
                break;
        }

        // 만약 가장 가까운 총알이 플레이어 주위 일정 범위에 들어왔을 때
        if (nearestBullet != null && currentState != PlayerState.AvoidBullet)
        {
            // 플레이어가 해당 총알을 피하는 동작을 수행하도록 상태 변경
            ChangeState(PlayerState.AvoidBullet);
        }

        // 현재 상태가 변경된 후 지정된 시간이 경과하면 상태를 이전으로 되돌림
        if (Time.time - stateChangeTime >= stateChangeDuration)
        {
            if (currentState == PlayerState.AvoidBullet)
            {
                // 총알을 피하는 상태일 때는 가장 가까운 creature이 가까이에 없으면 다시 creature을 피하는 상태로 변경
                if (target == null)
                {
                    ChangeState(PlayerState.MoveTowardsCreature);
                }
            }
            else if (currentState == PlayerState.MoveAwayFromCreature)
            {
                // creature을 피하는 상태일 때는 가장 가까운 creature이 가까이에 없으면 다시 creature을 피는 상태로 변경
                if (target == null)
                {
                    ChangeState(PlayerState.MoveTowardsCreature);
                }
            }
        }

        UpdateDetectionRange();
        Debug.Log("Current State: " + currentState);
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
        Vector3 bulletDirection = nearestBullet.position - nearestBullet.position - nearestBullet.GetComponent<Rigidbody>().velocity.normalized;

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

            // creature을 피하기 위해 후진하므로 시야를 creature에게 향하도록 회전하지 않음
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

    private void UpdateDetectionRange()
    {
        // 원 모양의 점 생성
        Vector3[] points = new Vector3[37];
        for (int i = 0; i < 37; i++)
        {
            float angle = i * Mathf.PI * 2f / 36f;
            points[i] = transform.position + new Vector3(Mathf.Sin(angle) * bulletDetectionRange, 0f, Mathf.Cos(angle) * bulletDetectionRange);
        }

        // 라인 렌더러에 점 설정
        detectionRangeVisual.SetPositions(points);
    }

    // 플레이어 상태를 변경하는 메서드
    private void ChangeState(PlayerState newState)
    {
        currentState = newState;
        stateChangeTime = Time.time;
    }
}
