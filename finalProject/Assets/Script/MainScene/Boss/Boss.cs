using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    public float speed = 5.0f; // 보스가 이동할 속도
    public float rotationSpeed = 5.0f; // 보스가 회전할 속도

    public GameObject atk0Prefab; // 폭탄 프리팹
    public GameObject atk1Prefab; // ATK1 프리팹
    public GameObject atk2Prefab; // 교체할 프리팹
    public GameObject atk3Prefab; // 보스가 이동한 자리에 남길 불 프리팹

    public float attackRange = 3.0f; // 폭탄 공격을 할 거리

    private Transform player; // 플레이어의 위치를 저장할 변수
    private bool isAttacking = false; // 보스가 공격 중인지 여부
    private bool isControlled = false; // 보스가 플레이어에 의해 제어되는지 여부
    private Animator animator; // 애니메이터 컴포넌트
    private Rigidbody rb; // Rigidbody 컴포넌트

    private float idleStartTime; // Idle 상태 시작 시간
    private float idleTimeToReattack = 2.0f; // Idle 상태가 지속된 후 재공격 시간
    private bool IsIdle = false; // 현재 Idle 상태인지 여부

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어가 사정거리 내에 있을 때
        if (distanceToPlayer <= attackRange)
        {
            if (!isAttacking)
            {
                // 사정거리 내에서 바로 공격
                StartCoroutine(BasicAttack());
            }
        }
        else
        {
            IsIdle = false;
            // 플레이어가 사정거리 밖으로 나간 경우 보스가 다시 플레이어를 추적하도록 설정
            if (!isAttacking)
            {
                MoveTowardsPlayer();
            }
        }

        // 플레이어의 키보드 입력에 대한 공격 처리
        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            StartCoroutine(ReplaceClosestCreatures());
        }

        if (Input.GetKeyDown(KeyCode.C) && !isAttacking)
        {
            StartCoroutine(ControlBoss());
        }
    }

    private void MoveTowardsPlayer()
    {
        if (IsIdle) return; // idle 상태에서는 이동하지 않음

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // 회전
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // 이동
        Vector3 moveDirection = direction * speed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);

        ResetAllTriggers();
        animator.SetBool("IsWalk", true); // Walk 애니메이션 트리거
    }



    private void HandleIdleState(float distanceToPlayer)
    {
        if (Time.time - idleStartTime > idleTimeToReattack)
        {
            // Idle 상태에서 플레이어가 여전히 사정거리 안에 있으면 기본 공격을 다시 수행
            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(BasicAttack());
            }
        }
    }

    IEnumerator BasicAttack()
    {
        isAttacking = true;

        while (true)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(rb.rotation, lookRotation) < 5.0f)
            {
                break;
            }

            yield return null;
        }

        ResetAllTriggers();
        animator.SetBool("IsIdle", false);
        animator.SetBool("ATK0", true);

        // 보스가 공격 중일 때 이동을 멈추게 하고 원래 속도 저장
        float originalSpeed = speed;
        speed = 0; // 공격 중 이동을 막음

        // 현재 재생 중인 애니메이션의 길이를 계산
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // 공격 위치로 폭탄 생성
        Vector3 attackPosition = player.position;
        Instantiate(atk0Prefab, attackPosition, Quaternion.identity);

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animationLength / 2);
        animator.SetBool("ATK0", false);

        // 공격이 끝난 후 idle 상태로 전환
        IsIdle = true;
        idleStartTime = Time.time;

        ResetAllTriggers();
        animator.SetBool("IsIdle", true); // Idle 상태로 명확히 전환

        // Speed를 원래 값으로 복원
        speed = originalSpeed; // 이동 속도를 원래 값으로 복구

        isAttacking = false;
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        ResetAllTriggers();
        animator.SetTrigger("ATK1");

        GameObject atk1 = transform.Find("ATK1").gameObject;
        atk1.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        atk1.SetActive(false);

        animator.ResetTrigger("ATK1");

        isAttacking = false;
    }

    IEnumerator ReplaceClosestCreatures()
    {
        isAttacking = true;

        ResetAllTriggers();
        animator.SetTrigger("ATK2");

        float originalSpeed = speed;
        float originalRotationSpeed = rotationSpeed;
        speed = 0;
        rotationSpeed = 0;

        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        if (creatures.Length == 0) yield break;

        Vector3 currentPosition = transform.position;
        var closestCreatures = creatures
            .OrderBy(creature => Vector3.Distance(creature.transform.position, currentPosition))
            .Take(10)
            .ToList();

        foreach (GameObject closestCreature in closestCreatures)
        {
            Vector3 position = closestCreature.transform.position;
            Quaternion rotation = closestCreature.transform.rotation;
            Destroy(closestCreature);
            Instantiate(atk2Prefab, position, rotation);
        }

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // 원래의 speed와 rotationSpeed 복원
        speed = originalSpeed;
        rotationSpeed = originalRotationSpeed;
        animator.ResetTrigger("ATK2");

        isAttacking = false;
    }

    IEnumerator ControlBoss()
    {
        isControlled = true;
        isAttacking = true;

        float originalSpeed = speed;
        float originalRotationSpeed = rotationSpeed;
        float controlSpeed = speed * 2.0f;
        float controlRotationSpeed = 720.0f;

        float lastFireTime = Time.time;
        float controlDuration = 10.0f;
        float controlEndTime = Time.time + controlDuration;

        while (Time.time < controlEndTime)
        {
            float moveHorizontal = Input.GetAxis("Boss_Horizontal");
            float moveVertical = Input.GetAxis("Boss_Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            if (movement != Vector3.zero)
            {
                rb.MovePosition(rb.position + movement * controlSpeed * Time.deltaTime);

                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, controlRotationSpeed * Time.deltaTime);

                ResetAllTriggers();
                animator.SetTrigger("IsRun");

                if (Time.time - lastFireTime >= 0.1f)
                {
                    Vector3 firePosition = transform.position - transform.forward * 20f;
                    Instantiate(atk3Prefab, firePosition, Quaternion.identity);
                    lastFireTime = Time.time;
                }
            }
            else
            {
                ResetAllTriggers();
                animator.SetTrigger("IsIdle");
            }

            yield return null;
        }

        ResetAllTriggers();

        // 원래의 speed와 rotationSpeed 복원
        speed = originalSpeed;
        rotationSpeed = originalRotationSpeed;
        isControlled = false;
        isAttacking = false;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void ResetAllTriggers()
    {
        animator.ResetTrigger("IsIdle");
        animator.ResetTrigger("IsRun");
        animator.ResetTrigger("ATK1");
        animator.ResetTrigger("ATK2");
        animator.ResetTrigger("ATK0");
    }
}
