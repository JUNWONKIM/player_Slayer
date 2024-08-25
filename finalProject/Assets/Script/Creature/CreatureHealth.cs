using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    public float maxHealth = 1; // 최대 체력
    public float currentHealth; // 현재 체력
    private Animator animator; // Creature의 애니메이터 컴포넌트
    private bool isDead = false; // 적이 죽었는지 여부
    private Rigidbody rb;

    // 죽을 때 재생할 소리 관련 변수
    public AudioClip deathSound; // 죽는 소리 클립
    private AudioSource audioSource; // 오디오 소스

    void Start()
    {
        currentHealth = maxHealth; // 시작할 때 최대 체력으로 설정
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
            currentHealth -= amount; // 데미지만큼 체력 감소

            if (currentHealth <= 0)
            {
                Die(); // 체력이 0 이하이면 사망 처리
            }
        }
    }

    void Die()
    {
        if (animator != null)
        {
            // Creature의 애니메이터에게 죽는 애니메이션을 재생하라고 알림
            animator.SetBool("isDie", true);
        }

        // 죽는 소리 재생
        if (audioSource != null && deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.volume = 1f; // 볼륨을 필요에 맞게 조절
            audioSource.Play(); // 죽는 소리 재생
        }

        gameObject.tag = "Untagged";
        // 몇 초 후에 태그를 변경하여 다른 스크립트에서 적임을 인식하지 못하게 함

        PlayerLV.IncrementCreatureDeathCount();
        // 적이 죽었음을 표시
        isDead = true;

        // 적의 위치를 고정시키고 회전을 멈춤
        rb.velocity = Vector3.zero;

        // 애니메이션의 길이에 따라 오브젝트를 파괴
        float dieAnimationLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Destroy(gameObject, dieAnimationLength);
    }
}
