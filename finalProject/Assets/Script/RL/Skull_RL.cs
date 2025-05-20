using System.Collections;
using UnityEngine;

public class Skull_RL : MonoBehaviour
{
    public float moveSpeed = 12f;
    public float damageAmount = 1f;
    public float stopDistance = 5f;
    public float lifetime = 6f;

    public Transform ownerAgent;
    public CreatureSpawner2 spawnerOwner;

    private Rigidbody rb;
    private Animator animator;

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

        if (spawnerOwner != null)
            spawnerOwner.NotifyCreatureDestroyed();

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.transform == ownerAgent)
        {
            AgentHp hp = other.GetComponent<AgentHp>();
            if (hp != null)
                hp.TakeDamage(damageAmount);

            if (spawnerOwner != null)
                spawnerOwner.NotifyCreatureDestroyed();

            Destroy(gameObject);
        }
    }
}
