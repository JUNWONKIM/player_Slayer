using System.Collections;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;
    public float spawnInterval = 3f;
    public int maxGhosts = 1;

    [Header("Spawn Target")]
    public Transform ownerAgent;

    [Header("Field Info")]
    public Transform field;
    public float fieldSize = 100f;
    public float minSpawnDistance = 10f;

    private bool isResetting = false;

    void Start()
    {
        if (ownerAgent == null || field == null)
        {
            Debug.LogWarning("GhostSpawner: ownerAgent 또는 field가 설정되지 않았습니다!");
            return;
        }

        StartCoroutine(SpawnGhostsRoutine());
    }

    IEnumerator SpawnGhostsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (isResetting) continue;

            int aliveGhosts = CountOwnedGhosts();
            if (aliveGhosts >= maxGhosts) continue;

            SpawnGhost();
        }
    }

    void SpawnGhost()
    {
        float halfSize = fieldSize * 0.5f;
        Vector3 spawnPosition;
        int attempt = 0;
        const int maxAttempts = 30;

        do
        {
            Vector3 offset = new Vector3(
                Random.Range(-halfSize, halfSize),
                0f,
                Random.Range(-halfSize, halfSize)
            );
            spawnPosition = field.position + offset;
            attempt++;

            if (attempt >= maxAttempts)
            {
                Debug.LogWarning("GhostSpawner: spawn 위치 찾기 실패");
                return;
            }

        } while (Vector3.Distance(spawnPosition, ownerAgent.position) < minSpawnDistance);

        GameObject ghost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        ghost.tag = "Creature";

        Ghost_RL ghostScript = ghost.GetComponent<Ghost_RL>();
        if (ghostScript != null)
        {
            ghostScript.ownerAgent = ownerAgent;
        }
    }

    public void ResetGhosts()
    {
        isResetting = true;

        foreach (var ghost in GameObject.FindGameObjectsWithTag("Creature"))
        {
            var script = ghost.GetComponent<Ghost_RL>();
            if (script != null && script.ownerAgent == ownerAgent)
            {
                Destroy(ghost);
            }
        }

        StartCoroutine(ResumeSpawningNextFrame());
    }

    IEnumerator ResumeSpawningNextFrame()
    {
        yield return null;
        isResetting = false;
    }

    int CountOwnedGhosts()
    {
        int count = 0;
        foreach (var ghost in GameObject.FindGameObjectsWithTag("Creature"))
        {
            var script = ghost.GetComponent<Ghost_RL>();
            if (script != null && script.ownerAgent == ownerAgent)
            {
                count++;
            }
        }
        return count;
    }
}
