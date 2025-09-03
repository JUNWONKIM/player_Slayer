using System.Collections.Generic;
using UnityEngine;

public class PlayerLV : MonoBehaviour
{
    public static PlayerLV instance;

    private static int creatureDeathCount = 0; // ���� ũ��ó�� ��
    public int LV = 1; // ���� ����
    public int killsForNextLevel = 10; // ���� �������� �ʿ��� ų ��

    // �ɷ�ġ ��� ��
    public float fireRateIncrease = 0.8f;
    public float moveSpeedIncrease = 30f;

    public int projectileCountIncrease_1 = 1; // �߻�ü ���� ����
    public float damageIncrease_1 = 1;

    public int projectileCountIncrease_2 = 1; // �߻�ü ���� ����
    public float damageIncrease_2 = 1;

    public int projectileCountIncrease_3 = 1; // �߻�ü ���� ����
    public float damageIncrease_3 = 1;

    public int projectileCountIncrease_4 = 3; // �߻�ü ���� ����
    public float damageIncrease_4 = 1;

    // �� case�� ���� Ƚ���� �����ϴ� ����
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
        if (creatureDeathCount >= killsForNextLevel) //���� ũ���� ���� ���� ������
        {
            LevelUp();
        }
    }

    public static void IncrementCreatureDeathCount() //���� ũ���� ī��Ʈ
    {
        creatureDeathCount++;
        //Debug.Log("Creatures Killed: " + creatureDeathCount);
    }

    void LevelUp() //������ ó��
    {
        LV++;
        creatureDeathCount -= killsForNextLevel; // ���� ų ī��Ʈ���� �ʿ� ų ����ŭ ����
        killsForNextLevel += 0; // ���� �������� �ʿ��� ų �� ����       
        IncreaseRandomStat(); // ������ �� ���� �ɷ�ġ ����

    }

    void IncreaseRandomStat() //���� ���� ����
    {

        //�ִ� ���ġ ����
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

        //��� ���� �ִ� ��� �� ����
        if (availableStats.Count == 0)
        {   
            return;
        }

        int randomStat = availableStats[Random.Range(0, availableStats.Count)];

        switch (randomStat)
        {
            case 0: //�߻� �ӵ� ����
                Player_Shooter_1.instance.IncreaseFireRate(fireRateIncrease);
                Player_Shooter_4.instance.IncreaseFireRate(fireRateIncrease);
                fireRateIncreaseCount++;
                break;
            case 1: //��� �̵� �ӵ� ����
                PlayerAI.instance.IncreaseMoveSpeed(moveSpeedIncrease);
                moveSpeedIncreaseCount++;
                break;
            case 2: //���� 1 ��ȭ
                Player_Shooter_1.instance.IncreaseProjectileCount(projectileCountIncrease_1);
                Player_Shooter_1.instance.IncreaseDamage(damageIncrease_1);
                damageAndProjectileIncreaseCount_1++;
                break;
            case 3: //���� 2 ��ȭ
                Player_Shooter_2.instance.IncreaseProjectileCount(projectileCountIncrease_2);
                Player_Shooter_2.instance.IncreaseDamage(damageIncrease_2);
                damageAndProjectileIncreaseCount_2++;
                break;
            case 4: //���� 3 ��ȭ
                Player_Shooter_3.instance.IncreaseSwordNum();
                Player_Shooter_3.instance.IncreaseDamage(damageIncrease_3);
                damageAndProjectileIncreaseCount_3++;
                break;
            case 5: //���� 4 ��ȭ
                Player_Shooter_4.instance.IncreaseBulletCount(projectileCountIncrease_4);
                Player_Shooter_4.instance.IncreaseDamage(damageIncrease_4);

                damageAndProjectileIncreaseCount_4++;
                break;
        }
    }
}
