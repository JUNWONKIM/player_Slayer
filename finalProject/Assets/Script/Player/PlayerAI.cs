using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    private Transform target; // 가장 가까운 적의 위치
    private Rigidbody rb; // 플레이어의 Rigidbody 컴포넌트
    private float timeSinceLastFind = 0f; // 마지막으로 적을 찾은 시간

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 0.5초마다 가장 가까운 적을 찾음
        timeSinceLastFind += Time.deltaTime;
        if (timeSinceLastFind >= 0.2f)
        {
            FindClosestEnemy();
            timeSinceLastFind = 0f;
        }

        MoveTowardsEnemy(); // 매 프레임마다 적의 반대 방향으로 이동
    }

    void MoveTowardsEnemy()
    {
        if (target != null)
        {
            // 적의 반대 방향으로 이동
            Vector3 moveDirection = (transform.position - target.position).normalized;
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);

            // 적 바라보기
            Vector3 lookAtDirection = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(lookAtDirection);
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            if (closestEnemy.transform != target) // 현재 타겟과 가장 가까운 적이 다를 때만 타겟을 변경
            {
                target = closestEnemy.transform;

                // 변경된 적을 바라보기
                Vector3 lookAtDirection = (target.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(lookAtDirection);
            }
        }
    }
}
