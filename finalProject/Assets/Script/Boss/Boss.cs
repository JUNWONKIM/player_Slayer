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
    public Slider uiSlider; // UISliderController에서 가져온 Slider 컴포넌트

    public float attackRange = 3.0f; // 폭탄 공격을 할 거리

    private Transform player; // 플레이어의 위치를 저장할 변수
    private bool isAttacking = false; // 보스가 공격 중인지 여부
    private bool isControlled = false; // 보스가 플레이어에 의해 제어되는지 여부
    private Animator animator; // 애니메이터 컴포넌트

    private float idleStartTime; // Idle 상태 시작 시간
    private float idleTimeToReattack = 2.0f; // Idle 상태가 지속된 후 재공격 시간
    private bool isIdle = false; // 현재 Idle 상태인지 여부

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isAttacking && !isControlled)
        {
            if (distanceToPlayer <= attackRange)
            {
                if (isIdle)
                {
                    // Idle 상태가 2초 이상 지속되었을 때 기본 공격을 다시 실행
                    if (Time.time - idleStartTime > idleTimeToReattack)
                    {
                        StartCoroutine(BasicAttack());
                    }
                }
                else
                {
                    // 기본 공격을 실행
                    StartCoroutine(BasicAttack());
                }
            }
            else
            {
                // 플레이어가 공격 사거리 밖으로 나갔으므로 다시 플레이어를 향해 걸어가기
                isIdle = false;
                MoveTowardsPlayer();
            }
        }

        // Z 키를 눌렀을 때 공격 시작
        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        // X 키를 눌렀을 때 가장 가까운 Creature 프리팹을 교체
        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            StartCoroutine(ReplaceClosestCreatures());
        }

        // C 키를 눌렀을 때 10초 동안 플레이어에 의해 제어
        if (Input.GetKeyDown(KeyCode.C) && !isAttacking)
        {
            StartCoroutine(ControlBoss());
            if (uiSlider != null)
            {
                uiSlider.gameObject.SetActive(true); // 슬라이더를 활성화
                StartCoroutine(StartSliderCountdown()); // 슬라이더의 값을 줄이기 시작
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        // 플레이어를 향해 다가가기
        Vector3 direction = (player.position - transform.position).normalized;

        // 플레이어를 바라보기
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // 플레이어를 향해 이동하기
        transform.position += transform.forward * speed * Time.deltaTime;

        // 이동 중에는 Idle 상태 해제
        if (isIdle)
        {
            // Idle 상태 해제
            ResetAllTriggers();
            animator.SetBool("IsIdle", false);
            isIdle = false;
        }
    }

    IEnumerator BasicAttack()
    {
        // 공격 중임을 표시
        isAttacking = true;

        // 기본 공격 애니메이션 실행
        ResetAllTriggers();
        animator.SetBool("IsIdle", false);
        animator.SetBool("ATK0", true);

        // 보스가 멈추도록 속도 설정
        float originalSpeed = speed;
        speed = 0;

        // 애니메이션이 끝날 때까지 대기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength); // 애니메이션 전체 길이만큼 대기

        // 현재 플레이어 위치에 폭탄을 생성
        Vector3 attackPosition = player.position;
        Instantiate(atk0Prefab, attackPosition, Quaternion.identity);

        // 애니메이션 종료 후 나머지 대기 시간
        yield return new WaitForSeconds(animationLength / 2); // 애니메이션 절반 시점까지 대기
        animator.SetBool("ATK0", false);

        // Idle 상태로 전환하고 2초 대기
        isIdle = true;
        idleStartTime = Time.time;

        // 애니메이션 상태를 Idle로 설정
        ResetAllTriggers();
        animator.SetBool("IsIdle", true);

        // 기본 공격 완료
        isAttacking = false;
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        // 모든 트리거 리셋 후 Attack 애니메이션 실행
        ResetAllTriggers();
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

    IEnumerator ReplaceClosestCreatures()
    {
        isAttacking = true;

        // 모든 트리거 리셋 후 ReplaceClosestCreatures 애니메이션 실행
        ResetAllTriggers();
        animator.SetTrigger("ATK2");

        // 보스가 멈추도록 속도와 회전 속도 설정
        float originalSpeed = speed;
        float originalRotationSpeed = rotationSpeed;
        speed = 0;
        rotationSpeed = 0;

        // 가장 가까운 10개의 Creature 오브젝트를 찾음
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        if (creatures.Length == 0) yield break;

        Vector3 currentPosition = transform.position;

        // 가장 가까운 10개의 Creature 오브젝트를 거리순으로 정렬하여 선택
        var closestCreatures = creatures
            .OrderBy(creature => Vector3.Distance(creature.transform.position, currentPosition))
            .Take(10)
            .ToList();

        // 가까운 Creature 오브젝트들을 새로운 프리팹으로 교체
        foreach (GameObject closestCreature in closestCreatures)
        {
            Vector3 position = closestCreature.transform.position;
            Quaternion rotation = closestCreature.transform.rotation;
            Destroy(closestCreature);
            Instantiate(atk2Prefab, position, rotation);
        }

        // 애니메이션이 끝날 때까지 기다림
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // 원래 상태로 돌아가기
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
        float controlSpeed = 20.0f; // 플레이어가 제어할 때 보스의 속도
        float controlRotationSpeed = 720.0f; // 플레이어가 제어할 때 보스의 회전 속도

        float lastFireTime = Time.time;
        float controlDuration = 10.0f; // 제어 시간 10초
        float controlEndTime = Time.time + controlDuration;

        while (Time.time < controlEndTime)
        {
            // 방향키로 보스 제어
            float moveHorizontal = Input.GetAxis("Boss_Horizontal");
            float moveVertical = Input.GetAxis("Boss_Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            if (movement != Vector3.zero)
            {
                // 보스 위치 업데이트
                transform.position += movement * controlSpeed * Time.deltaTime;

                // 이동 방향으로 보스 회전
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, controlRotationSpeed * Time.deltaTime);

                // 애니메이션 트리거 설정
                ResetAllTriggers();
                animator.SetTrigger("isRun");

                // 0.1초마다 불 프리팹 생성
                if (Time.time - lastFireTime >= 0.1f)
                {
                    Vector3 firePosition = transform.position - transform.forward * 20f; // 보스 뒤쪽에 불 생성
                    Instantiate(atk3Prefab, firePosition, Quaternion.identity);
                    lastFireTime = Time.time;
                }
            }
            else
            {
                // 이동하지 않으면 Idle 상태로 변경
                ResetAllTriggers();
                animator.SetTrigger("isIdle");
            }

            yield return null;
        }

        // 애니메이션 트리거를 모두 false로 설정
        ResetAllTriggers();

        // 원래 상태로 돌아가기
        speed = originalSpeed;
        rotationSpeed = originalRotationSpeed;
        isControlled = false;
        isAttacking = false;
    }

    private IEnumerator StartSliderCountdown()
    {
        // 슬라이더를 활성화하고 값 줄이기 시작
        uiSlider.gameObject.SetActive(true);
        uiSlider.value = 1.0f; // 슬라이더를 꽉 찬 상태로 설정

        float startTime = Time.time;

        while (Time.time < startTime + 10.0f) // 10초 동안 슬라이더 값을 줄임
        {
            uiSlider.value = Mathf.Lerp(1.0f, 0.0f, (Time.time - startTime) / 10.0f);
            yield return null;
        }

        // 슬라이더가 완전히 빈 상태로 설정
        uiSlider.value = 0.0f;

        // 슬라이더의 게임 오브젝트를 비활성화
        uiSlider.gameObject.SetActive(false);
    }

    private void ResetAllTriggers()
    {
        // 모든 트리거를 리셋하여 이전 애니메이션 상태가 남지 않도록 함
        animator.ResetTrigger("isIdle");
        animator.ResetTrigger("isRun");
        animator.ResetTrigger("ATK1");
        animator.ResetTrigger("ATK2");
        animator.ResetTrigger("ATK0"); // ATK0 트리거 리셋 추가
    }
}
