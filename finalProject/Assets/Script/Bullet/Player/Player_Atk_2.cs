using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Atk_2 : MonoBehaviour
{
    public static Player_Atk_2 Instance;

    public float lifetime = 1.5f; // 이펙트가 사라지기까지의 시간
    public float damageAmount = 1f; // 폭발로 인한 데미지
    public SphereCollider explosionCollider; // 폭발 범위 콜라이더

    private bool hasExploded = false; // 폭발이 이미 발생했는지 여부

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        // 폭발 범위 콜라이더를 비활성화
        explosionCollider.enabled = false;

        // 콜라이더를 활성화하고 데미지를 준 후 이펙트를 파괴
        StartCoroutine(ActivateColliderAndDealDamage());
    }

    private IEnumerator ActivateColliderAndDealDamage()
    {
        // lifetime - 0.2f 시간 동안 대기
        yield return new WaitForSeconds(lifetime - 0.2f);

        // 폭발 범위 콜라이더를 활성화
        explosionCollider.enabled = true;
        hasExploded = true;

        // 폭발 범위 내의 적들에게 데미지를 줌
        DealDamage();

        // 잠시 대기 후 콜라이더를 비활성화
        yield return new WaitForSeconds(0.2f);
        explosionCollider.enabled = false;

        // 오브젝트 파괴
        Destroy(gameObject);
    }

    void DealDamage()
    {
        // 폭발 효과의 트리거 범위 내의 모든 콜라이더를 가져옴
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionCollider.radius);
        foreach (var hitCollider in hitColliders)
        {
            // 충돌한 객체의 태그가 "Creature"인 경우
            if (hitCollider.CompareTag("Creature"))
            {
                // 충돌한 객체의 HP를 감소시킴
                CreatureHealth enemyHealth = hitCollider.GetComponent<CreatureHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체의 태그가 "Creature"인 경우
        if (other.CompareTag("Creature") && hasExploded)
        {
            // 충돌한 객체의 HP를 감소시킴
            CreatureHealth enemyHealth = other.GetComponent<CreatureHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }

    public void IncreaseDamage(float amount)
    {
        damageAmount += amount;
        Debug.Log("폭탄 데미지 : " + damageAmount);
    }
}
