using UnityEngine;

public class Skull : MonoBehaviour
{
    public float moveSpeed = 5f; // 적의 이동 속도
    private Transform player; // 플레이어의 위치
    private Rigidbody rb; // 적의 Rigidbody 컴포넌트
    private Animator animator; // 적의 Animator 컴포넌트

    void Start()
    {
        // 플레이어 게임 오브젝트를 찾아 트랜스폼을 할당
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    void FixedUpdate()
    {
        // isDie가 false인 경우에만 이동 및 회전 실행
        if (!animator.GetBool("isDie"))
        {
            // 플레이어를 향해 이동
            Vector3 moveDirection = (player.position - transform.position).normalized;
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            // 적이 플레이어를 바라보도록 회전
            Vector3 lookDirection = (player.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            rb.MoveRotation(rotation);
        }
    }

    // Animator에서 isDie가 true가 될 때 호출되는 함수
    public void DieAnimationComplete()
    {
        // 스크립트를 비활성화하여 이동 및 회전 멈춤
        enabled = false;
    }
}
