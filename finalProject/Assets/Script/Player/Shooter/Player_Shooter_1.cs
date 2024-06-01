using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shooter_1 : MonoBehaviour
{
    public static Player_Shooter_1 instance;

    public GameObject projectilePrefab; // 발사체 프리팹을 할당할 변수

    public float fireInterval = 1f; // 발사 간격
    public float fireIntervalSlowMultiplier = 2f; // Slow 효과 시 발사 간격 배수
    public float detectionRange = 100f; // 적을 탐지할 범위
    public float projectileSpeed = 100f;
    public int projectilesPerFire = 1; // 한 번에 발사할 발사체 수
    public float burstInterval = 0.1f; // 연속 발사 간격
    public float damageAmount = 1; // 데미지 양

    private float lastFireTime; // 마지막 발사 시간
    private bool isSlowed = false; // Slow 상태 여부

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
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
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        List<GameObject> allCreatures = new List<GameObject>();
        allCreatures.AddRange(creatures);

        GameObject closestCreature = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject creature in allCreatures)
        {
            float distance = Vector3.Distance(transform.position, creature.transform.position);
            if (distance < closestDistance && distance <= detectionRange)
            {
                closestCreature = creature;
                closestDistance = distance;
            }
        }

        if (closestCreature != null)
        {
            Vector3 targetDirection = closestCreature.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            GameObject bullet = Instantiate(projectilePrefab, transform.position, rotation);

            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            if (bulletRigidbody != null)
            {
                bulletRigidbody.velocity = targetDirection.normalized * projectileSpeed;
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
            fireInterval *= fireIntervalSlowMultiplier; // 발사 간격을 두 배로 늘림
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
        fireInterval -= amount;
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
        // Player_Shooter_4 클래스의 damageAmount 값을 변경
        damageAmount += amount;
        Debug.Log("투사체  데미지 : " + damageAmount);
    }

    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;
        private Player_Shooter_1 shooterInstance;

        public BulletCollisionHandler(Player_Shooter_1 shooterInstance)
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

                // 총알을 파괴
                Destroy(gameObject);
            }
        }
    }
}
