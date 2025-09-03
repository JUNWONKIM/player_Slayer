using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch_freeze_effect : MonoBehaviour
{
    private Transform player;
    public float lifetime = 5.0f;  // 파티클의 지속 시간

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, lifetime);  // 지속 시간 뒤 파티클 삭제
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position;  // 파티클이 용사를 따라다니게함
        }
    }
}
