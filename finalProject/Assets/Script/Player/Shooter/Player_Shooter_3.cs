using System.Collections;
using UnityEngine;

public class Player_Shooter_3 : MonoBehaviour
{
    public static Player_Shooter_3 instance;

    public GameObject swordPrefab; // 칼 프리팹
    public float summonInterval = 5f; // 칼 소환 간격
    public float distanceFromPlayer = 30f; // 플레이어로부터의 거리
    public int swordNum = 0;
    // Start is called before the first frame update

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        // 일정 간격마다 칼을 소환하는 코루틴 시작
        InvokeRepeating("SummonSword", 0f, summonInterval);
    }

    void SummonSword()
    {
        // 플레이어 게임오브젝트 가져오기
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {

            if (swordNum == 1)
            {
                // 플레이어 위치와 칼의 소환 위치 계산
                Vector3 summonPosition = player.transform.position + new Vector3(distanceFromPlayer, 0f, 0f);
                Quaternion summonRotation = Quaternion.Euler(90f, 90f, 0f);

                // 칼 소환
                Instantiate(swordPrefab, summonPosition, summonRotation);
            }

            if (swordNum == 2)
            {
                // 플레이어 위치와 칼의 소환 위치 계산
                Vector3 summonPosition = player.transform.position + new Vector3(distanceFromPlayer, 0f, 0f);
                Quaternion summonRotation = Quaternion.Euler(90f, 90f, 0f);
                Vector3 summonPosition2 = player.transform.position + new Vector3(-distanceFromPlayer, 0f, 0f);
                Quaternion summonRotation2 = Quaternion.Euler(90f, 90f, 0f);

                // 칼 소환
                Instantiate(swordPrefab, summonPosition, summonRotation);
                Instantiate(swordPrefab, summonPosition2, summonRotation2);
            }

            if (swordNum == 3)
            {
                // 각도 간격 설정
                float angleInterval = 360f / 3f;
                float currentAngle = 0f;

                for (int i = 0; i < swordNum; i++)
                {
                    // 플레이어 위치와 칼의 소환 위치 계산
                    float x = distanceFromPlayer * Mathf.Cos(currentAngle * Mathf.Deg2Rad);
                    float z = distanceFromPlayer * Mathf.Sin(currentAngle * Mathf.Deg2Rad);
                    Vector3 summonPosition = player.transform.position + new Vector3(x, 0f, z);
                    Quaternion summonRotation = Quaternion.Euler(90f, 90f, 0f);

                    // 칼 소환
                    Instantiate(swordPrefab, summonPosition, summonRotation);

                    // 다음 칼을 소환하기 위해 각도 증가
                    currentAngle += angleInterval;
                }

            }
        }
    }
    public void IncreaseSwordNum()
    {

        swordNum++;
        Debug.Log("칼 개수 : " + swordNum);

    }
}
