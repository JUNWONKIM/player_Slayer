using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossHP : MonoBehaviour
{
    public float maxHP = 1; // �ִ� ü��
    public float currentHP; // ���� ü��
    public AudioClip deathSound; // �״� �Ҹ�


    private Animator animator;
    private bool isDead = false; 
    private Rigidbody rb;
    private AudioSource audioSource; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        currentHP = maxHP; // ���� �� �ִ� ü������ ����
  
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; // 2D �Ҹ��� ����
        }
    }

    public void TakeDamage(float amount) //���ظ� ���� �� ����
    {
        if (!isDead)
        {
            currentHP -= amount; // ��������ŭ ü�� ����

            if (currentHP <= 0)
            {
                Die(); // ü���� 0 �����̸� ���� ó��
            }
        }
    }

    void Die() //���� ó��
    {
        if (animator != null)
        {
            animator.SetBool("isDie", true); //die �ִϸ��̼� ����
        }

        // �״� �Ҹ� ���
        if (audioSource != null && deathSound != null)
        {
            audioSource.clip = deathSound;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        //�����Ӱ� ȸ�� ����
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll; 

        // NavMeshAgent ��Ȱ��ȭ
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        gameObject.tag = "Untagged";

        PlayerLV.IncrementCreatureDeathCount(); // ũ���� ���� ī��Ʈ ����
        isDead = true;
           
        SceneManager.LoadScene("LoseScene"); 

        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        float dieAnimationLength = clipInfo[0].clip.length;

        Destroy(gameObject, dieAnimationLength);
    }
}
