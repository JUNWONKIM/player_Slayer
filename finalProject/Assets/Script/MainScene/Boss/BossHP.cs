using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossHP : MonoBehaviour
{
    public float maxHP = 1; // 최대 체력
    public float currentHP; // 현재 체력
    private Animator animator; // Creature의 애니메이터 컴포넌트
    private bool isDead = false; // 적이 죽었는지 여부
    private Rigidbody rb;

    // 죽을 때 재생할 소리 관련 변수
    public AudioClip deathSound; // 죽는 소리 클립
    private AudioSource audioSource; // 오디오 소스

    void Start()
    {
        currentHP = maxHP; // 시작할 때 최대 체력으로 설정
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기

        // AudioSource 설정
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; // 2D 소리로 설정
        }
    }

    // 데미지를 입었을 때 호출되는 함수
    public void TakeDamage(float amount)
    {
        if (!isDead)
        {
            currentHP -= amount; // 데미지만큼 체력 감소

            if (currentHP <= 0)
            {
                Die(); // 체력이 0 이하이면 사망 처리
            }
        }
    }

    void Die()
    {
        if (animator != null)
        {
            animator.SetBool("isDie", true);
        }

        // 죽는 소리 재생
        if (audioSource != null && deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        // Rigidbody 물리 상호작용 비활성화 및 움직임 고정
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll; // 모든 축의 움직임 및 회전 고정

        // NavMeshAgent 비활성화 (보스가 NavMesh를 사용하고 있다면)
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        gameObject.tag = "Untagged";

        PlayerLV.IncrementCreatureDeathCount();
        isDead = true;

        // 보스가 죽으면 씬을 패배 씬으로 전환
        SceneManager.LoadScene("LoseScene"); // 패배 씬 이름으로 변경 필요

        // 애니메이션의 정확한 길이를 가져오기
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float dieAnimationLength = clipInfo[0].clip.length;

        Destroy(gameObject, dieAnimationLength);
    }
}
