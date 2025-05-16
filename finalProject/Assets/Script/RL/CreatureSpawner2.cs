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

        Vector3 spawnPos = targetAgent.position + moveDirection * spawnRadius;

        Vector2 offset = Random.insideUnitCircle.normalized * 3f;
        spawnPos += new Vector3(offset.x, 0f, offset.y);

        GameObject prefabToSpawn = Random.value < 0.5f ? skullPrefab : ghostPrefab;
        GameObject creature = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        creature.tag = "Creature";

        if (prefabToSpawn == skullPrefab)
        {
            var skull = creature.GetComponent<Skull_RL>();
            if (skull != null)
            {
                skull.ownerAgent = targetAgent;
                StartCoroutine(RemoveOnDeath(creature));
            }
        }
        else
        {
            var ghost = creature.GetComponent<Ghost_RL>();
            if (ghost != null)
            {
                ghost.ownerAgent = targetAgent;
                StartCoroutine(RemoveOnDeath(creature));
            }
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
