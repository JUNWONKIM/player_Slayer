using System.Collections;
using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
    public GameObject skullPrefab;         // 해골 프리팹
    public float spawnRadius = 20f;        // 소환 반경
    public float spawnInterval = 3f;       // 소환 간격
    public int maxSkulls = 10;             // 최대 소환 수

    private int currentSkullCount = 0;

    [Header("Spawn Target")]
    public Transform ownerAgent; // 이 스포너가 소환한 해골이 추적할 에이전트

    void Start()
    {
        if (ownerAgent == null)
        {
            Debug.LogWarning("SkullSpawner의 ownerAgent가 설정되지 않았습니다!");
            return;
        }

        StartCoroutine(SpawnSkullsRoutine());
    }

    IEnumerator SpawnSkullsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentSkullCount >= maxSkulls) continue;

            SpawnSkull();
        }
    }

    void SpawnSkull()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0;
        Vector3 spawnPosition = ownerAgent.position + randomDirection;

        GameObject skull = Instantiate(skullPrefab, spawnPosition, Quaternion.identity);
        skull.tag = "Creature";
        currentSkullCount++;

        // 소환한 해골에게 타겟 설정
        Skull_RL skullScript = skull.GetComponent<Skull_RL>();
        if (skullScript != null)
        {
            skullScript.ownerAgent = ownerAgent;
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

        yield return new WaitForSeconds(2f);
        currentSkullCount--;
    }
}
