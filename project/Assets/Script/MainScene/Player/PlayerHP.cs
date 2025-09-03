using System.Collections;
using UnityEngine;
public class PlayerHP : MonoBehaviour
{
    public float hp = 1000f; //���� ü��
    public float max_hp = 1000f; //�ִ� ü��
    public GameObject bossPrefab; // Boss ������
    public float bossSpawnRadius = 200f; // Boss ���� �ݰ�
    public UI_BossHp bossHpUI;

    private GameObject boss; // ���� ��Ȱ��ȭ �Ǿ� �ִ� Boss

    void Start()
    {
        hp = max_hp;

        //���� ������ & ü�� �����̴� ���� �� ��Ȱ��ȭ
        if (bossPrefab != null)
        {
            boss = Instantiate(bossPrefab);
            boss.SetActive(false);

            if (bossHpUI != null && bossHpUI.hpSlider != null)
            {
                bossHpUI.hpSlider.gameObject.SetActive(false);
            }
        }
    }
    void Update()
    {
        // HP�� �ִ� HP�� 30% ������ �� Boss�� ����
        if (hp <= max_hp * 0.3f && boss != null && !boss.activeInHierarchy)
        {
            SpawnBossNearPlayer();
        }
    }

    public void TakeDamage(float damage) //��� ���� ���� �� ü�� ����
    {
        hp -= damage;
        Debug.Log("Player HP: " + hp);
    }

    void SpawnBossNearPlayer() //��� ��ó�� ���� ��ȯ
    {
        // ����� ��ġ�� ������
        Vector3 playerPosition = transform.position;

        // ������ ��� �ֺ� ���� ��ġ�� ����
        Vector3 randomPosition = playerPosition + new Vector3(
            Random.Range(-bossSpawnRadius, bossSpawnRadius),
            0,
            Random.Range(-bossSpawnRadius, bossSpawnRadius)
        );

        // y�� 0���� ����
        randomPosition.y = 0;

        // ���� Ȱ��ȭ
        boss.transform.position = randomPosition;
        boss.SetActive(true);

        // �����̴��� Ȱ��ȭ�ϰ� ���� ü���� UI�� ����
        if (bossHpUI != null && bossHpUI.hpSlider != null)
        {
            bossHpUI.hpSlider.gameObject.SetActive(true); // �����̴� Ȱ��ȭ
            bossHpUI.SetBossHp(boss.GetComponent<BossHP>()); // ���� ü�� ����
        }
    }
} 
