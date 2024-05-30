using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Shooter_2 : MonoBehaviour
{
    public static Player_Shooter_2 instance;

    public GameObject objectToSpawn; // 소환할 오브젝트 프리팹
    public float spawnInterval = 3f; // 소환 간격
    public float spawnRadius = 20f; // 소환 범위 반지름
    public float fixedYPosition = 2.5f; // 고정된 Y 축 위치
    public int projectilesPerFire = 1; // 한 번에 발사할 발사체 수

    private float lastSpawnTime; // 마지막 소환 시간

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // 일정 간격으로 오브젝트 소환
        if (Time.time - lastSpawnTime > spawnInterval)
        {
            SpawnObject();
            lastSpawnTime = Time.time;
        }
    }

    void SpawnObject()
    {
        for (int i = 0; i < projectilesPerFire; i++)
        {
            // 플레이어 주변 원형 범위 내에서 랜덤한 위치 계산
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = fixedYPosition; // 높이를 고정된 Y 값으로 설정

            // 오브젝트 소환
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    public void IncreaseProjectileCount(int amount)
    {
        projectilesPerFire += amount;
        Debug.Log("Projectile count increased, now firing: " + projectilesPerFire + " projectiles per shot.");
    }
}
