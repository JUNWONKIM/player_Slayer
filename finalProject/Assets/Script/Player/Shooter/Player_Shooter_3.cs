using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player_Shooter_3 : MonoBehaviour
{
    public static Player_Shooter_3 instance;

    public GameObject swordPrefab; // 칼 프리팹
    public float summonInterval = 5f; // 칼 소환 간격
    public float distanceFromPlayer = 10f; // 플레이어로부터의 거리
    public float damageAmount = 0f;
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
                GameObject sword = Instantiate(swordPrefab, summonPosition, summonRotation);

                // 충돌 처리 컴포넌트를 동적으로 추가
                BulletCollisionHandler collisionHandler = sword.AddComponent<BulletCollisionHandler>();
                collisionHandler.damageAmount = damageAmount;
            }
            else
            {
                // 원의 반지름 계산
                float radius = distanceFromPlayer;

                for (int i = 0; i < swordNum; i++)
                {
                    // 각 칼의 각도 계산
                    float angle = i * (360f / swordNum);

                    // 칼의 소환 위치 계산
                    float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                    float z = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                    Vector3 summonPosition = player.transform.position + new Vector3(x, 0f, z);
                    Quaternion summonRotation = Quaternion.Euler(90f, 90f, angle);

                    // 칼 소환
                    GameObject sword = Instantiate(swordPrefab, summonPosition, summonRotation);

                    // 충돌 처리 컴포넌트를 동적으로 추가
                    BulletCollisionHandler collisionHandler = sword.AddComponent<BulletCollisionHandler>();
                    collisionHandler.damageAmount = damageAmount;
                }
            }
        }
    }




    public void IncreaseSwordNum()
    {

        swordNum++;
        Debug.Log("칼 개수 : " + swordNum);

    }

    public void IncreaseDamage(float amount)
    {
        // Player_Shooter_4 클래스의 damageAmount 값을 변경
        damageAmount += amount;
        Debug.Log("칼 데미지 : " + damageAmount);
    }


    public class BulletCollisionHandler : MonoBehaviour
    {
        public float damageAmount;
        private Player_Shooter_3 shooterInstance;

        // 생성자를 이용하여 Player_Shooter_4 인스턴스를 전달받음
        public BulletCollisionHandler(Player_Shooter_3 shooterInstance)
        {
            this.shooterInstance = shooterInstance;
        }

        void OnTriggerEnter(Collider other)
        {
            // 충돌한 객체의 태그가 "Creature"인 경우
            if (other.gameObject.CompareTag("Creature"))
            {
                // 충돌한 객체의 HP를 감소시킴
                CreatureHealth enemyHealth = other.gameObject.GetComponent<CreatureHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                }
            }
        }
     }
}
