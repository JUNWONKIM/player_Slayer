using System.Collections;
using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
    public GameObject skullPrefab;         // �ذ� ������
    public float spawnRadius = 20f;        // ��ȯ �ݰ�
    public float spawnInterval = 3f;       // ��ȯ ����
    public int maxSkulls = 10;             // �ִ� ��ȯ ��

    private int currentSkullCount = 0;

    [Header("Spawn Target")]
    public Transform ownerAgent; // �� �����ʰ� ��ȯ�� �ذ��� ������ ������Ʈ

    void Start()
    {
        if (ownerAgent == null)
        {
            Debug.LogWarning("SkullSpawner�� ownerAgent�� �������� �ʾҽ��ϴ�!");
            return;
        }

        StartCoroutine(SpawnSkullsRoutine());
    }

    IEnumerator SpawnSkullsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (currentSkullCount >= maxSkulls) continue;

            SpawnSkull();
        }
    }

    void SpawnSkull()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0;
        Vector3 spawnPosition = ownerAgent.position + randomDirection;

        GameObject skull = Instantiate(skullPrefab, spawnPosition, Quaternion.identity);
        skull.tag = "Creature";
        currentSkullCount++;

        // ��ȯ�� �ذ񿡰� Ÿ�� ����
        Skull_RL skullScript = skull.GetComponent<Skull_RL>();
        if (skullScript != null)
        {
            skullScript.ownerAgent = ownerAgent;
            skullScript.StartCoroutine(RemoveOnDeath(skull));
        }
    }

    IEnumerator RemoveOnDeath(GameObject skull)
    {
        Animator animator = skull.GetComponent<Animator>();
        while (animator != null && !animator.GetBool("isDie"))
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        currentSkullCount--;
    }
}
