using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public float hp = 1000f;
    public float max_hp = 1000f;
    public GameObject bossPrefab; // Boss 프리팹
    public float bossSpawnRadius = 200f; // Boss 생성 반경
    private GameObject boss; // 현재 비활성화 되어 있는 Boss

    // Start is called before the first frame update
    void Start()
    {
        hp = max_hp;

        // Boss 프리팹을 미리 생성하지 않고, 비활성화된 상태로 설정
        if (bossPrefab != null)
        {
            boss = Instantiate(bossPrefab);
            boss.SetActive(false); // 초기에는 비활성화
        }
    }

    // Update is called once per frame
    void Update()
    {
        // HP가 최대 HP의 30% 이하일 때 Boss를 생성
        if (hp <= max_hp * 0.3f && boss != null && !boss.activeInHierarchy)
        {
            SpawnBossNearPlayer();
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log("Player HP: " + hp);
    }

    void SpawnBossNearPlayer()
    {
        // 플레이어의 위치를 가져옴
        Vector3 playerPosition = transform.position;

        // Boss를 플레이어 주변 200 유닛 내의 랜덤 위치로 생성
        Vector3 randomPosition = playerPosition + new Vector3(
            Random.Range(-bossSpawnRadius, bossSpawnRadius),
            0,
            Random.Range(-bossSpawnRadius, bossSpawnRadius)
        );

        // y는 0으로 설정
        randomPosition.y = 0;

        // Boss를 활성화하고 위치를 설정
        boss.transform.position = randomPosition;
        boss.SetActive(true);

        // Boss가 생성되면 더 이상 생성하지 않도록
        enabled = false;
    }
}
