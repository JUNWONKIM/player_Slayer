using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch_freeze_effect : MonoBehaviour
{
    private Transform player;
    public float lifetime = 5.0f;  // ��ƼŬ�� ���� �ð�

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, lifetime);  // ���� �ð� �� ��ƼŬ ����
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.position;  // ��ƼŬ�� ��縦 ����ٴϰ���
        }
    }
}
