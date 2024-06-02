using System.Collections;
using UnityEngine;

public class Player_Shooter_3 : MonoBehaviour
{
    public static Player_Shooter_3 instance;

    public GameObject swordPrefab; // 칼 프리팹
    public float summonInterval = 5f; // 칼 소환 간격
    public float distanceFromPlayer = 10f; // 플레이어로부터의 거리
    public float damageAmount = 0f;
    public int swordNum = 0;
    public float orbitSpeed = 50f; // 칼의 회전 속도

    private bool isSlowed = false; // Slow 상태 여부
    public float summonIntervalSlowMultiplier = 2f; // Slow 효과 시 발사 간격 배수

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 일정 간격마다 칼을 소환하는 코루틴 시작
        InvokeRepeating("SummonSword", 0f, summonInterval);
    }

    void Update()
    {
        CheckForSlowObjects();
    }

    void SummonSword()
    {
        // 플레이어 게임오브젝트 가져오기
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (swordNum == 1)
            {
                // 플레이어 위치와 칼의 소환 위치 계산
                Vector3 summonPosition = player.transform.position + new Vector3(distanceFromPlayer, 0f, 0f);
                Quaternion summonRotation = Quaternion.Euler(90f, 0f, 0f);

                // 칼 소환
                GameObject sword = Instantiate(swordPrefab, summonPosition, summonRotation);
                sword.AddComponent<SwordOrbit>().Initialize(player.transform, distanceFromPlayer, 1, 0, orbitSpeed);

                // 충돌 처리 컴포넌트를 동적으로 추가
                BulletCollisionHandler collisionHandler = sword.AddComponent<BulletCollisionHandler>();
                collisionHandler.damageAmount = damageAmount;

                // 3초 후에 칼 파괴
                Destroy(sword, 3f);
            }
            else
            {
                // 원의 반지름 계산
                float radius = distanceFromPlayer;

                for (int i = 0; i < swordNum; i++)
                {
                    // 각 칼의 각도 계산
                    float angle = i * (360f / swordNum);

                    // 칼의 소환 위치 계산
                    float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    Vector3 summonPosition = player.transform.position + new Vector3(x, 0f, z);
                    Quaternion summonRotation = Quaternion.Euler(90f, 0f, angle);

                    // 칼 소환
                    GameObject sword = Instantiate(swordPrefab, summonPosition, summonRotation);
                    sword.AddComponent<SwordOrbit>().Initialize(player.transform, distanceFromPlayer, swordNum, i, orbitSpeed);

                    // 충돌 처리 컴포넌트를 동적으로 추가
                    BulletCollisionHandler collisionHandler = sword.AddComponent<BulletCollisionHandler>();
                    collisionHandler.damageAmount = damageAmount;

                    // 3초 후에 칼 파괴
                    Destroy(sword, 3f);
                }
            }
        }
    }

    private void CheckForSlowObjects()
    {
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        if (slowObjects.Length > 0 && !isSlowed)
        {
            summonInterval *= summonIntervalSlowMultiplier; // 발사 간격을 두 배로 늘림
            isSlowed = true;
        }
        else if (slowObjects.Length == 0 && isSlowed)
        {
            summonInterval /= summonIntervalSlowMultiplier; // 발사 간격을 원래대로 돌림
            isSlowed = false;
        }
    }

    public void IncreaseSwordNum()
    {
        swordNum++;
        Debug.Log("칼 개수 : " + swordNum);
    }

    public void IncreaseDamage(float amount)
    {
        // Player_Shooter_4 클래스의 damageAmount 값을 변경
        damageAmount += amount;
        Debug.Log("칼 데미지 : " + damageAmount);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;

        void OnTriggerEnter(Collider other)
        {
            // 충돌한 객체의 태그가 "Creature"인 경우
            if (other.gameObject.CompareTag("Creature"))
            {
                // 충돌한 객체의 HP를 감소시킴
                CreatureHealth enemyHealth = other.gameObject.GetComponent<CreatureHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                }

                Mummy enemyHealth2 = other.gameObject.GetComponent<Mummy>();
                if (enemyHealth2 != null)
                {
                    enemyHealth2.TakeDamage(damageAmount);
                }
            }

           
        }
    }
}

public class SwordOrbit : MonoBehaviour
{
    private Transform playerTransform;
    private float orbitRadius;
    private int totalSwords;
    private int swordIndex;
    private float orbitSpeed;

    public void Initialize(Transform player, float radius, int swordCount, int index, float speed)
    {
        playerTransform = player;
        orbitRadius = radius;
        totalSwords = swordCount;
        swordIndex = index;
        orbitSpeed = speed;
    }

    void Start()
    {
        // 3초 후에 칼 파괴
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        // 각도를 증가시키면서 회전
        float angle = swordIndex * (360f / totalSwords) + (Time.time * orbitSpeed);
        float x = orbitRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = orbitRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

        // 새로운 위치 계산
        Vector3 newPosition = playerTransform.position + new Vector3(x, 0f, z);
        transform.position = newPosition;

        // 칼이 플레이어 반대 방향을 바라보도록 회전 설정
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-directionToPlayer);
        transform.rotation = lookRotation * Quaternion.Euler(90, 0, 0);
    }
}
