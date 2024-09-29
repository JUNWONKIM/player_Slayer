using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossHP : MonoBehaviour
{
    public float maxHP = 1; // 최대 체력
    public float currentHP; // 현재 체력
    public AudioClip deathSound; // 죽는 소리


    private Animator animator;
    private bool isDead = false; 
    private Rigidbody rb;
    private AudioSource audioSource; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        currentHP = maxHP; // 시작 시 최대 체력으로 설정
  
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; // 2D 소리로 설정
        }
    }

    public void TakeDamage(float amount) //피해를 입을 시 적용
    {
        if (!isDead)
        {
            currentHP -= amount; // 데미지만큼 체력 감소

            if (currentHP <= 0)
            {
                Die(); // 체력이 0 이하이면 죽음 처리
            }
        }
    }

    void Die() //죽음 처리
    {
        if (animator != null)
        {
            animator.SetBool("isDie", true); //die 애니메이션 실행
        }

        // 죽는 소리 재생
        if (audioSource != null && deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        //움직임과 회전 제한
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll; 

        // NavMeshAgent 비활성화
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        gameObject.tag = "Untagged";

        PlayerLV.IncrementCreatureDeathCount(); // 크리쳐 데스 카운트 증가
        isDead = true;
           
        SceneManager.LoadScene("LoseScene"); 

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float dieAnimationLength = clipInfo[0].clip.length;

        Destroy(gameObject, dieAnimationLength);
    }
}
