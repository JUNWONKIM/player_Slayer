using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public float hp = 1000f;
    public float max_hp = 1000f;
    public GameObject bossPrefab; // Boss 프리팹
    public float bossSpawnRadius = 200f; // Boss 생성 반경
    private GameObject boss; // 현재 비활성화 되어 있는 Boss

    public UI_BossHp bossHpUI; // UI_BossHp 스크립트 참조

    void Start()
    {
        hp = max_hp;

        // Boss 프리팹을 미리 생성하고 비활성화 상태로 설정
        if (bossPrefab != null)
        {
            boss = Instantiate(bossPrefab);
            boss.SetActive(false); // 처음에는 비활성화
        }

        // 슬라이더도 처음에는 비활성화
        if (bossHpUI != null && bossHpUI.healthSlider != null)
        {
            bossHpUI.healthSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // HP가 최대 HP의 30% 이하일 때 Boss를 생성
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

    void SpawnBossNearPlayer()
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

        // 슬라이더를 활성화하고 보스 체력을 UI에 설정
        if (bossHpUI != null && bossHpUI.healthSlider != null)
        {
            bossHpUI.healthSlider.gameObject.SetActive(true); // 슬라이더 활성화
            bossHpUI.SetBossHealth(boss.GetComponent<CreatureHealth>()); // 보스 체력 연결
        }
    }
}
