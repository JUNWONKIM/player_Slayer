using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch_freeze_effect : MonoBehaviour
{
    private Transform player;
    public float lifetime = 5.0f;  // 파티클의 수명 시간

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, lifetime);  // 5초 후에 파티클을 파괴
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position;  // 파티클을 플레이어 위치로 이동
        }
    }

}
