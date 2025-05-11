using System.Collections;
using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
    public GameObject skullPrefab;         // �ذ� ������
    public Transform player;               // �÷��̾�(������Ʈ) Ʈ������
    public float spawnRadius = 20f;        // �÷��̾�κ��� �� m ������ ��ġ�� ��ȯ����
    public float spawnInterval = 3f;       // �ذ� ��ȯ ����
    public int maxSkulls = 10;             // ���ÿ� ������ �� �ִ� �ذ� �� ����

    private int currentSkullCount = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        StartCoroutine(SpawnSkullsRoutine());
    }

    IEnumerator SpawnSkullsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // �ִ� �� ����
            if (currentSkullCount >= maxSkulls) continue;

            SpawnSkull();
        }
    }

    void SpawnSkull()
    {
        // �÷��̾� ��ó ���� ��ġ ���
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0;
        Vector3 spawnPosition = player.position + randomDirection;

        GameObject skull = Instantiate(skullPrefab, spawnPosition, Quaternion.identity);
        skull.tag = "Creature";  // Ȥ�ö� �����տ� �±װ� �Ⱥپ� �ִٸ�
        currentSkullCount++;

        // �ذ��� ������ ī��Ʈ ���� (Skull ��ũ��Ʈ�� �߰� �ʿ�)
        Skull skullScript = skull.GetComponent<Skull>();
        if (skullScript != null)
        {
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

        // �ذ� ������Ʈ�� ������ ���� �ð� �� �����ϰ� ī��Ʈ ���̱�
        yield return new WaitForSeconds(2f);
        currentSkullCount--;
    }
}
