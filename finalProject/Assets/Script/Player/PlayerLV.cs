using System.Collections.Generic;
using UnityEngine;

public class PlayerLV : MonoBehaviour
{
    public static PlayerLV instance;

    private static int creatureDeathCount = 0; // 죽은 크리처의 수
    private int level = 0; // 현재 레벨
    public int killsForNextLevel = 10; // 다음 레벨까지 필요한 킬 수

    // 능력치 상승 폭
    public float fireRateIncrease = 0.2f;
    public float moveSpeedIncrease = 30f;

    public int projectileCountIncrease_1 = 1; // 발사체 개수 증가
    public float damageIncrease_1 = 1;

    public int projectileCountIncrease_2 = 1; // 발사체 개수 증가
    public float damageIncrease_2 = 1;

    public int projectileCountIncrease_3 = 1; // 발사체 개수 증가
    public float damageIncrease_3 = 1;

    public int projectileCountIncrease_4 = 3; // 발사체 개수 증가
    public float damageIncrease_4 = 1;

    // 각 case의 실행 횟수를 추적하는 변수
    private int fireRateIncreaseCount = 0;
    private int moveSpeedIncreaseCount = 0;
    private int damageAndProjectileIncreaseCount_1 = 0;
    private int damageAndProjectileIncreaseCount_2 = 0;
    private int damageAndProjectileIncreaseCount_3 = 0;
    private int damageAndProjectileIncreaseCount_4 = 0;


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
        // 레벨업 조건 확인
        if (creatureDeathCount >= killsForNextLevel)
        {
            LevelUp();
        }
    }

    public static void IncrementCreatureDeathCount()
    {
        creatureDeathCount++;
        Debug.Log("Creatures Killed: " + creatureDeathCount);
    }

    void LevelUp()
    {
        level++;
        creatureDeathCount -= killsForNextLevel; // 현재 킬 카운트에서 필요 킬 수만큼 빼줌
        killsForNextLevel += 0; // 다음 레벨업에 필요한 킬 수 증가
        Debug.Log("Level Up! Current Level: " + level); // 레벨업 시 현재 레벨을 출력
        IncreaseRandomStat(); // 레벨업 시 랜덤 능력치 증가
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
        if (damageAndProjectileIncreaseCount_2 < 3)
            availableStats.Add(3);
        if (damageAndProjectileIncreaseCount_3 < 3)
            availableStats.Add(4);
        if (damageAndProjectileIncreaseCount_4 < 3)
            availableStats.Add(5);

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
                Player_Shooter_1.instance.IncreaseProjectileCount(projectileCountIncrease_1);
                Player_Shooter_1.instance.IncreaseDamage(damageIncrease_1);
                damageAndProjectileIncreaseCount_1++;
                break;
            case 3:
                Player_Shooter_2.instance.IncreaseProjectileCount(projectileCountIncrease_2);
                Player_Shooter_2.instance.IncreaseDamage(damageIncrease_2);
                damageAndProjectileIncreaseCount_2++;
                break;
            case 4:
                Player_Shooter_3.instance.IncreaseSwordNum();
                Player_Shooter_3.instance.IncreaseDamage(damageIncrease_2);
                damageAndProjectileIncreaseCount_3++;
                break;
            case 5:
                Player_Shooter_4.instance.IncreaseBulletCount(projectileCountIncrease_4);
                Player_Shooter_4.instance.IncreaseDamage(damageIncrease_4);

                damageAndProjectileIncreaseCount_4++;
                break;
        }
    }
}
