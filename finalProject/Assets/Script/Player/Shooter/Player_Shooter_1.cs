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
            FireProjectile();
            if (i < projectilesPerFire - 1)
            {
                yield return new WaitForSeconds(burstInterval);
            }
        }
    }

    void FireProjectile()
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
            GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, rotation);

            Rigidbody projectileRigidbody = projectileInstance.GetComponent<Rigidbody>();
            if (projectileRigidbody != null)
            {
                projectileRigidbody.velocity = targetDirection.normalized * projectileSpeed;
            }
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
        Debug.Log("Fire rate increased, interval is now: " + fireInterval);
    }

    public void IncreaseProjectileCount(int amount)
    {
        projectilesPerFire += amount;
        Debug.Log("Projectile count increased, now firing: " + projectilesPerFire + " projectiles per shot.");
    }
}
