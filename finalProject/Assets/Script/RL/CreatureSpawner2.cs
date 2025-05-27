using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;

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
    public List<GameObject> spawnedBullets = new List<GameObject>(); // (옵션)

    private float curriculumSkullSpeed = -1f; // default -1이면 prefab 값 사용

    void Start()
    {
        float skullSpeed = Academy.Instance.EnvironmentParameters.GetWithDefault("skullSpeed", 13f);
        float spawnInterval = Academy.Instance.EnvironmentParameters.GetWithDefault("spawnInterval", 3f);
        int maxSkulls = Mathf.FloorToInt(Academy.Instance.EnvironmentParameters.GetWithDefault("maxSkulls", 2f));

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
        spawnedCreatures.Clear();
        spawnedBullets.Clear(); // (옵션)
    }

    void Update()
    {
        if (targetAgent == null) return;

        spawnTimer += Time.deltaTime;
        CleanupDestroyedObjects();

        if (spawnTimer >= spawnInterval && spawnedCreatures.Count < maxCreatures)
        {
            spawnTimer -= spawnInterval;
            SpawnSkull();
        }
    }

    private void CleanupDestroyedObjects()
    {
        spawnedCreatures.RemoveAll(c => c == null);
        spawnedBullets.RemoveAll(b => b == null); // (옵션)
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

        var skr = skull.GetComponent<Skull_RL>();
        if (skr != null)
        {
            skr.ownerAgent = targetAgent;
            skr.spawnerOwner = this;

            // 커리큘럼 적용: 속도 반영
            if (curriculumSkullSpeed > 0f)
                skr.moveSpeed = curriculumSkullSpeed;
        }

        spawnedCreatures.Add(skull);
    }

    // ✅ 가장 가까운 적 1개 반환
    public GameObject GetNearestCreature()
    {
        if (targetAgent == null || spawnedCreatures.Count == 0) return null;

        return spawnedCreatures
            .Where(c => c != null)
            .OrderBy(c => Vector3.Distance(targetAgent.position, c.transform.position))
            .FirstOrDefault();
    }

    // ✅ 주변 크리처 여러 개 반환
    public GameObject[] GetNearestCreatures(int count)
    {
        return spawnedCreatures
            .Where(c => c != null)
            .OrderBy(c => Vector3.Distance(targetAgent.position, c.transform.position))
            .Take(count)
            .ToArray();
    }

    // ✅ 주변 탄환 관측 (옵션)
    public GameObject[] GetNearestBullets(int count)
    {
        return spawnedBullets
            .Where(b => b != null)
            .OrderBy(b => Vector3.Distance(targetAgent.position, b.transform.position))
            .Take(count)
            .ToArray();
    }

    // ✅ 커리큘럼 적용 메서드
    public void SetCurriculum(float skullSpeed, float spawnInterval, int maxSkulls)
    {
        this.curriculumSkullSpeed = skullSpeed;
        this.spawnInterval = spawnInterval;
        this.maxCreatures = maxSkulls;
    }
}
