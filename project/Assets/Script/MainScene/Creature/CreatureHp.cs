using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class CreatureHp : MonoBehaviour
{
    public float maxHp = 1; // 최대 체력
    public float currentHp; // 현재 체력
    public AudioClip deathSound; // 죽는 소리

    private Animator animator;
    private bool isDead = false; // 적 생존 여부
    private Rigidbody rb;
    private AudioSource audioSource; 



    void Start()
    {

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        currentHp = maxHp; // 시작 시 최대 체력으로 설정
         
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; 
        }
    }

 
    public void TakeDamage(float amount) //피해를 입을 시
    {
        if (!isDead)
        {
            currentHp -= amount; // 체력 감소

            if (currentHp <= 0)
            {
                Destroy(gameObject);
                //Die(); 
            }
        }
    }

    //void Die() //사망 처리
    //{
    //    if (animator != null)
    //    {
    //        animator.SetBool("isDie", true); //죽는 애니메이션 실행
    //    }

    //    // 죽는 소리 재생
    //    if (audioSource != null && deathSound != null)
    //    {
    //        audioSource.clip = deathSound;
    //        audioSource.volume = 1f;
    //        audioSource.Play();
    //    }

    //    //움직임 고정
    //    rb.isKinematic = true;
    //    rb.velocity = Vector3.zero;
    //    rb.constraints = RigidbodyConstraints.FreezeAll; 

    //    // NavMeshAgent 비활성화 
    //    NavMeshAgent agent = GetComponent<NavMeshAgent>();
    //    if (agent != null)
    //    {
    //        agent.enabled = false;
    //    }

    //    //태그 제거
    //    gameObject.tag = "Untagged";

    //    //크리처 데스 카운트 추가
    //    PlayerLV.IncrementCreatureDeathCount();
    //    isDead = true;

        
    //    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
    //    float dieAnimationLength = clipInfo[0].clip.length;

    //    //오브젝트 제거
    //    Destroy(gameObject, dieAnimationLength);
    //}

}