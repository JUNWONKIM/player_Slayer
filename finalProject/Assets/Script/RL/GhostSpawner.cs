using System.Collections.Generic;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;
    public int ghostCount = 4;
    public float spawnRadius = 40f;
    public float minGhostDistance = 5f;
    public float minDistanceFromPlayer = 10f;

    [Header("Spawn Target")]
    public Transform ownerAgent;

    private List<GameObject> spawnedGhosts = new List<GameObject>();

    void Start()
    {
        if (ownerAgent == null)
        {
            Debug.LogWarning("GhostSpawner: ownerAgent가 설정되지 않았습니다!");
            return;
        }

        SpawnInitialGhosts();
    }

    void SpawnInitialGhosts()
    {
        int spawned = 0;
        int attempts = 0;
        const int maxAttempts = 100;

        while (spawned < ghostCount && attempts < maxAttempts)
        {
            Vector3 offset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0f,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Vector3 spawnPos = ownerAgent.position + offset;

            // 에이전트와의 거리 조건
            if (Vector3.Distance(spawnPos, ownerAgent.position) < minDistanceFromPlayer)
            {
                attempts++;
                continue;
            }

            // 기존 고스트들과의 거리 조건
            bool tooClose = false;
            foreach (var ghost in spawnedGhosts)
            {
                if (Vector3.Distance(ghost.transform.position, spawnPos) < minGhostDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                GameObject ghost = Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
                ghost.tag = "Creature";

                Ghost_RL ghostScript = ghost.GetComponent<Ghost_RL>();
                if (ghostScript != null)
                {
                    ghostScript.ownerAgent = ownerAgent;
                }

                spawnedGhosts.Add(ghost);
                spawned++;
            }

            attempts++;
        }

        if (spawned < ghostCount)
        {
            Debug.LogWarning($"GhostSpawner: {ghostCount}개 중 {spawned}개만 배치됨 (충돌 회피 실패)");
        }
    }

    public void ResetGhosts()
    {
        foreach (var ghost in spawnedGhosts)
        {
            if (ghost != null)
            {
                Destroy(ghost);
            }
        }
        spawnedGhosts.Clear();
        SpawnInitialGhosts();
    }
}
