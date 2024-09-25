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

    public AudioClip summonSound; // 칼 소환 시 재생할 사운드 클립
    private AudioSource audioSource; // AudioSource 변수 추가

    // 사운드 속도와 볼륨 설정 변수
    [Range(0.1f, 3f)] public float summonSoundPitch = 1f; // 사운드의 속도 (피치)
    [Range(0f, 1f)] public float summonSoundVolume = 1f; // 사운드의 볼륨

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
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
                Vector3 summonPosition = player.transform.position + new Vector3(distanceFromPlayer, 0f, 0f);
                Quaternion summonRotation = Quaternion.Euler(90f, 0f, 0f);

                // 칼 소환
                GameObject sword = Instantiate(swordPrefab, summonPosition, summonRotation);
                sword.AddComponent<SwordOrbit>().Initialize(player.transform, distanceFromPlayer, 1, 0, orbitSpeed);

                BulletCollisionHandler collisionHandler = sword.AddComponent<BulletCollisionHandler>();
                collisionHandler.damageAmount = damageAmount;

                // 칼 소환 사운드 재생
                StartCoroutine(PlaySummonSoundTwice());

                Destroy(sword, 3f);
            }
            else
            {
                float radius = distanceFromPlayer;

                for (int i = 0; i < swordNum; i++)
                {
                    float angle = i * (360f / swordNum);

                    float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    Vector3 summonPosition = player.transform.position + new Vector3(x, 0f, z);
                    Quaternion summonRotation = Quaternion.Euler(90f, 0f, angle);

                    GameObject sword = Instantiate(swordPrefab, summonPosition, summonRotation);
                    sword.AddComponent<SwordOrbit>().Initialize(player.transform, distanceFromPlayer, swordNum, i, orbitSpeed);

                    BulletCollisionHandler collisionHandler = sword.AddComponent<BulletCollisionHandler>();
                    collisionHandler.damageAmount = damageAmount;

                    // 칼 소환 사운드 재생
                    StartCoroutine(PlaySummonSoundTwice());

                    Destroy(sword, 3f);
                }
            }
        }
    }

    IEnumerator PlaySummonSoundTwice()
    {
        if (audioSource != null && summonSound != null)
        {
            // 첫 번째 재생
            audioSource.clip = summonSound;
            audioSource.pitch = summonSoundPitch;
            audioSource.volume = summonSoundVolume;
            audioSource.Play();

            // 첫 번째 사운드 재생이 끝날 때까지 대기
            yield return new WaitForSeconds(summonSound.length / summonSoundPitch);

            // 두 번째 재생
            audioSource.Play();
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
        damageAmount += amount;
        Debug.Log("칼 데미지 : " + damageAmount);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Creature") || other.gameObject.CompareTag("Boss"))
            {
                // CreatureHealth와 Mummy 컴포넌트 모두 체크
                CreatureHealth enemyHealth = other.gameObject.GetComponent<CreatureHealth>();
                BossHP BossHealth = other.gameObject.GetComponent<BossHP>();
                Mummy enemyHealth2 = other.gameObject.GetComponent<Mummy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                    Destroy(gameObject);
                }

                if (enemyHealth2 != null)
                {
                    enemyHealth2.TakeDamage(damageAmount);
                    Destroy(gameObject);
                }

                if (BossHealth != null)
                {
                    BossHealth.TakeDamage(damageAmount);
                    Destroy(gameObject);
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
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (playerTransform == null)
            return;

        float angle = swordIndex * (360f / totalSwords) + (Time.time * orbitSpeed);
        float x = orbitRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float z = orbitRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

        Vector3 newPosition = playerTransform.position + new Vector3(x, 0f, z);
        transform.position = newPosition;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(-directionToPlayer);
        transform.rotation = lookRotation * Quaternion.Euler(90, 0, 0);
    }
}
