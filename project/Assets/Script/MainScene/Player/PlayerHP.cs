using System.Collections;
using UnityEngine;
public class PlayerHP : MonoBehaviour
{
    public float hp = 1000f; //현재 체력
    public float max_hp = 1000f; //최대 체력
    public GameObject bossPrefab; // Boss 프리팹
    public float bossSpawnRadius = 200f; // Boss 생성 반경
    public UI_BossHp bossHpUI;

    private GameObject boss; // 현재 비활성화 되어 있는 Boss

    void Start()
    {
        hp = max_hp;

        //보스 프리팹 & 체력 슬라이더 생성 및 비활성화
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
        // HP가 최대 HP의 30% 이하일 때 Boss를 생성
        if (hp <= max_hp * 0.3f && boss != null && !boss.activeInHierarchy)
        {
            SpawnBossNearPlayer();
        }
    }

    public void TakeDamage(float damage) //용사 피해 입을 시 체력 감소
    {
        hp -= damage;
        Debug.Log("Player HP: " + hp);
    }

    void SpawnBossNearPlayer() //용사 근처에 보스 소환
    {
        // 용사의 위치를 가져옴
        Vector3 playerPosition = transform.position;

        // 보스를 용사 주변 랜덤 위치로 생성
        Vector3 randomPosition = playerPosition + new Vector3(
            Random.Range(-bossSpawnRadius, bossSpawnRadius),
            0,
            Random.Range(-bossSpawnRadius, bossSpawnRadius)
        );

        // y는 0으로 설정
        randomPosition.y = 0;

        // 보스 활성화
        boss.transform.position = randomPosition;
        boss.SetActive(true);

        // 슬라이더를 활성화하고 보스 체력을 UI에 설정
        if (bossHpUI != null && bossHpUI.hpSlider != null)
        {
            bossHpUI.hpSlider.gameObject.SetActive(true); // 슬라이더 활성화
            bossHpUI.SetBossHp(boss.GetComponent<BossHP>()); // 보스 체력 연결
        }
    }
} 
