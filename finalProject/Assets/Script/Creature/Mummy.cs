using UnityEngine;

public class Mummy : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float normalSpeed = 2f; // 미라의 기본 이동 속도
    public float chaseSpeed = 5f; // 플레이어를 추격할 때의 속도
    public float chaseRange = 10f; // 플레이어를 추격하기 시작하는 범위
    public float explodeRange = 1.5f; // 폭발하는 범위
    public GameObject explosionPrefab; // 폭발 이펙트 프리팹
    public float chaseDuration = 3f; // 추격 지속 시간 (초)

    private Rigidbody rb;
    private bool isChasing = false;
    private bool hasDirectionSet = false; // 방향이 설정되었는지 여부
    private Vector3 chaseDirection; // 돌진할 때의 방향
    private float chaseTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= explodeRange)
        {
            // 플레이어와의 거리가 폭발 범위 안에 들어오면
            Explode();
        }
        else if (!isChasing && distanceToPlayer <= chaseRange)
        {
            // 플레이어와의 거리가 추격 범위 안에 들어오면
            isChasing = true;
            chaseTimer = chaseDuration;
            hasDirectionSet = false; // 방향 초기화
        }

        if (isChasing)
        {
            chaseTimer -= Time.deltaTime;

            if (!hasDirectionSet)
            {
                SetChaseDirection();
            }

            if (chaseTimer <= 0f)
            {
                // 추격 시작 후 3초가 지나면 폭발
                Explode();
            }
        }
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            MoveTowardsChaseDirection(chaseSpeed);
            RotateTowardsDirection(chaseDirection); // 돌진하는 방향을 바라보도록 회전
        }
        else
        {
            Vector3 direction = (player.position - transform.position).normalized;
            MoveTowardsDirection(direction, normalSpeed);
            RotateTowardsDirection(direction); // 플레이어를 향하는 방향을 바라보도록 회전
        }
    }

    void MoveTowardsDirection(Vector3 direction, float speed)
    {
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    void MoveTowardsChaseDirection(float speed)
    {
        Vector3 newPosition = transform.position + chaseDirection * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    void RotateTowardsDirection(Vector3 direction)
    {
        // 해당 방향을 바라보도록 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        rb.MoveRotation(targetRotation);
    }

    void SetChaseDirection()
    {
        // 플레이어의 위치를 기준으로 돌진할 랜덤한 방향 설정
        Vector3 randomDirection = Random.insideUnitSphere * 10f; // 2f는 오차 반경입니다.
        randomDirection += player.position;
        randomDirection.y = transform.position.y; // 수직 이동 방지
        chaseDirection = (randomDirection - transform.position).normalized;
        hasDirectionSet = true; // 방향이 설정되었음을 표시
    }

    void Explode()
    {
        // 폭발 효과 생성
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        PlayerLV.IncrementCreatureDeathCount();
        // 미라 제거
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 총알에 맞으면 폭발
            Explode();
        }
    }
}
