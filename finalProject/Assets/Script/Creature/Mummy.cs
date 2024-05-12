using UnityEngine;

public class Mummy : MonoBehaviour
{
    public enum State
    {
        MovingTowardsPlayer,
        ChargingTowardsPlayer,
        Retreating,
        Stopped // 새로 추가된 상태: 멈춤
    }

    public float moveSpeed = 5f; // 이동 속도
    public float chargingSpeed = 10f; // 돌진 속도
    public float chargingDistance = 5f; // 돌진 시작 거리
    public float retreatDistance = 2f; // 충돌 후 후퇴 거리
    public float retreatDuration = 2f; // 후퇴 지속 시간
    private Transform player; // 플레이어의 위치
    private Rigidbody rb; // 몬스터의 Rigidbody 컴포넌트
    private State currentState = State.MovingTowardsPlayer; // 현재 상태
    private Vector3 retreatStartPosition; // 후퇴 시작 위치
    private float retreatStartTime; // 후퇴 시작 시간

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 위치 찾기
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
    }

    void Update()
    {
        if (player != null)
        {
            // 플레이어와 몬스터 간의 거리 계산
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            switch (currentState)
            {
                case State.MovingTowardsPlayer:
                    if (distanceToPlayer <= chargingDistance)
                    {
                        currentState = State.ChargingTowardsPlayer;
                    }
                    else
                    {
                        MoveTowardsPlayer();
                    }
                    break;
                case State.ChargingTowardsPlayer:
                    if (distanceToPlayer >= retreatDistance)
                    {
                        currentState = State.Stopped; // 충돌하지 않고 플레이어에게 도달하면 멈춤
                    }
                    else
                    {
                        ChargeTowardsPlayer();
                    }
                    break;
                case State.Retreating:
                    if (Time.time - retreatStartTime >= retreatDuration)
                    {
                        currentState = State.MovingTowardsPlayer;
                    }
                    else
                    {
                        Retreat();
                    }
                    break;
            }
        }
    }

    void MoveTowardsPlayer()
    {
        // 플레이어를 향해 바라보도록 회전
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rb.MoveRotation(rotation);

        // 플레이어 방향으로 이동
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void ChargeTowardsPlayer()
    {
        // 플레이어를 향해 바라보도록 회전
        Vector3 lookDirection = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(lookDirection);
        rb.MoveRotation(rotation);

        // 플레이어 방향으로 돌진
        rb.MovePosition(transform.position + transform.forward * chargingSpeed * Time.deltaTime);
    }

    void Retreat()
    {
        // 후퇴 시작 위치로 이동
        rb.MovePosition(Vector3.Lerp(retreatStartPosition, transform.position, (Time.time - retreatStartTime) / retreatDuration));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 충돌 시 돌진 상태 종료 및 후퇴 상태로 변경
            currentState = State.Retreating;
            retreatStartPosition = transform.position;
            retreatStartTime = Time.time;
        }
    }
}
