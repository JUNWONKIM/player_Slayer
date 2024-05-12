using UnityEngine;

public class CreatureHealth : MonoBehaviour
{
    public int maxHealth = 1; // 최대 체력
    private int currentHealth; // 현재 체력
    private Animator animator; // Creature의 애니메이터 컴포넌트
    private bool isDead = false; // 적이 죽었는지 여부
    private Rigidbody rb;

    void Start()
    {
        currentHealth = maxHealth; // 시작할 때 최대 체력으로 설정
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
    }

    // 데미지를 입었을 때 호출되는 함수
    public void TakeDamage(int amount)
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
        // Creature의 애니메이터에게 죽는 애니메이션을 재생하라고 알림
        animator.SetBool("isDie", true);
        gameObject.tag = "Untagged";
        // 몇 초 후에 태그를 변경하여 다른 스크립트에서 적임을 인식하지 못하게 함


        // 적이 죽었음을 표시
        isDead = true;

        // 적의 위치를 고정시키고 회전을 멈춤
        rb.velocity = Vector3.zero;

        float dieAnimationLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Destroy(gameObject, dieAnimationLength);
    }

   
}
