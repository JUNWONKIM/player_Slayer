using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class CreatureHp : MonoBehaviour
{
    public float maxHp = 1; // �ִ� ü��
    public float currentHp; // ���� ü��
    public AudioClip deathSound; // �״� �Ҹ�

    private Animator animator;
    private bool isDead = false; // �� ���� ����
    private Rigidbody rb;
    private AudioSource audioSource; 



    void Start()
    {

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        currentHp = maxHp; // ���� �� �ִ� ü������ ����
         
        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f; 
        }
    }

 
    public void TakeDamage(float amount) //���ظ� ���� ��
    {
        if (!isDead)
        {
            currentHp -= amount; // ü�� ����

            if (currentHp <= 0)
            {
                Destroy(gameObject);
                //Die(); 
            }
        }
    }

    //void Die() //��� ó��
    //{
    //    if (animator != null)
    //    {
    //        animator.SetBool("isDie", true); //�״� �ִϸ��̼� ����
    //    }

    //    // �״� �Ҹ� ���
    //    if (audioSource != null && deathSound != null)
    //    {
    //        audioSource.clip = deathSound;
    //        audioSource.volume = 1f;
    //        audioSource.Play();
    //    }

    //    //������ ����
    //    rb.isKinematic = true;
    //    rb.velocity = Vector3.zero;
    //    rb.constraints = RigidbodyConstraints.FreezeAll; 

    //    // NavMeshAgent ��Ȱ��ȭ 
    //    NavMeshAgent agent = GetComponent<NavMeshAgent>();
    //    if (agent != null)
    //    {
    //        agent.enabled = false;
    //    }

    //    //�±� ����
    //    gameObject.tag = "Untagged";

    //    //ũ��ó ���� ī��Ʈ �߰�
    //    PlayerLV.IncrementCreatureDeathCount();
    //    isDead = true;

        
    //    AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
    //    float dieAnimationLength = clipInfo[0].clip.length;

    //    //������Ʈ ����
    //    Destroy(gameObject, dieAnimationLength);
    //}

}