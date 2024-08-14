using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public float minDistance = 200.0f; // 포탈로부터의 최소 거리

    void OnTriggerEnter(Collider other)
    {
        // 모든 오브젝트가 포탈과 충돌할 때
        MoveObjectToRandomLocation(other.transform);
    }

    void MoveObjectToRandomLocation(Transform objectTransform)
    {
        // 포탈 위치에서 벗어난 랜덤 위치를 계산
        Vector3 randomDirection = Random.insideUnitSphere * minDistance;
        randomDirection.y = 0; // y 축을 0으로 설정하여 지면에 위치하도록 함

        // 새로운 위치를 포탈에서 minDistance 이상 떨어진 곳으로 계산
        Vector3 newPosition = transform.position + randomDirection;

        // 오브젝트의 위치를 새로운 위치로 설정
        objectTransform.position = newPosition;
    }
}
