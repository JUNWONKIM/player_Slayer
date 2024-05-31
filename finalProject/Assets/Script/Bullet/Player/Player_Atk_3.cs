using UnityEngine;

public class Player_Atk_3 : MonoBehaviour
{
    public float rotationSpeed = 180f; // 칼의 회전 속도 (Y축 기준)
    public float distanceFromPlayer = 2f; // 플레이어로부터의 거리
    public float lifetime = 3f;
    public float damageAmount = 1f;

    private GameObject player; // 플레이어 오브젝트

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
        // Player 태그를 가진 오브젝트를 찾아서 해당 오브젝트를 저장
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // 칼이 플레이어를 바라보도록 설정
            transform.LookAt(player.transform);

            // 칼을 플레이어 주위를 중심으로 회전시키는 코드
            transform.RotateAround(player.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);

            // 플레이어 위치를 기준으로 한 방향 벡터 계산
            Vector3 directionFromPlayer = transform.position - player.transform.position;
            directionFromPlayer.y = 0f; // y 축 방향은 고려하지 않음

            // 플레이어로부터 일정 거리를 유지하기 위한 위치 계산
            Vector3 targetPosition = player.transform.position + directionFromPlayer.normalized * distanceFromPlayer;

            // 칼을 플레이어로부터 일정 거리를 유지하면서 이동
            transform.position = targetPosition;

            transform.Rotate(Vector3.right, -90f);
            
        }
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

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("칼 데미지 :  " + damageAmount);
    }
}
