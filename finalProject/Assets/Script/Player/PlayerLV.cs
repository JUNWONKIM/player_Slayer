using System.Collections.Generic;
using UnityEngine;

public class PlayerLV : MonoBehaviour
{
    public static PlayerLV instance;

    private static int creatureDeathCount = 0; // 죽은 크리처의 수

    // 능력치 상승 폭
   
    public float fireRateIncrease = 0.2f;
    public float moveSpeedIncrease = 30f;

    public int projectileCountIncrease_1 = 1; // 발사체 개수 증가
    public float damageIncrease_1 = 1;

    public int projectileCountIncrease_2 = 1; // 발사체 개수 증가
    public float damageIncrease_2 = 1;

    // 각 case의 실행 횟수를 추적하는 변수
    private int fireRateIncreaseCount = 0;
    private int moveSpeedIncreaseCount = 0;

    private int damageAndProjectileIncreaseCount_1 = 0;
    private int damageAndProjectileIncreaseCount_2 = 0;



    void Awake()
    {
        // 싱글톤 패턴을 사용하여 GameManager의 인스턴스를 유지
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (creatureDeathCount >= 1)
        {
            IncreaseRandomStat();
            creatureDeathCount -= 1; // 10마리 죽였으므로 카운트 초기화
        }
    }

    public static void IncrementCreatureDeathCount()
    {
        creatureDeathCount++;
        Debug.Log("Creatures Killed: " + creatureDeathCount);
    }

    void IncreaseRandomStat()
    {
        List<int> availableStats = new List<int>();

        if (fireRateIncreaseCount < 2)
            availableStats.Add(0);
        if (moveSpeedIncreaseCount < 2)
            availableStats.Add(1);
        if (damageAndProjectileIncreaseCount_1 < 2)
            availableStats.Add(2);
        if (damageAndProjectileIncreaseCount_2 < 2)
            availableStats.Add(3);

        if (availableStats.Count == 0)
        {
            Debug.Log("All stats have been increased 2 times.");
            return;
        }

        int randomStat = availableStats[Random.Range(0, availableStats.Count)];

        switch (randomStat)
        {
            case 0:
                Player_Shooter_1.instance.IncreaseFireRate(fireRateIncrease);
                fireRateIncreaseCount++;
                break;
            case 1:
                PlayerAI.instance.IncreaseMoveSpeed(moveSpeedIncrease);
                moveSpeedIncreaseCount++;
                break;
            case 2:
                Player_Atk_1.Instance.IncreaseDamage(damageIncrease_1);
                Player_Shooter_1.instance.IncreaseProjectileCount(projectileCountIncrease_1);
                damageAndProjectileIncreaseCount_1++;
                break;
            case 3:
                Player_Atk_2_1.Instance.IncreaseDamage(damageIncrease_2);
                Player_Shooter_2.instance.IncreaseProjectileCount(projectileCountIncrease_2);
                damageAndProjectileIncreaseCount_2++;
                break;

        }
    }
}
