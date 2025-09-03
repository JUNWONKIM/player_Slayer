using System.Collections;
using UnityEngine;

public class Skull_RL : MonoBehaviour
{
    public float moveSpeed = 12f;
    public float damageAmount = 1f;
    public float stopDistance = 5f;
    public float lifetime = 6f;
    public float damageInterval = 0.5f;

    public Transform ownerAgent;
    public CreatureSpawner2 spawnerOwner;

    private Rigidbody rb;
    private Animator animator;

    private bool isTouchingPlayer = false;
    private Coroutine damageCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        StartCoroutine(SelfDestructAfterTime());
    }

    void Update()
    {
        if (ownerAgent == null || animator.GetBool("isDie")) return;

        float distance = Vector3.Distance(transform.position, ownerAgent.position);

        if (distance > stopDistance)
        {
            Vector3 dir = (ownerAgent.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }

        Vector3 lookDir = ownerAgent.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
    }

    IEnumerator SelfDestructAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject); // 해골 수명 종료
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform == ownerAgent)
        {
            if (!isTouchingPlayer)
            {
                isTouchingPlayer = true;
                damageCoroutine = StartCoroutine(DealDamageOverTime(other.GetComponent<AgentHp>()));
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.transform == ownerAgent)
        {
            if (isTouchingPlayer)
            {
                isTouchingPlayer = false;
                if (damageCoroutine != null)
                    StopCoroutine(damageCoroutine);
            }
        }
    }

    IEnumerator DealDamageOverTime(AgentHp hp)
    {
        while (isTouchingPlayer && hp != null)
        {
            hp.TakeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
