using System.Collections;
using UnityEngine;

public class Skull_RL : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float damageAmount = 1f;
    public float stopDistance = 5f;
    public Transform ownerAgent;

    private Rigidbody rb;
    private Animator animator;
    private bool canDealDamage = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!animator.GetBool("isDie") && ownerAgent != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, ownerAgent.position);

            if (distanceToPlayer > stopDistance)
            {
                Vector3 moveDirection = (ownerAgent.position - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }

            Vector3 lookDirection = ownerAgent.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(0.5f);
        canDealDamage = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDealDamage)
        {
            if (ownerAgent != null && other.transform == ownerAgent)
            {
                AgentHp agentHP = other.GetComponent<AgentHp>();
                if (agentHP != null)
                {
                    agentHP.TakeDamage(1f); // ✅ 즉사급 데미지
                    //StartCoroutine(DamageCooldown());
                }

                Destroy(gameObject); // ✅ 충돌 시 즉시 사라짐
            }
        }
    }
}
