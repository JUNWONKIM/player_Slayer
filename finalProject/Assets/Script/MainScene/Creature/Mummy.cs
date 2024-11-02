using UnityEngine;
using System.Collections;
public class Mummy : MonoBehaviour
{
   
    public float normalSpeed = 2f; // 이동 속도
    public float chaseSpeed = 5f; // 돌진 속도
    public float chaseRange = 10f; //돌진 시작 범위
    public float explodeRange = 1.5f; // 폭발 시작 범위
    public GameObject explosionPrefab; // 폭발 이펙트 프리팹
    public float chaseDuration = 3f; // 돌진 시간
    public float damageAmount = 1f; //데미지
    public float maxHealth = 1; // 최대 체력
    public float currentHealth; // 현재 체력
    public float chaseErrorRadius = 10f; //돌진 오차 범위

    private Rigidbody rb;
    private Transform player;
    private bool isChasing = false; //돌진 여부
    private bool hasDirectionSet = false; // 방향이 설정되었는지 여부
    private Vector3 chaseDirection; // 돌진 방향
    private float chaseTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        currentHealth = maxHealth; // 시작 시 최대 체력으로 설정

    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //폭발 범위일 시 폭발
        if (distanceToPlayer <= explodeRange)
        {           
            Explode();
        }

        //돌진 범위 안일 시
        else if (!isChasing && distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            chaseTimer = chaseDuration;
            hasDirectionSet = false; // 방향 초기화
        }

        //돌진 시
        if (isChasing)
        {
            chaseTimer -= Time.deltaTime;

            //방향 설정
            if (!hasDirectionSet)
            {
                SetChaseDirection();
            }

            //돌진 시간 뒤 폭발
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
            RotateTowardsDirection(chaseDirection); // 돌진하는 방향을 바라보도록 회전
        }
        else
        {
            Vector3 direction = (player.position - transform.position).normalized;
            MoveTowardsDirection(direction, normalSpeed);
            RotateTowardsDirection(direction); // 플레이어를 향하는 방향을 바라보도록 회전
        }
    }

    void MoveTowardsDirection(Vector3 direction, float speed) //용사를 향해 이동
    {
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    void MoveTowardsChaseDirection(float speed) //용사를 향해 돌진
    {
        Vector3 newPosition = transform.position + chaseDirection * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    void RotateTowardsDirection(Vector3 direction) //용사를 향해 회전
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        rb.MoveRotation(targetRotation);
    }

    void SetChaseDirection() //돌진 방향 설정
    {
        Vector3 randomDirection = Random.insideUnitSphere * chaseErrorRadius; // 오차 반경 설정
        randomDirection += player.position;
        randomDirection.y = transform.position.y; // y축 이동 금지
        chaseDirection = (randomDirection - transform.position).normalized;
        hasDirectionSet = true; // 방향 설정 완료
    }

    void Explode() //폭발 실행
    {
        Instantiate(explosionPrefab, transform.position, transform.rotation); //폭발 이펙트 생성
        PlayerLV.IncrementCreatureDeathCount(); //크리쳐 데스 카운트 추가
        
        Destroy(gameObject); //미라 제거
    }



    public void TakeDamage(float amount)
    {
        currentHealth -= amount; // 데미지만큼 체력 감소

        if (currentHealth <= 0)
        {
            Explode(); // 체력이 0이 되면 폭발
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Explode(); // 용사와 충돌 시 폭발 실행
        }
    }
}
