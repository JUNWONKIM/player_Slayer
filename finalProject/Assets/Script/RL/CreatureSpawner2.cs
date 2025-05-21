using System.Collections;
using UnityEngine;

public class CreatureSpawner2 : MonoBehaviour
{
    public GameObject skullPrefab;
    public float spawnRadius = 30f;
    public float spawnInterval = 1.5f;
    public int maxCreatures = 5;

    private int currentCreatureCount = 0;
    private Transform targetAgent;
    private Vector3 previousAgentPos;

    public void SetTargetAgent(Transform agent)
    {
        targetAgent = agent;
        previousAgentPos = agent.position; // 최초 위치 저장
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
            if (targetAgent != null && currentCreatureCount < maxCreatures)
            {
                SpawnSkull();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSkull()
    {
        Vector3 currentPos = targetAgent.position;
        Vector3 moveDir = (currentPos - previousAgentPos).normalized;

        if (moveDir.magnitude < 0.1f)
        {
            moveDir = targetAgent.forward; // 거의 안 움직이면 앞 방향 fallback
        }

        previousAgentPos = currentPos;

        Quaternion rotation = Quaternion.Euler(0, Random.Range(-30f, 30f), 0);
        Vector3 spawnDir = rotation * moveDir;

        Vector3 spawnPos = currentPos + spawnDir.normalized * spawnRadius;
        spawnPos += new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

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
    }

    public void ResetSpawner()
    {
        currentCreatureCount = 0;
    }
}
