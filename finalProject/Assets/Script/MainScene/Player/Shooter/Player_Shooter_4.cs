using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shooter_4 : MonoBehaviour
{
    public static Player_Shooter_4 instance;

    public GameObject bulletPrefab; // 탄환 프리팹
    public Transform firePoint; // 탄환 발사 위치
    public float fireInterval = 1f; // 발사 간격 (초 단위)

    public float bulletSpeed = 20f; // 탄환 속도
    public int bulletCount = 0; // 총알 수

    public float damageAmount = 10; // 데미지 양
    public float lifetime = 2f; // 탄환의 수명 (초)

    private Transform playerTransform; // 플레이어 트랜스폼

    private bool isSlowed = false; // Slow 상태 여부
    public float fireIntervalSlowMultiplier = 2f; // Slow 효과 시 발사 간격 배수

    public AudioClip fireSound; // 발사 사운드 클립
    private AudioSource audioSource; // AudioSource 변수 추가

    // 사운드 속도와 볼륨 설정 변수
    [Range(0.1f, 3f)] public float fireSoundPitch = 1f; // 사운드의 속도 (피치)
    [Range(0f, 1f)] public float fireSoundVolume = 1f; // 사운드의 볼륨

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
    }

    void Start()
    {
        // Player 태그의 오브젝트를 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }

        // 일정 시간마다 Shoot 메서드 호출
        InvokeRepeating("Shoot", 0f, fireInterval);
    }

    void Update()
    {
        CheckForSlowObjects();
    }

    void Shoot()
    {
        if (playerTransform == null || bulletCount <= 0)
        {
            return; // Player 오브젝트를 찾지 못하거나 bulletCount가 0 이하인 경우 발사하지 않음
        }

        // 발사 사운드 재생
        PlayFireSound();

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 탄환 확산 설정
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0);
            Vector3 spreadDirection = spreadRotation * playerTransform.forward;

            // 방향 벡터에서 y 값 설정
            spreadDirection.y = Mathf.Clamp(spreadDirection.y, 0.1f, 1f); // y 값이 음수로 내려가지 않도록 클램핑

            Vector3 direction = spreadDirection.normalized;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = direction * bulletSpeed;

            // 탄환 파괴를 스크립트에서 직접 관리
            Destroy(bullet, lifetime);

            // 충돌 처리 컴포넌트를 동적으로 추가
            BulletCollisionHandler collisionHandler = bullet.AddComponent<BulletCollisionHandler>();
            collisionHandler.damageAmount = damageAmount;
        }
    }

    void PlayFireSound()
    {
        if (audioSource != null && fireSound != null)
        {
            audioSource.clip = fireSound;
            audioSource.pitch = fireSoundPitch;
            audioSource.volume = fireSoundVolume;
            audioSource.Play();
        }
    }

    private void CheckForSlowObjects()
    {
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        if (slowObjects.Length > 0 && !isSlowed)
        {
            fireInterval *= fireIntervalSlowMultiplier; // 발사 간격을 두 배로 늘림
            isSlowed = true;
        }
        else if (slowObjects.Length == 0 && isSlowed)
        {
            fireInterval /= fireIntervalSlowMultiplier; // 발사 간격을 원래대로 돌림
            isSlowed = false;
        }
    }

    public void IncreaseBulletCount(int amount)
    {
        bulletCount += amount;
        Debug.Log("샷건 개수 : " + bulletCount);
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("샷건 데미지 : " + damageAmount);
    }

    public void IncreaseFireRate(float amount)
    {
        fireInterval /= amount;
        if (fireInterval < 0.1f) fireInterval = 0.1f; // 최소 발사 간격 제한
        Debug.Log("샷건 발사 속도 :" + fireInterval);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Creature"))
            {
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
