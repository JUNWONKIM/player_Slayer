using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortal : MonoBehaviour
{
    public GameObject portalPrefab; // 포탈 프리팹
    public float distanceToSpawnPortal = 40.0f; // 포탈 생성 거리
    public float portalOffset = 20.0f; // 포탈 생성 위치
    public float portalHeight = 15.0f; // 포탈의 y 위치

    private GameObject[] portals; // 생성된 포탈을 저장
    private bool portalSpawned = false; // 포탈 생성 여부
    private GameObject boss; 

    void Start()
    {
        portals = new GameObject[8]; //포탈 8개 초기화

        boss = GameObject.FindGameObjectWithTag("Boss");
    }

    void Update()
    {
        // 보스 오브젝트를 지속적으로 찾음
        if (boss == null)
        {
            boss = GameObject.FindGameObjectWithTag("Boss");
            return;
        }

        // 보스와의 거리 확인
        float distanceToBoss = Vector3.Distance(transform.position, boss.transform.position);
              
        if (distanceToBoss <= distanceToSpawnPortal && !portalSpawned)  // 보스와의 거리가 distanceToSpawnPortal 이하일 때
        {
            SpawnPortals(); //포탈 소환
        }
      
        else if (distanceToBoss > distanceToSpawnPortal && portalSpawned)   // 보스가 거리 밖으로 나가면 포탈을 삭제
        {
            DestroyPortals(); //포탈 삭제
        }
        else if (portalSpawned)
        {
            UpdatePortalPositions(); //포탈 위치 업데이트
        }
    }

    void SpawnPortals() //포탈 생성
    {
        //위치
        Vector3[] directions = new Vector3[] {
            transform.forward,                // 북쪽
            -transform.forward,               // 남쪽
            transform.right,                  // 동쪽
            -transform.right,                 // 서쪽
            (transform.forward + transform.right).normalized,    // 북동쪽
            (transform.forward - transform.right).normalized,    // 북서쪽
            (-transform.forward + transform.right).normalized,   // 남동쪽
            (-transform.forward - transform.right).normalized    // 남서쪽
        };
        //바라보는 방향
        Quaternion[] rotations = new Quaternion[] {
            Quaternion.LookRotation(transform.forward),                // 북쪽
            Quaternion.LookRotation(-transform.forward),               // 남쪽
            Quaternion.LookRotation(transform.right),                  // 동쪽
            Quaternion.LookRotation(-transform.right),                 // 서쪽
            Quaternion.LookRotation((transform.forward + transform.right).normalized),    // 북동쪽
            Quaternion.LookRotation((transform.forward - transform.right).normalized),    // 북서쪽
            Quaternion.LookRotation((-transform.forward + transform.right).normalized),   // 남동쪽
            Quaternion.LookRotation((-transform.forward - transform.right).normalized)    // 남서쪽
        };

        for (int i = 0; i < directions.Length; i++)
        {
            // 포탈의 위치를 계산
            Vector3 portalPosition = transform.position + directions[i] * portalOffset;
            portalPosition.y = portalHeight;

            // 회전
            portals[i] = Instantiate(portalPrefab, portalPosition, rotations[i]);
        }

        // 포탈 생성 상태 체크
        portalSpawned = true;
    }

    void DestroyPortals() //포탈 삭제
    {
        // 생성된 포탈을 삭제
        for (int i = 0; i < portals.Length; i++)
        {
            if (portals[i] != null)
            {
                Destroy(portals[i]);
            }
        }

        // 포탈 생성 상태 체크
        portalSpawned = false;
    }

    void UpdatePortalPositions() //포탈 위치 업데이트
    {
        Vector3[] directions = new Vector3[] {
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
