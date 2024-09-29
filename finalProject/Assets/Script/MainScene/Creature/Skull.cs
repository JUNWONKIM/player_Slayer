using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float damageAmount = 1f; //데미지


    private Transform player;
    private Rigidbody rb;
    private Animator animator; 
    private bool canDealDamage = true; // 데미지를 줄 수 있는 상태 여부

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody>(); 
        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
       
        if (!animator.GetBool("isDie")) // 살아 있을 경우
        {
            // 용사를 향해 이동
            Vector3 moveDirection = (player.position - transform.position).normalized;
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            // 용사를 향해 회전
            Vector3 lookDirection = (player.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            rb.MoveRotation(rotation);
        }
    }

  
    private IEnumerator DamageCooldown() //피해를 입히면 0.5간 피해를 주지 못하게 막음
    {
        canDealDamage = false;
        yield return new WaitForSeconds(0.5f);
        canDealDamage = true;
    }


    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Player") && canDealDamage)
        {
            PlayerHP playerHP = collision.gameObject.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.hp -= damageAmount;  //용사에게 피해를 줌
                StartCoroutine(DamageCooldown()); //피해 쿨타임 
            }
        }
    }

}
