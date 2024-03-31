using UnityEngine;

public class Enemy_1 : MonoBehaviour
{
    public float moveSpeed = 5f; // 적의 이동 속도
    private Transform player; // 플레이어의 위치

   

    void Start()
    {
        // 플레이어 게임 오브젝트를 찾아 트랜스폼을 할당
        player = GameObject.FindGameObjectWithTag("Player").transform;

     
    }

    void Update()
    {
        // 플레이어를 향해 이동
        transform.LookAt(player);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }
}
