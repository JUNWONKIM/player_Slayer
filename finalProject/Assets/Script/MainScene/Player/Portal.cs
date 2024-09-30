using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public float minDistance = 200.0f; // 포탈로부터의 최소 거리

    void OnTriggerEnter(Collider other)
    {
        MoveObjectToRandomLocation(other.transform);
    }

    void MoveObjectToRandomLocation(Transform objectTransform) //오브젝트를 랜덤 위치로 이동
    {
        // 포탈 위치에서 벗어난 랜덤 위치를 계산
        Vector3 randomDirection = Random.insideUnitSphere * minDistance;
        randomDirection.y = 0;

        Vector3 newPosition = transform.position + randomDirection;

        objectTransform.position = newPosition;
    }
}
