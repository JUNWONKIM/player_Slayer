using System.Collections;
using UnityEngine;

public class CreatureSpawner2 : MonoBehaviour
{
    public GameObject skullPrefab;           // 해골 프리팹
    public float spawnRadius = 50f;          // 스폰 거리
    public float spawnInterval = 3f;         // 스폰 간격
    public int maxCreatures = 4;             // 최대 해골 수

    private int currentCreatureCount = 0;
    private Transform targetAgent;

    public void SetTargetAgent(Transform agent)
    {
        targetAgent = agent;
    }

    void OnEnable()
    {
        currentCreatureCount = 0;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (targetAgent == null)
            {
                Debug.Log("[Spawner] No target agent.");
                continue;
            }

            if (currentCreatureCount < maxCreatures)
            {
                SpawnSkull();
                Debug.Log($"[Spawner] Skull spawned. Count: {currentCreatureCount}/{maxCreatures}");
            }
            else
            {
                Debug.Log("[Spawner] Max creatures reached. Skipping spawn.");
            }
        }
    }

    void SpawnSkull()
    {
        Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPos = targetAgent.position + new Vector3(offset.x, 0, offset.y);

        GameObject skull = Instantiate(skullPrefab, spawnPos, Quaternion.identity);
        skull.tag = "Creature";

        Skull_RL skullScript = skull.GetComponent<Skull_RL>();
        if (skullScript != null)
        {
            skullScript.ownerAgent = targetAgent;
            skullScript.spawnerOwner = this;
        }

        currentCreatureCount++;
    }

    public void NotifyCreatureDestroyed()
    {
        currentCreatureCount = Mathf.Max(0, currentCreatureCount - 1);
        Debug.Log($"[Spawner] Skull destroyed. Remaining: {currentCreatureCount}");
    }

    public void ResetSpawner()
    {
        currentCreatureCount = 0;
    }
}
