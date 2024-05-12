using System.Collections.Generic;
using UnityEngine;

public class Player_Shooter : MonoBehaviour
{
    public GameObject projectilePrefab; // 발사체 프리팹을 할당할 변수
    public float fireInterval = 1f; // 발사 간격
    public float detectionRange = 100f; // 적을 탐지할 범위
    public float projectileSpeed = 100f;
    private float lastFireTime; // 마지막 발사 시간

    private LineRenderer detectionRangeVisual; // 적 탐지 범위를 시각적으로 표시할 라인 렌더러

    void Start()
    {
        // LineRenderer 컴포넌트 추가
        detectionRangeVisual = gameObject.AddComponent<LineRenderer>();

        // 라인 렌더러 설정
        detectionRangeVisual.material = new Material(Shader.Find("Sprites/Default"));
        detectionRangeVisual.startColor = Color.red;
        detectionRangeVisual.endColor = Color.red;
        detectionRangeVisual.startWidth = 0.1f;
        detectionRangeVisual.endWidth = 0.1f;
        detectionRangeVisual.positionCount = 37; // 원의 꼭지점 개수
    }

    void Update()
    {
        // 일정 간격으로 가장 가까운 적을 탐지하고 발사체를 발사
        if (Time.time - lastFireTime > fireInterval)
        {
            FireProjectile();
            lastFireTime = Time.time;
        }

        // 탐지 범위를 플레이어의 중심을 따라다니도록 업데이트
        UpdateDetectionRange();
    }

    void FireProjectile()
    {
        // 가장 가까운 적을 탐지
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        GameObject[] creatures2 = GameObject.FindGameObjectsWithTag("Creature_2");
        List<GameObject> allCreatures = new List<GameObject>();
        allCreatures.AddRange(creatures);
        allCreatures.AddRange(creatures2);

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
            else
            {
                Debug.LogWarning("Projectile prefab does not have a Rigidbody component.");
            }
        }
    }


    // 탐지 범위를 플레이어의 중심을 따라다니도록 업데이트하는 함수
    private void UpdateDetectionRange()
    {
        // 원 모양의 점 생성
        Vector3[] points = new Vector3[37];
        for (int i = 0; i < 37; i++)
        {
            float angle = i * Mathf.PI * 2f / 36f;
            points[i] = transform.position + new Vector3(Mathf.Sin(angle) * detectionRange, 0f, Mathf.Cos(angle) * detectionRange);
        }

        // 라인 렌더러에 점 설정
        detectionRangeVisual.SetPositions(points);
    }
}
