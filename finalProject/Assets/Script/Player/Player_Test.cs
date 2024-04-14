using UnityEngine;

public class Player_Test : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float detectionRange = 10f; // 탐지 범위
    public Color detectionColor = Color.red; // 탐지 범위의 색상
    public float updateInterval = 1f; // 업데이트 간격
    public float lookAtInterval = 0.2f; // 바라보기 간격

    private LineRenderer detectionRangeVisual; // 적 탐지 범위를 시각적으로 표시할 라인 렌더러

    private void Start()
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

        // 코루틴 시작
        InvokeRepeating("UpdateDetection", 0f, updateInterval);
        InvokeRepeating("LookAtEnemy", 0f, lookAtInterval);
    }

    private void Update()
    {
        // 플레이어 이동
        MovePlayer();
    }

    private void MovePlayer()
    {
        // 현재 적의 무게중심 방향으로 이동
        Vector3 centerOfMass = FindCenterOfMass();
        Vector3 moveDirection = centerOfMass - transform.position;
        moveDirection.y = 0; // Y 축 이동 제한
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }

    private void UpdateDetection()
    {
        // 탐지 범위를 시각적으로 나타내기
        DrawDetectionRange(transform.position, Vector3.up, detectionRange, detectionColor);
    }

    // 주변에 있는 적들의 위치의 무게중심을 찾는 함수
    private Vector3 FindCenterOfMass()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // 적들의 게임 오브젝트를 가져옴

        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (GameObject enemy in enemies)
        {
            // 적과의 거리를 계산하여 탐지 범위 내에 있는지 확인
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= detectionRange)
            {
                center += enemy.transform.position; // 적의 위치를 더함
                count++;
            }
        }

        if (count == 0)
        {
            // 탐지 범위 내에 적이 없으면 현재 위치를 반환
            return transform.position;
        }

        center /= count; // 평균을 구해서 무게중심을 찾음

        return center;
    }

    // 탐지 범위를 시각적으로 나타내는 함수
    private void DrawDetectionRange(Vector3 position, Vector3 up, float radius, Color color, int segments = 36)
    {
        // 원 모양의 점 생성
        Vector3[] points = new Vector3[37];
        for (int i = 0; i < 37; i++)
        {
            float angle = i * Mathf.PI * 2f / 36f;
            points[i] = position + new Vector3(Mathf.Sin(angle) * radius, 0f, Mathf.Cos(angle) * radius);
        }

        // 라인 렌더러에 점 설정
        detectionRangeVisual.SetPositions(points);
    }

    private void LookAtEnemy()
    {
        // 가장 가까운 적을 찾아 플레이어가 해당 적을 향하도록 설정
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(playerPosition, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            transform.LookAt(closestEnemy.transform);
        }
    }
}
