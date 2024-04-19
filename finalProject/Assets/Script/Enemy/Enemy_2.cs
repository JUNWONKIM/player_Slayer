using UnityEngine;

public class Enemy_2 : MonoBehaviour
{
    public float moveSpeed = 5f; // 적의 이동 속도
    public float stoppingDistance = 5f; // 플레이어와의 멈추는 거리
    public float retreatDistance = 5f; // 플레이어로부터 후퇴하는 거리

    public GameObject projectile; // 발사할 투사체
    public Transform firePoint; // 발사 지점
    public float fireRate = 1f; // 발사 속도 (1초당 한 발)

    private Transform player; // 플레이어의 위치
    private float nextFireTime = 0f; // 다음 발사 시간

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 위치 찾기
    }

    void Update()
    {

        if (player != null)
        {
            // 플레이어를 향해 이동
            if (Vector3.Distance(transform.position, player.position) > stoppingDistance)
            {
                transform.LookAt(player);
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            // 플레이어와 일정 거리가 되면 멈추고 투사체 발사
            else if (Vector3.Distance(transform.position, player.position) <= stoppingDistance && Vector3.Distance(transform.position, player.position) > retreatDistance)
            {
                if (Time.time >= nextFireTime)
                {
                    // 플레이어를 향해 투사체 발사
                    transform.LookAt(player);
                    Vector3 direction = player.position - firePoint.position;
                    direction.Normalize();
                    GameObject bullet = Instantiate(projectile, firePoint.position, Quaternion.identity);
                    bullet.GetComponent<Rigidbody>().velocity = direction * 100f; // 투사체 속도
                    nextFireTime = Time.time + 1f / fireRate; // 다음 발사 시간 설정
                }
            }
            // 플레이어로부터 일정 거리 이상 떨어지면 후퇴
            else if (Vector3.Distance(transform.position, player.position) < retreatDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, -moveSpeed * Time.deltaTime);
            }
        }
    }
}
