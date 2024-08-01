using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player; // 플레이어의 위치를 저장할 변수
    public float speed = 5.0f; // 보스가 이동할 속도
    public float rotationSpeed = 5.0f; // 보스가 회전할 속도

    // Update is called once per frame
    void Update()
    {
        // 플레이어를 향해 다가가기
        Vector3 direction = (player.position - transform.position).normalized;

        // 플레이어를 바라보기
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // 플레이어를 향해 이동하기
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
