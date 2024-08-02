using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player; // 플레이어의 위치를 저장할 변수
    public float speed = 5.0f; // 보스가 이동할 속도
    public float rotationSpeed = 5.0f; // 보스가 회전할 속도
    public GameObject atk1Prefab; // ATK1 프리팹
    private bool isAttacking = false; // 보스가 공격 중인지 여부
    private Animator animator; // 애니메이터 컴포넌트

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 공격 중일 때는 이동하지 않음
        if (isAttacking)
            return;

        // 플레이어를 향해 다가가기
        Vector3 direction = (player.position - transform.position).normalized;

        // 플레이어를 바라보기
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // 플레이어를 향해 이동하기
        transform.position += transform.forward * speed * Time.deltaTime;

        // Q 키를 눌렀을 때 공격 시작
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        // 애니메이션 트리거 설정
        animator.SetTrigger("ATK1");

        // ATK1 프리팹 활성화
        GameObject atk1 = transform.Find("ATK1").gameObject;
        atk1.SetActive(true);

        // 3초 동안 대기
        yield return new WaitForSeconds(3.0f);

        // ATK1 프리팹 비활성화
        atk1.SetActive(false);

        // 원래 애니메이션으로 돌아가기
        animator.ResetTrigger("ATK1");

        isAttacking = false;
    }
}
