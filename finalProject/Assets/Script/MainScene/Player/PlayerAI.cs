using System.Collections;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    public static PlayerAI instance; // 싱글톤

    public float moveSpeed = 100f; // 이동 속도
    public float slowSpeed = 2f; // 슬로우 배수
    public bool isFreezed = false; // 슬로우 상태 여부

    public float avoidanceDistance = 3f; // 투사체를 피하는 거리
    public float bulletDetectionRange = 20f; // 투사체 감지 거리

    private Transform target; // 가장 가까운 적의 위치
    private Transform nearestBullet; // 가장 가까운 투사체의 위치
    private Rigidbody rb;
    private Animator animator; // 애니메이터 추가

    private enum PlayerState // 용사 상태 구분
    {
        MoveAwayFromCreature, // 적에게서 멀어짐
        AvoidBullet, // 투사체를 피함
        Idle // 대기 상태
    }
    private PlayerState currentState = PlayerState.MoveAwayFromCreature;

    private float stateChangeTime = 0f;
    private float stateChangeDuration = 0.5f; // 상태 변경 유지 시간

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        StartCoroutine(FindClosestObjectsCoroutine());
    }

    void Update()
    {
        // 상태에 따라 다른 행동을 수행
        switch (currentState)
        {
            case PlayerState.MoveAwayFromCreature:
                MoveAwayFromCreature();
                break;
            case PlayerState.AvoidBullet:
                if (nearestBullet != null)
                {
                    AvoidBullet(nearestBullet.position);
                }
                break;
            case PlayerState.Idle:
                IdleState();
                break;
        }

       
        if (nearestBullet == null && target == null) // 크리쳐나 보스가 없고, 투사체도 없을 경우
        {
            ChangeState(PlayerState.Idle); // Idle 상태로 전환
        }
        else if (nearestBullet != null && Vector3.Distance(transform.position, nearestBullet.position) < bulletDetectionRange)
        {
            ChangeState(PlayerState.AvoidBullet); // 투사체가 있으면 AvoidBullet 상태로 전환
        }
        else if (target != null) // 크리쳐나 보스가 있으면 MoveAwayFromCreature 상태로 전환
        {
            ChangeState(PlayerState.MoveAwayFromCreature);
        }

        LookAtTarget(); // 가까운 크리쳐를 바라봄
        CheckForSlowObjects(); // 슬로우 이펙트가 있는지 확인
    }



    void IdleState() //idle 상태
    {
        animator.SetBool("isIdle", true); // Idle 애니메이션 설정
    }

    void LookAtTarget() // 크리쳐를 바라봄
    {
        if (target != null)
        {
            Vector3 lookAtDirection = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookAtDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 10f));
        }
    }

    void MoveAwayFromCreature() //크리처에게서 멀어짐
    {
      
        animator.SetBool("isIdle", false); //walk 애니메이션 실행


        //크치러에게서 반대로 이동 & 가까운 크리처를 바라봄
        if (target != null)
        {
            Vector3 moveDirection = (transform.position - target.position).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            Vector3 lookAtDirection = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookAtDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, Time.fixedDeltaTime * 10f)); 
        }
    }

    void AvoidBullet(Vector3 bulletPosition) //투사체 회피 
    {
        
        animator.SetBool("isIdle", false); //walk 애니메이션 실행

        //
        if (nearestBullet != null)
        {
            Vector3 directionToPlayer = transform.position - bulletPosition;
            Vector3 bulletDirection = nearestBullet.GetComponent<Rigidbody>().velocity.normalized;
            Vector3 perpendicular = Vector3.Cross(bulletDirection, Vector3.up).normalized; //투사체와 수직으로 방향 설정

            
            rb.MovePosition(rb.position + perpendicular * moveSpeed * Time.fixedDeltaTime); 
        }
    }

    private IEnumerator FindClosestObjectsCoroutine() //가장 가까운 보스, 크리쳐, 투사체를 찾음
    {
        while (true)
        {
            FindClosestCreatureOrBoss();  // 가까운 크리쳐나 보스 찾기
            FindClosestBullet();          // 가까운 투사체 찾기
            yield return new WaitForSeconds(0.2f); 
        }
    }

    void FindClosestCreatureOrBoss() //가까운 크리쳐나 보스 찾기
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature"); 
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;
        
        //각 크리처에 대하여 가장 가까운 거리를 가진 크리처를 찾음
        foreach (GameObject creature in creatures)
        {
            float distance = Vector3.Distance(transform.position, creature.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = creature;
            }
        }

        //보스를 찾음
        foreach (GameObject boss in bosses)
        {
            float distance = Vector3.Distance(transform.position, boss.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = boss;
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget.transform;
        }
        else
        {
            target = null; 
        }
    }

    void FindClosestBullet() //가장 가까운 투사체 찾기
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("C_Bullet");
        float closestDistance = Mathf.Infinity;
        GameObject closestBullet = null;

        foreach (GameObject bullet in bullets)
        {
            float distance = Vector3.Distance(transform.position, bullet.transform.position);

            //각각의 투사체에 대해 가장 가까운 투사체를 찾음
            if (distance < closestDistance && distance <= bulletDetectionRange)
            {
                closestDistance = distance;
                closestBullet = bullet;
            }
        }

        if (closestBullet != null)
        {
            nearestBullet = closestBullet.transform;
        }
        else
        {
            nearestBullet = null; 
        }
    }

    private void ChangeState(PlayerState newState) //상태 변경
    {
        currentState = newState;
        stateChangeTime = Time.time;

      
        if (newState == PlayerState.Idle)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }

    private void CheckForSlowObjects() //마녀의 슬로우 이펙트 존재 확인
    {
        GameObject[] freezeObjects = GameObject.FindGameObjectsWithTag("Freeze");

        //존재할 경우 속도 감소
        if (freezeObjects.Length > 0 && !isFreezed)
        {
            moveSpeed /= slowSpeed;
            isFreezed = true;
        }

        //없을 시 속도 복구
        else if (freezeObjects.Length == 0 && isFreezed)
        {
            moveSpeed *= slowSpeed;
            isFreezed = false;
        }
    }

    public void IncreaseMoveSpeed(float amount) //속도 증가 (레벨업 시 필요)
    {
        moveSpeed *= amount;
    }
}
