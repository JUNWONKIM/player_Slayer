using System.Collections;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float speed = 5.0f; // 이동 속도
    public float rotationSpeed = 5.0f; // 회전 속도

    public GameObject atk0Prefab; // 기본 공격 프리팹
    public GameObject atk1Prefab; // Z 공격 프리팹
    public GameObject atk2Prefab; // X 공격 프리팹
    public GameObject atk3Prefab; // C 공격 프리팹

    public float attackRange = 3.0f; // 기본 공격 사거리

    private Transform player; 
    private bool isAttacking = false; // 공격 중인지 여부
    private bool isControlled = false; //C로 조종 여부
    private Animator animator;
    private Rigidbody rb;
    private float idleStartTime; 
    private float idleTimeToReattack = 2.0f; 
    private bool IsIdle = false; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange) // 용사가 사거리 내에 있을 시
        {
            if (!isAttacking)
            {
                StartCoroutine(BasicAttack()); // 공격
            }
        }
        else
        {
            IsIdle = false;
            if (!isAttacking) // 사거리 밖에 있을 시 & 공격 중 아닐 시
            {
                MoveTowardsPlayer(); //용사를 향해 이동
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && !isAttacking)
        {
            StartCoroutine(Z_Attack());
        }

        if (Input.GetKeyDown(KeyCode.X) && !isAttacking)
        {
            StartCoroutine(X_Attack());
        }

        if (Input.GetKeyDown(KeyCode.C) && !isAttacking)
        {
            StartCoroutine(C_Attack());
        }
    }

    private void MoveTowardsPlayer() //용사를 향해 이동
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
        animator.SetBool("IsWalk", true); // Walk 애니메이션 실행
    }



    private void HandleIdleState(float distanceToPlayer) //기본 공격과 idle상태 전환
    {
        if (Time.time - idleStartTime > idleTimeToReattack) //idle 지속시간 종료 시
        {
           
            if (distanceToPlayer <= attackRange) //사거리 안에 용사가 있으면
            {
                StartCoroutine(BasicAttack()); //기본 공격
            }
        }
    }

    IEnumerator BasicAttack() //기본 공격
    {
        isAttacking = true;

        while (true)
        {
            //용사를 향해 회전
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);

            if (Quaternion.Angle(rb.rotation, lookRotation) < 5.0f)
            {
                break;
            }

            yield return null;
        }

        //기본 공격 애니메이션 실행
        ResetAllTriggers();
        animator.SetBool("IsIdle", false);
        animator.SetBool("ATK0", true);

        // 속도 0, 이동 x
        float originalSpeed = speed;
        speed = 0; 

        // 현재 재생 중인 애니메이션의 길이를 계산
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;
        yield return new WaitForSeconds(animationLength);

        // 용사 위치에 폭발 프리팹 생성
        Vector3 attackPosition = player.position;
        Instantiate(atk0Prefab, attackPosition, Quaternion.identity);

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(animationLength / 2);
        animator.SetBool("ATK0", false);

        // 공격이 끝난 후 idle 상태로 전환
        IsIdle = true;
        idleStartTime = Time.time;

        ResetAllTriggers();
        animator.SetBool("IsIdle", true);

        //이동 속도 복구
        speed = originalSpeed;

        isAttacking = false;
    }

    IEnumerator Z_Attack() //공격
    {
        isAttacking = true;

        ResetAllTriggers();
        animator.SetTrigger("ATK1"); //Z 공격 애니메이션 실행

        GameObject atk1 = transform.Find("ATK1").gameObject;
        atk1.SetActive(true);

        yield return new WaitForSeconds(3.0f); //3초 대기

        atk1.SetActive(false);

        animator.ResetTrigger("ATK1");

        isAttacking = false;
    }

    IEnumerator X_Attack()
    {
        isAttacking = true;

        ResetAllTriggers();
        animator.SetTrigger("ATK2"); //X 공격 애니메이션 실행

        //이동 및 회전 정지
        float originalSpeed = speed;
        float originalRotationSpeed = rotationSpeed;
        speed = 0;
        rotationSpeed = 0;

        //가까운 Creature태그의 오브젝트 10개 찾기
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        if (creatures.Length == 0) yield break;

        Vector3 currentPosition = transform.position;
        var closestCreatures = creatures
            .OrderBy(creature => Vector3.Distance(creature.transform.position, currentPosition))
            .Take(10)
            .ToList();

        //찾은 오브젝트들을 공격 프리팹으로 변경
        foreach (GameObject closestCreature in closestCreatures)
        {
            Vector3 position = closestCreature.transform.position;
            Quaternion rotation = closestCreature.transform.rotation;
            Destroy(closestCreature);
            Instantiate(atk2Prefab, position, rotation);
        }

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // 이동 속도 복구
        speed = originalSpeed;
        rotationSpeed = originalRotationSpeed;

        animator.ResetTrigger("ATK2");

        isAttacking = false;
    }

    IEnumerator C_Attack()
    {
        isControlled = true;
        isAttacking = true;

        //기본 상태 저장
        float originalSpeed = speed;
        float originalRotationSpeed = rotationSpeed;

        //컨트롤 시 상태 변경
        float controlSpeed = speed * 2.0f;
        float controlRotationSpeed = 720.0f;

        float lastFireTime = Time.time;
        float controlDuration = 10.0f;
        float controlEndTime = Time.time + controlDuration;

        while (Time.time < controlEndTime) //지속 시간 동안
        {
            //방향키 사용
            float moveHorizontal = Input.GetAxis("Boss_Horizontal");
            float moveVertical = Input.GetAxis("Boss_Vertical");

            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

            if (movement != Vector3.zero)
            {
                rb.MovePosition(rb.position + movement * controlSpeed * Time.deltaTime);

                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                rb.rotation = Quaternion.RotateTowards(rb.rotation, toRotation, controlRotationSpeed * Time.deltaTime);

                ResetAllTriggers();
                animator.SetTrigger("IsRun"); //Run 애니메이션 실행

                if (Time.time - lastFireTime >= 0.1f) //지나가는 바닥에 불 생성
                {
                    Vector3 firePosition = transform.position - transform.forward * 20f;
                    Instantiate(atk3Prefab, firePosition, Quaternion.identity);
                    lastFireTime = Time.time;
                }
            }
            else
            {
                ResetAllTriggers();
                animator.SetTrigger("IsIdle"); //idle 상태로 변경
            }

            yield return null;
        }

        ResetAllTriggers();

        //기본 상태로 되돌림
        speed = originalSpeed;
        rotationSpeed = originalRotationSpeed;
        isControlled = false;
        isAttacking = false;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void ResetAllTriggers() //모든 애니메이션 초기화 (walk 상태로 변경)
    {
        animator.ResetTrigger("IsIdle");
        animator.ResetTrigger("IsRun");
        animator.ResetTrigger("ATK1");
        animator.ResetTrigger("ATK2");
        animator.ResetTrigger("ATK0");
    }
}
