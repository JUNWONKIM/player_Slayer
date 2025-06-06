using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;

public class CreatureSpawner2 : MonoBehaviour
{
    public GameObject skullPrefab;
    public GameObject ghostPrefab; // 👈 고스트 프리팹 추가

    public float spawnRadius = 30f;
    public float spawnInterval = 1.5f;
    public int maxCreatures = 3;

    private Transform targetAgent;
    private Vector3 previousAgentPos;
    private float spawnTimer = 0f;

    public List<GameObject> spawnedCreatures = new List<GameObject>();
    public List<GameObject> spawnedBullets = new List<GameObject>(); // (옵션)

    private float curriculumSkullSpeed = -1f;

    void Start()
    {
        float skullSpeed = Academy.Instance.EnvironmentParameters.GetWithDefault("skullSpeed", 22f);
        float spawnInterval = Academy.Instance.EnvironmentParameters.GetWithDefault("spawnInterval", 3f);
        int maxSkulls = Mathf.FloorToInt(Academy.Instance.EnvironmentParameters.GetWithDefault("maxSkulls", 3));

        SetCurriculum(skullSpeed, spawnInterval, maxSkulls);
    }

    public void SetTargetAgent(Transform agent)
    {
        targetAgent = agent;
        previousAgentPos = agent.position;
        ResetSpawner();
    }

    public void ResetSpawner()
    {
        spawnTimer = 0f;
        CleanupDestroyedObjects();

        // 👇 실제 GameObject도 파괴
        foreach (var c in spawnedCreatures)
        {
            if (c != null)
                GameObject.Destroy(c);
        }

        foreach (var b in spawnedBullets)
        {
            if (b != null)
                GameObject.Destroy(b);
        }

        spawnedCreatures.Clear();
        spawnedBullets.Clear();
    }


    void Update()
    {
        if (targetAgent == null) return;

        spawnTimer += Time.deltaTime;
        CleanupDestroyedObjects();

        if (spawnTimer >= spawnInterval && spawnedCreatures.Count < maxCreatures)
        {
            spawnTimer -= spawnInterval;
            SpawnSkullOrGhost(); // 👈 랜덤으로 소환
        }
    }

    private void CleanupDestroyedObjects()
    {
        spawnedCreatures.RemoveAll(c => c == null);
        spawnedBullets.RemoveAll(b => b == null);
    }

    private void SpawnSkullOrGhost()
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

        // 👇 랜덤하게 해골 또는 고스트 선택
        GameObject prefabToSpawn = Random.value < 0.5f ? skullPrefab : ghostPrefab;
        GameObject creature = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        if (creature.TryGetComponent(out Skull_RL skull))
        {
            skull.ownerAgent = targetAgent;
            skull.spawnerOwner = this;
            if (curriculumSkullSpeed > 0f)
                skull.moveSpeed = curriculumSkullSpeed;
        }
        else if (creature.TryGetComponent(out Ghost_RL ghost))
        {
            ghost.ownerAgent = targetAgent;
            ghost.spawnerOwner = this;
        }

        spawnedCreatures.Add(creature);
    }

    public GameObject GetNearestCreature()
    {
        if (targetAgent == null || spawnedCreatures.Count == 0) return null;

        return spawnedCreatures
            .Where(c => c != null)
            .OrderBy(c => Vector3.Distance(targetAgent.position, c.transform.position))
            .FirstOrDefault();
    }

    public GameObject[] GetNearestCreatures(int count)
    {
        return spawnedCreatures
            .Where(c => c != null)
            .OrderBy(c => Vector3.Distance(targetAgent.position, c.transform.position))
            .Take(count)
            .ToArray();
    }

    public GameObject[] GetNearestBullets(int count)
    {
        return spawnedBullets
            .Where(b => b != null)
            .OrderBy(b => Vector3.Distance(targetAgent.position, b.transform.position))
            .Take(count)
            .ToArray();
    }

    public void SetCurriculum(float skullSpeed, float spawnInterval, int maxSkulls)
    {
        this.curriculumSkullSpeed = skullSpeed;
        this.spawnInterval = spawnInterval;
        this.maxCreatures = maxSkulls;
    }
}
