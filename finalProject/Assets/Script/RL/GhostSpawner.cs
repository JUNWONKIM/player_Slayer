using System.Collections;
using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    public GameObject ghostPrefab;         // ��Ʈ ������
    public float spawnRadius = 20f;        // ��ȯ �ݰ�
    public float spawnInterval = 3f;       // ��ȯ ����
    public int maxGhosts = 10;             // �ִ� ��ȯ ��

    private int currentGhostCount = 0;

    [Header("Spawn Target")]
    public Transform ownerAgent; // �� �����ʰ� ��ȯ�� ��Ʈ�� ������ ������Ʈ

    void Start()
    {
        if (ownerAgent == null)
            ownerAgent = transform.parent; // �θ� ������Ʈ�� �ڵ� ����

        if (ownerAgent == null)
        {
            Debug.LogWarning("GhostSpawner�� ownerAgent�� �������� �ʾҽ��ϴ�!");
            return;
        }

        StartCoroutine(SpawnGhostsRoutine());
    }

    IEnumerator SpawnGhostsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentGhostCount >= maxGhosts) continue;

            SpawnGhost();
        }
    }

    void SpawnGhost()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0;
        Vector3 spawnPosition = ownerAgent.position + randomDirection;

        GameObject ghost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        ghost.tag = "Creature";
        currentGhostCount++;

        // ��Ʈ���� Ÿ�� ����
        Ghost_RL ghostScript = ghost.GetComponent<Ghost_RL>();
        if (ghostScript != null)
        {
            ghostScript.ownerAgent = ownerAgent;
            StartCoroutine(RemoveOnDeath(ghost));
        }
    }

    IEnumerator RemoveOnDeath(GameObject ghost)
    {
        Animator animator = ghost.GetComponent<Animator>();
        while (animator != null && !animator.GetBool("isDie"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        currentGhostCount--;
    }
}