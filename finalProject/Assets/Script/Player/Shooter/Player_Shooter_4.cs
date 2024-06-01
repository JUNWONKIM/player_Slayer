using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shooter_4 : MonoBehaviour
{
    public static Player_Shooter_4 instance;

    public GameObject bulletPrefab; // 탄환 프리팹
    public Transform firePoint; // 탄환 발사 위치
    public float bulletSpeed = 20f; // 탄환 속도
    public int bulletCount = 0; // 총알 수
    public float fireRate = 1f; // 발사 간격 (초 단위)
    public float damageAmount = 10; // 데미지 양
    public float lifetime = 2f; // 탄환의 수명 (초)

    private Transform playerTransform; // 플레이어 트랜스폼


    void Awake()
    {
        instance = this;
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
        InvokeRepeating("Shoot", 0f, fireRate);
    }

    void Shoot()
    {
        if (playerTransform == null)
        {
            return; // Player 오브젝트를 찾지 못한 경우 발사하지 않음
        }

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 탄환 확산 설정
            float xSpread = Random.Range(-30f, 30f);
            float ySpread = Random.Range(2f, 5f); // y축 확산 범위 2에서 5 사이

            // 회전된 방향 벡터 계산
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

    public void IncreaseBulletCount(int amount)
    {

        bulletCount += amount;
        Debug.Log("샷건 개수 : " + bulletCount);

    }

    public void IncreaseDamage(float amount)
    {
        // Player_Shooter_4 클래스의 damageAmount 값을 변경
        damageAmount += amount;
        Debug.Log("샷건 데미지 : " + damageAmount);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;
        private Player_Shooter_4 shooterInstance;

        // 생성자를 이용하여 Player_Shooter_4 인스턴스를 전달받음
        public BulletCollisionHandler(Player_Shooter_4 shooterInstance)
        {
            this.shooterInstance = shooterInstance;
        }

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
            }
        }

      
    }
}
