using System.Collections.Generic;
using UnityEngine;

public class Player_Shooter : MonoBehaviour
{
    public GameObject projectilePrefab; // 발사체 프리팹을 할당할 변수
    public float fireInterval = 1f; // 발사 간격
    public float fireInterval_slow = 2f;
    public float detectionRange = 100f; // 적을 탐지할 범위
    public float projectileSpeed = 100f;
    private float lastFireTime; // 마지막 발사 시간

  

    void Start()
    {
        // LineRenderer 컴포넌트 추가
     
    }

    void Update()
    {
        // 일정 간격으로 가장 가까운 적을 탐지하고 발사체를 발사
        if (Time.time - lastFireTime > fireInterval)
        {
            FireProjectile();
            lastFireTime = Time.time;
        }

    }

    void FireProjectile()
    {
        // 가장 가까운 적을 탐지
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

        // 발사체를 발사할 적이 있는 경우 발사
        if (closestCreature != null)
        {
            Vector3 targetDirection = closestCreature.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(targetDirection);
            GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, rotation);

            // 발사체에 이동 속도 부여
            Rigidbody projectileRigidbody = projectileInstance.GetComponent<Rigidbody>();
            if (projectileRigidbody != null)
            {
                // 발사할 적의 방향으로 발사체를 이동시킴
                projectileRigidbody.velocity = targetDirection.normalized * projectileSpeed;
            }
        }

        CheckForSlowObjects();
    }


    // 탐지 범위를 플레이어의 중심을 따라다니도록 업데이트하는 함수
 
 
    private void CheckForSlowObjects()
    {
        // 주변에 있는 모든 게임 오브젝트를 가져옵니다.
        GameObject[] slowObjects = GameObject.FindGameObjectsWithTag("Slow");

        // 주변에 Slow 태그를 가진 오브젝트가 있는지 확인합니다.
        if (slowObjects.Length > 0)
        {
            // Slow 태그를 가진 오브젝트가 존재하면 이동 속도를 감소시킵니다.
            fireInterval = fireInterval_slow; // 이동 속도를 50%로 줄입니다.
        }
        else
        {
            // Slow 태그를 가진 오브젝트가 존재하지 않으면 원래 이동 속도로 복원합니다.
           fireInterval = 1f; // 이동 속도를 100%로 복원합니다.
        }
    }
}
