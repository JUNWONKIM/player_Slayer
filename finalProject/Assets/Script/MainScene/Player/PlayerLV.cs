using System.Collections.Generic;
using UnityEngine;

public class PlayerLV : MonoBehaviour
{
    public static PlayerLV instance;

    private static int creatureDeathCount = 0; // 죽은 크리처의 수
    public int LV = 1; // 현재 레벨
    public int killsForNextLevel = 10; // 다음 레벨까지 필요한 킬 수

    // 능력치 상승 폭
    public float fireRateIncrease = 0.8f;
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
      
        if (instance == null)
        {
            instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (creatureDeathCount >= killsForNextLevel) //죽은 크리쳐 수에 따른 레벨업
        {
            LevelUp();
        }
    }

    public static void IncrementCreatureDeathCount() //죽은 크리쳐 카운트
    {
        creatureDeathCount++;
        //Debug.Log("Creatures Killed: " + creatureDeathCount);
    }

    void LevelUp() //레벨업 처리
    {
        LV++;
        creatureDeathCount -= killsForNextLevel; // 현재 킬 카운트에서 필요 킬 수만큼 빼줌
        killsForNextLevel += 0; // 다음 레벨업에 필요한 킬 수 증가       
        IncreaseRandomStat(); // 레벨업 시 랜덤 능력치 증가

    }

    void IncreaseRandomStat() //랜덤 스탯 증가
    {

        //최대 상승치 제한
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

        //모든 스탯 최대 상승 시 멈춤
        if (availableStats.Count == 0)
        {   
            return;
        }

        int randomStat = availableStats[Random.Range(0, availableStats.Count)];

        switch (randomStat)
        {
            case 0: //발사 속도 증가
                Player_Shooter_1.instance.IncreaseFireRate(fireRateIncrease);
                Player_Shooter_4.instance.IncreaseFireRate(fireRateIncrease);
                fireRateIncreaseCount++;
                break;
            case 1: //용사 이동 속도 증가
                PlayerAI.instance.IncreaseMoveSpeed(moveSpeedIncrease);
                moveSpeedIncreaseCount++;
                break;
            case 2: //공격 1 강화
                Player_Shooter_1.instance.IncreaseProjectileCount(projectileCountIncrease_1);
                Player_Shooter_1.instance.IncreaseDamage(damageIncrease_1);
                damageAndProjectileIncreaseCount_1++;
                break;
            case 3: //공격 2 강화
                Player_Shooter_2.instance.IncreaseProjectileCount(projectileCountIncrease_2);
                Player_Shooter_2.instance.IncreaseDamage(damageIncrease_2);
                damageAndProjectileIncreaseCount_2++;
                break;
            case 4: //공격 3 강화
                Player_Shooter_3.instance.IncreaseSwordNum();
                Player_Shooter_3.instance.IncreaseDamage(damageIncrease_3);
                damageAndProjectileIncreaseCount_3++;
                break;
            case 5: //공격 4 강화
                Player_Shooter_4.instance.IncreaseBulletCount(projectileCountIncrease_4);
                Player_Shooter_4.instance.IncreaseDamage(damageIncrease_4);

                damageAndProjectileIncreaseCount_4++;
                break;
        }
    }
}
