using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public float minDistance = 200.0f; // ��Ż�κ����� �ּ� �Ÿ�

    void OnTriggerEnter(Collider other)
    {
        MoveObjectToRandomLocation(other.transform);
    }

    void MoveObjectToRandomLocation(Transform objectTransform) //������Ʈ�� ���� ��ġ�� �̵�
    {
        // ��Ż ��ġ���� ��� ���� ��ġ�� ���
        Vector3 randomDirection = Random.insideUnitSphere * minDistance;
        randomDirection.y = 0;

        Vector3 newPosition = transform.position + randomDirection;

        objectTransform.position = newPosition;
    }
}
