using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AI_Test : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도

    private Vector3 targetPosition; // 목표 지점

    void Start()
    {
        // 소환되었을 때 가장 적은 곳으로 이동하도록 설정
        SetTargetPosition();
    }

    void Update()
    {
        // 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 목표 지점에 도착하면 새로운 목표 지점 설정
        if (transform.position == targetPosition)
        {
            SetTargetPosition();
        }

        // 플레이어가 적 몬스터를 향하도록 설정
        LookAtEnemy();
    }

    void SetTargetPosition()
    {
        // 모든 적 몬스터들 간의 가장 먼 거리를 찾아서 해당 방향으로 이동
        Vector3 fleeDirection = FindFleeDirection();
        targetPosition = transform.position + fleeDirection * 10f; // 이동 거리를 조정하여 목표 위치 설정
    }

    Vector3 FindFleeDirection()
    {
        Vector3 fleeDirection = Vector3.zero;
        float farthestDistance = 0f;
        Vector3 currentPosition = transform.position;

        // 모든 적을 찾아서 가장 먼 거리의 적을 찾음
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject) // 현재 자기 자신은 무시
            {
                float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
                if (distanceToEnemy > farthestDistance)
                {
                    farthestDistance = distanceToEnemy;
                    fleeDirection = currentPosition - enemy.transform.position;
                }
            }
        }

        return fleeDirection.normalized;
    }

    void LookAtEnemy()
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
