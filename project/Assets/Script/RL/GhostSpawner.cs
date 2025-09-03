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
            Debug.LogWarning("GhostSpawner: ownerAgent�� �������� �ʾҽ��ϴ�!");
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

            // ������Ʈ���� �Ÿ� ����
            if (Vector3.Distance(spawnPos, ownerAgent.position) < minDistanceFromPlayer)
            {
                attempts++;
                continue;
            }

            // ���� ��Ʈ����� �Ÿ� ����
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
            Debug.LogWarning($"GhostSpawner: {ghostCount}�� �� {spawned}���� ��ġ�� (�浹 ȸ�� ����)");
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
