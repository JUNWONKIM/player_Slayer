using System.Collections;
using UnityEngine;

public class Player_Shooter_2 : MonoBehaviour
{
    public static Player_Shooter_2 instance;

    public GameObject spawnPrefab; // 총알 프리팹
    public float spawnInterval = 5f; // 총알 소환 간격
    public float spawnRadius = 20f; // 총알 소환 범위 반지름
    public float fixedYPosition = 2.5f; // 고정된 Y 축 위치
    public int projectilesPerFire = 0; // 한 번에 발사할 발사체 수
    public float bulletLifetime = 1.5f; // 총알의 수명
    public float damageAmount = 1f; // 총알의 데미지

    private float lastSpawnTime; // 마지막 소환 시간

    private bool isSlowed = false; // Slow 상태 여부
    public float spawnIntervalSlowMultiplier = 2f; // Slow 효과 시 발사 간격 배수

    public AudioClip chargeSound; // 첫 번째 사운드 클립
    public AudioClip explodeSound; // 두 번째 사운드 클립
    private AudioSource audioSource; // AudioSource 변수 추가

    // 사운드 속도와 볼륨 설정 변수
    [Range(0.1f, 3f)] public float chargeSoundPitch = 1f; // 첫 번째 사운드의 속도 (피치)
    [Range(0f, 1f)] public float chargeSoundVolume = 1f; // 첫 번째 사운드의 볼륨
    [Range(0.1f, 3f)] public float explodeSoundPitch = 1f; // 두 번째 사운드의 속도 (피치)
    [Range(0f, 1f)] public float explodeSoundVolume = 1f; // 두 번째 사운드의 볼륨

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
    }

    void Start()
    {
        InvokeRepeating("SpawnBulletWithExplosion", 0f, spawnInterval);
    }

    void Update()
    {
        CheckForSlowObjects();
    }

    void SpawnBulletWithExplosion()
    {
        for (int i = 0; i < projectilesPerFire; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = fixedYPosition;

            GameObject bullet = Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
            Destroy(bullet, bulletLifetime);

            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;

            Collider bulletCollider = bullet.GetComponent<Collider>();
            bulletCollider.enabled = false;

            StartCoroutine(EnableColliderAfterDelay(bulletCollider, 1.0f));

            StartCoroutine(PlaySoundsSequentially());
        }
    }

    IEnumerator PlaySoundsSequentially()
    {
        if (audioSource != null)
        {
            // 첫 번째 사운드 재생
            audioSource.clip = chargeSound;
            audioSource.pitch = chargeSoundPitch;
            audioSource.volume = chargeSoundVolume;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length / audioSource.pitch);

            // 두 번째 사운드 재생
            audioSource.clip = explodeSound;
            audioSource.pitch = explodeSoundPitch;
            audioSource.volume = explodeSoundVolume;
            audioSource.Play();
        }
    }

    IEnumerator EnableColliderAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }

    private void CheckForSlowObjects()
    {
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        if (slowObjects.Length > 0 && !isSlowed)
        {
            spawnInterval *= spawnIntervalSlowMultiplier; // 발사 간격을 두 배로 늘림
            isSlowed = true;
        }
        else if (slowObjects.Length == 0 && isSlowed)
        {
            spawnInterval /= spawnIntervalSlowMultiplier; // 발사 간격을 원래대로 돌림
            isSlowed = false;
        }
    }

    public void IncreaseProjectileCount(int amount)
    {
        projectilesPerFire += amount;
        Debug.Log("폭탄 개수 " + projectilesPerFire);
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("폭탄 데미지 : " + damageAmount);
    }

    public void IncreaseFireRate(float amount)
    {
        spawnInterval /= amount;
        if (spawnInterval < 0.1f) spawnInterval = 0.1f;
        Debug.Log("폭탄 발사 속도 :" + spawnInterval);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;
        private Player_Shooter_2 shooterInstance;

        public BulletCollisionHandler(Player_Shooter_2 shooterInstance)
        {
            this.shooterInstance = shooterInstance;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Creature") || other.gameObject.CompareTag("Boss"))
            {
                // CreatureHp와 Mummy 컴포넌트 모두 체크
                CreatureHp enemyHealth = other.gameObject.GetComponent<CreatureHp>();
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
