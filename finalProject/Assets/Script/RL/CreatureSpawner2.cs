using UnityEngine;
using System.Collections;

public class CreatureSpawner2 : MonoBehaviour
{
    public GameObject skullPrefab;
    public GameObject ghostPrefab;
    public float spawnRadius = 20f;
    public float spawnInterval = 2f;
    public int maxCreatures = 20;

    private int currentCreatureCount = 0;
    private Transform targetAgent;

    private Vector3 lastAgentPosition;
    private Vector3 moveDirection;

    public void SetTargetAgent(Transform agent)
    {
        targetAgent = agent;
        lastAgentPosition = targetAgent.position;
    }

    void OnEnable()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitUntil(() => targetAgent != null);
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // ✅ 시작하자마자 1마리 즉시 소환
        if (targetAgent != null && currentCreatureCount < maxCreatures)
        {
            SpawnCreatureInFrontOfAgent();
        }

        // 이후 인터벌대로 반복 소환
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (targetAgent == null || currentCreatureCount >= maxCreatures) continue;

            SpawnCreatureInFrontOfAgent();
        }
    }


    void SpawnCreatureInFrontOfAgent()
    {
        moveDirection = (targetAgent.position - lastAgentPosition).normalized;
        lastAgentPosition = targetAgent.position;

        if (moveDirection == Vector3.zero)
            moveDirection = Random.onUnitSphere;

        moveDirection.y = 0;

        // ✅ 이동 방향에 약간의 각도 노이즈 추가 (±15도 정도)
        float angleOffset = Random.Range(-30f, 30f); // 각도 범위
        Quaternion rotation = Quaternion.Euler(0f, angleOffset, 0f);
        Vector3 randomizedDirection = rotation * moveDirection;

        Vector3 spawnPos = targetAgent.position + randomizedDirection.normalized * spawnRadius;

        // 좌우 랜덤 offset (필요하면 유지)
        Vector2 offset = Random.insideUnitCircle.normalized * 2f;
        spawnPos += new Vector3(offset.x, 0f, offset.y);

        GameObject creature = Instantiate(skullPrefab, spawnPos, Quaternion.identity);
        creature.tag = "Creature";

        var skull = creature.GetComponent<Skull_RL>();
        if (skull != null)
        {
            skull.ownerAgent = targetAgent;
            StartCoroutine(RemoveOnDeath(creature));
        }

        currentCreatureCount++;
    }


    IEnumerator RemoveOnDeath(GameObject creature)
    {
        Animator animator = creature.GetComponent<Animator>();
        while (animator != null && !animator.GetBool("isDie"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        currentCreatureCount--;
    }

    public void ResetSpawner()
    {
        currentCreatureCount = 0;
    }
}
