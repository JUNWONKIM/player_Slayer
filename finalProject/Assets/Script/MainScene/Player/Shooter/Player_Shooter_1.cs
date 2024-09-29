using System.Collections;
using UnityEngine;

public class Player_Shooter_1 : MonoBehaviour
{
    public static Player_Shooter_1 instance;

    public GameObject bulletPrefab; // 발사체 프리팹을 할당할 변수

    public float fireInterval = 1f; // 발사 간격
    public float detectionRange = 100f; // 적을 탐지할 범위
    public float projectileSpeed = 100f;
    public int projectilesPerFire = 1; // 한 번에 발사할 발사체 수
    public float burstInterval = 0.1f; // 연속 발사 간격
    public float damageAmount = 1; // 데미지 양

    private float lastFireTime; // 마지막 발사 시간

    private bool isSlowed = false; // Slow 상태 여부
    public float fireIntervalSlowMultiplier = 2f; // Slow 효과 시 발사 간격 배수

    public AudioClip shootSound; // 발사 사운드 클립 추가
    private AudioSource audioSource; // AudioSource 변수 추가
    [Range(0f, 1f)] // 인스펙터에서 슬라이드 바로 조절할 수 있게 설정
    public float volume = 1f; // 사운드 볼륨 조절 변수

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
            audioSource.volume = volume; // 초기 볼륨 설정
        }
        else
        {
            Destroy(gameObject); // Singleton 패턴을 위한 중복 인스턴스 제거
        }
    }

    void Update()
    {
        // 인스펙터에서 볼륨이 변경되었을 때 AudioSource에 적용
        audioSource.volume = volume;

        if (Time.time - lastFireTime > fireInterval)
        {
            StartCoroutine(FireProjectileBurst());
            lastFireTime = Time.time;
        }

        CheckForSlowObjects(); // Slow 태그 체크
    }

    IEnumerator FireProjectileBurst()
    {
        for (int i = 0; i < projectilesPerFire; i++)
        {
            Shoot();
            if (i < projectilesPerFire - 1)
            {
                yield return new WaitForSeconds(burstInterval);
            }
        }
    }

    void Shoot()
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Creature와 Boss 태그를 가진 객체를 모두 찾기
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Creature");
        foreach (GameObject target in GameObject.FindGameObjectsWithTag("Boss"))
        {
            var newTargets = new GameObject[targets.Length + 1];
            targets.CopyTo(newTargets, 0);
            newTargets[targets.Length] = target;
            targets = newTargets;
        }

        // 가장 가까운 목표를 찾기
        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < closestDistance && distance <= detectionRange)
            {
                closestTarget = target;
                closestDistance = distance;
            }
        }

        if (closestTarget != null)
        {
            Vector3 targetDirection = closestTarget.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.velocity = targetDirection.normalized * projectileSpeed;
            }

            // 발사 사운드 재생
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // 충돌 처리 컴포넌트를 동적으로 추가
            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;
        }
    }

    private void CheckForSlowObjects()
    {
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        if (slowObjects.Length > 0 && !isSlowed)
        {
            fireInterval *= fireIntervalSlowMultiplier; // 발사 간격을 느리게 함
            isSlowed = true;
        }
        else if (slowObjects.Length == 0 && isSlowed)
        {
            fireInterval /= fireIntervalSlowMultiplier; // 발사 간격을 원래대로 돌림
            isSlowed = false;
        }
    }

    public void IncreaseFireRate(float amount)
    {
        fireInterval /= amount;
        if (fireInterval < 0.1f) fireInterval = 0.1f; // 최소 발사 간격 제한
        Debug.Log("투사체 발사 속도 :" + fireInterval);
    }

    public void IncreaseProjectileCount(int amount)
    {
        projectilesPerFire += amount;
        Debug.Log("투사체 개수 : " + projectilesPerFire);
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("투사체 데미지 : " + damageAmount);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;

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
