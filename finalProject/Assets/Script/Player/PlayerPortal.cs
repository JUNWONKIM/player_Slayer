using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortal : MonoBehaviour
{
    public GameObject portalPrefab; // 생성할 포탈 프리팹
    public float distanceToSpawnPortal = 40.0f; // 포탈 생성 거리를 정의하는 변수
    public float portalOffset = 20.0f; // 포탈 생성 위치의 오프셋을 정의하는 변수
    public float portalHeight = 15.0f; // 포탈의 y 위치를 설정하는 변수

    private GameObject[] portals; // 생성된 포탈을 저장할 배열
    private bool portalSpawned = false; // 포탈이 이미 생성되었는지 여부를 추적하는 플래그
    private Transform boss; // 보스의 Transform을 저장할 변수

    void Start()
    {
        // 포탈을 저장할 배열을 초기화 (8개의 포탈 생성)
        portals = new GameObject[8];

        // 태그를 이용해 보스 찾기
        GameObject bossObject = GameObject.FindGameObjectWithTag("Boss");
        if (bossObject != null)
        {
            boss = bossObject.transform;
        }
        else
        {
            Debug.LogError("Boss object with tag 'Boss' not found.");
        }
    }

    void Update()
    {
        if (boss == null)
        {
            // 보스가 없으면 업데이트를 진행하지 않음
            return;
        }

        // 보스와의 거리 확인
        float distanceToBoss = Vector3.Distance(transform.position, boss.position);

        // 보스와의 거리가 distanceToSpawnPortal 이하일 때, 포탈이 생성되지 않은 경우에만 포탈 생성
        if (distanceToBoss <= distanceToSpawnPortal && !portalSpawned)
        {
            SpawnPortals();
        }
        // 보스가 거리 밖으로 나가면 포탈을 삭제
        else if (distanceToBoss > distanceToSpawnPortal && portalSpawned)
        {
            DestroyPortals();
        }
        else if (portalSpawned)
        {
            // 포탈의 위치를 보스와의 거리와 관련하여 업데이트
            UpdatePortalPositions();
        }
    }

    void SpawnPortals()
    {
        // 8 방향 벡터를 정의 (동서남북 + 대각선)
        Vector3[] directions = new Vector3[]
        {
            transform.forward,                // 북쪽
            -transform.forward,               // 남쪽
            transform.right,                  // 동쪽
            -transform.right,                 // 서쪽
            (transform.forward + transform.right).normalized,    // 북동쪽
            (transform.forward - transform.right).normalized,    // 북서쪽
            (-transform.forward + transform.right).normalized,   // 남동쪽
            (-transform.forward - transform.right).normalized    // 남서쪽
        };

        // 각 방향에 맞는 회전값을 정의
        Quaternion[] rotations = new Quaternion[]
        {
            Quaternion.LookRotation(transform.forward),                // 북쪽을 바라보는 회전
            Quaternion.LookRotation(-transform.forward),               // 남쪽을 바라보는 회전
            Quaternion.LookRotation(transform.right),                  // 동쪽을 바라보는 회전
            Quaternion.LookRotation(-transform.right),                 // 서쪽을 바라보는 회전
            Quaternion.LookRotation((transform.forward + transform.right).normalized),    // 북동쪽을 바라보는 회전
            Quaternion.LookRotation((transform.forward - transform.right).normalized),    // 북서쪽을 바라보는 회전
            Quaternion.LookRotation((-transform.forward + transform.right).normalized),   // 남동쪽을 바라보는 회전
            Quaternion.LookRotation((-transform.forward - transform.right).normalized)    // 남서쪽을 바라보는 회전
        };

        for (int i = 0; i < directions.Length; i++)
        {
            // 포탈의 위치를 계산
            Vector3 portalPosition = transform.position + directions[i] * portalOffset;
            portalPosition.y = portalHeight;

            // 포탈을 생성하고, 지정된 회전값으로 회전
            portals[i] = Instantiate(portalPrefab, portalPosition, rotations[i]);
        }

        // 포탈이 생성되었음을 표시
        portalSpawned = true;
    }

    void DestroyPortals()
    {
        // 생성된 포탈을 삭제
        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i] != null)
            {
                Destroy(portals[i]);
            }
        }

        // 포탈이 삭제되었음을 표시
        portalSpawned = false;
    }

    void UpdatePortalPositions()
    {
        // 포탈의 위치를 보스와의 거리와 관련하여 업데이트
        Vector3[] directions = new Vector3[]
        {
            transform.forward,                // 북쪽
            -transform.forward,               // 남쪽
            transform.right,                  // 동쪽
            -transform.right,                 // 서쪽
            (transform.forward + transform.right).normalized,    // 북동쪽
            (transform.forward - transform.right).normalized,    // 북서쪽
            (-transform.forward + transform.right).normalized,   // 남동쪽
            (-transform.forward - transform.right).normalized    // 남서쪽
        };

        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i] != null)
            {
                // 포탈의 새로운 위치를 계산
                Vector3 portalPosition = transform.position + directions[i] * portalOffset;
                portalPosition.y = portalHeight;

                // 포탈의 위치를 업데이트
                portals[i].transform.position = portalPosition;
                portals[i].transform.rotation = Quaternion.LookRotation(directions[i]);
            }
        }
    }
}
