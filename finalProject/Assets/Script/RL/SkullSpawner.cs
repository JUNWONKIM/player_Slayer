using System.Collections;
using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
    public GameObject skullPrefab;         // 해골 프리팹
    public Transform player;               // 플레이어(에이전트) 트랜스폼
    public float spawnRadius = 20f;        // 플레이어로부터 몇 m 떨어진 위치에 소환할지
    public float spawnInterval = 3f;       // 해골 소환 간격
    public int maxSkulls = 10;             // 동시에 존재할 수 있는 해골 수 제한

    private int currentSkullCount = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(SpawnSkullsRoutine());
    }

    IEnumerator SpawnSkullsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 최대 수 제한
            if (currentSkullCount >= maxSkulls) continue;

            SpawnSkull();
        }
    }

    void SpawnSkull()
    {
        // 플레이어 근처 랜덤 위치 계산
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0;
        Vector3 spawnPosition = player.position + randomDirection;

        GameObject skull = Instantiate(skullPrefab, spawnPosition, Quaternion.identity);
        skull.tag = "Creature";  // 혹시라도 프리팹에 태그가 안붙어 있다면
        currentSkullCount++;

        // 해골이 죽으면 카운트 감소 (Skull 스크립트에 추가 필요)
        Skull skullScript = skull.GetComponent<Skull>();
        if (skullScript != null)
        {
            skullScript.StartCoroutine(RemoveOnDeath(skull));
        }
    }

    IEnumerator RemoveOnDeath(GameObject skull)
    {
        Animator animator = skull.GetComponent<Animator>();
        while (animator != null && !animator.GetBool("isDie"))
        {
            yield return null;
        }

        // 해골 오브젝트가 죽으면 일정 시간 뒤 삭제하고 카운트 줄이기
        yield return new WaitForSeconds(2f);
        currentSkullCount--;
    }
}
