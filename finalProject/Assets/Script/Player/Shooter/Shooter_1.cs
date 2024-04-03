using UnityEngine;

public class Shooter_1 : MonoBehaviour
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

        // 원 모양의 점 생성
        Vector3[] points = new Vector3[37];
        for (int i = 0; i < 37; i++)
        {
            float angle = i * Mathf.PI * 2f / 36f;
            points[i] = new Vector3(Mathf.Sin(angle) * detectionRange, 0f, Mathf.Cos(angle) * detectionRange);
        }

        // 라인 렌더러에 점 설정
        detectionRangeVisual.SetPositions(points);
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
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= detectionRange)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }

        // 발사체를 발사할 적이 있는 경우 발사
        if (closestEnemy != null)
        {
            Vector3 targetDirection = closestEnemy.transform.position - transform.position;
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
}
