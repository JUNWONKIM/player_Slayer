using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATK2 : MonoBehaviour
{
    public float speed = 10.0f; // ATK2의 이동 속도
    public float damage = 50.0f; // ATK2가 플레이어에게 줄 데미지

    private Transform player; // 플레이어의 Transform

    void Start()
    {
        // 플레이어 오브젝트를 찾음
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        // 플레이어를 향해 이동
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 플레이어일 경우 데미지를 줌
        if (other.CompareTag("Player"))
        {
            PlayerHP playerHP = other.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
            }

            // 충돌 후 ATK2 오브젝트를 파괴
            Destroy(gameObject);
        }
    }
}
