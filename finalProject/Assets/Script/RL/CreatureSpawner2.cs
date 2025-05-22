using System.Collections.Generic;
using UnityEngine;

public class CreatureSpawner2 : MonoBehaviour
{
    public GameObject skullPrefab;
    public float spawnRadius = 30f;
    public float spawnInterval = 1.5f;
    public int maxCreatures = 3;

    private Transform targetAgent;
    private Vector3 previousAgentPos;
    private float spawnTimer = 0f;

    public List<GameObject> spawnedCreatures = new List<GameObject>();

    public void SetTargetAgent(Transform agent)
    {
        targetAgent = agent;
        previousAgentPos = agent.position;
        ResetSpawner();
    }

    public void ResetSpawner()
    {
        spawnTimer = 0f;
        CleanupDestroyedCreatures();
        spawnedCreatures.Clear();
    }

    void Update()
    {
        if (targetAgent == null) return;

        spawnTimer += Time.deltaTime;
        CleanupDestroyedCreatures();

        if (spawnTimer >= spawnInterval && spawnedCreatures.Count < maxCreatures)
        {
            spawnTimer -= spawnInterval;
            SpawnSkull();
        }
    }

    private void CleanupDestroyedCreatures()
    {
        // 리스트에서 null(파괴된) 객체 제거
        spawnedCreatures.RemoveAll(creature => creature == null);
    }

    private void SpawnSkull()
    {
        Vector3 currentPos = targetAgent.position;
        Vector3 moveDir = (currentPos - previousAgentPos).normalized;
        if (moveDir.sqrMagnitude < 0.001f)
            moveDir = targetAgent.forward;
        previousAgentPos = currentPos;

        Quaternion rot = Quaternion.Euler(0f, Random.Range(-30f, 30f), 0f);
        Vector3 spawnDir = rot * moveDir;
        Vector3 spawnPos = currentPos + spawnDir * spawnRadius;
        spawnPos += new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        GameObject skull = Instantiate(skullPrefab, spawnPos, Quaternion.identity);
        skull.tag = "Creature";

        var skr = skull.GetComponent<Skull_RL>();
        if (skr != null)
        {
            skr.ownerAgent = targetAgent;
            skr.spawnerOwner = this;
        }

        spawnedCreatures.Add(skull);
    }
}
