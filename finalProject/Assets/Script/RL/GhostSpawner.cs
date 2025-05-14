using System.Collections;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;         // 고스트 프리팹
    public float spawnRadius = 20f;        // 소환 반경
    public float spawnInterval = 3f;       // 소환 간격
    public int maxGhosts = 10;             // 최대 소환 수

    private int currentGhostCount = 0;

    [Header("Spawn Target")]
    public Transform ownerAgent; // 이 스포너가 소환한 고스트가 추적할 에이전트

    void Start()
    {
        if (ownerAgent == null)
            ownerAgent = transform.parent; // 부모를 에이전트로 자동 지정

        if (ownerAgent == null)
        {
            Debug.LogWarning("GhostSpawner의 ownerAgent가 설정되지 않았습니다!");
            return;
        }

        StartCoroutine(SpawnGhostsRoutine());
    }

    IEnumerator SpawnGhostsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentGhostCount >= maxGhosts) continue;

            SpawnGhost();
        }
    }

    void SpawnGhost()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0;
        Vector3 spawnPosition = ownerAgent.position + randomDirection;

        GameObject ghost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        ghost.tag = "Creature";
        currentGhostCount++;

        // 고스트에게 타겟 설정
        Ghost_RL ghostScript = ghost.GetComponent<Ghost_RL>();
        if (ghostScript != null)
        {
            ghostScript.ownerAgent = ownerAgent;
            StartCoroutine(RemoveOnDeath(ghost));
        }
    }

    IEnumerator RemoveOnDeath(GameObject ghost)
    {
        Animator animator = ghost.GetComponent<Animator>();
        while (animator != null && !animator.GetBool("isDie"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        currentGhostCount--;
    }
}