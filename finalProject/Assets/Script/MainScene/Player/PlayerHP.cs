using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public float hp = 1000f;
    public float max_hp = 1000f;
    public GameObject bossPrefab; // Boss 프리팹
    public float bossSpawnRadius = 200f; // Boss 생성 반경
    private GameObject boss; // 현재 비활성화 되어 있는 Boss
    public UI_BossHp uiBossHp; // UI_BossHp 스크립트 참조

    void Start()
    {
        hp = max_hp;

        if (bossPrefab != null)
        {
            boss = Instantiate(bossPrefab);
            boss.SetActive(false); // 초기에는 비활성화
        }
    }

    void Update()
    {
        if (hp <= max_hp * 0.3f && boss != null && !boss.activeInHierarchy)
        {
            SpawnBossNearPlayer();
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log("Player HP: " + hp);
    }

    public void SpawnBossNearPlayer()
    {
        // 플레이어의 위치를 가져옴
        Vector3 playerPosition = transform.position;

        // Boss를 플레이어 주변 200 유닛 내의 랜덤 위치로 생성
        Vector3 randomPosition = playerPosition + new Vector3(
            Random.Range(-bossSpawnRadius, bossSpawnRadius),
            0,
            Random.Range(-bossSpawnRadius, bossSpawnRadius)
        );

        // y는 0으로 설정
        randomPosition.y = 0;

        // Boss를 활성화하고 위치를 설정
        boss.transform.position = randomPosition;
        boss.SetActive(true);

        // 슬라이더에 보스 체력 설정
        UI_BossHp uiBossHp = FindObjectOfType<UI_BossHp>();
        if (uiBossHp != null)
        {
            CreatureHealth bossHealth = boss.GetComponent<CreatureHealth>();
            uiBossHp.SetBossHealth(bossHealth);
        }

        // Boss가 생성되면 더 이상 생성하지 않도록
        enabled = false;
    }


}
