using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Mummy_ex : MonoBehaviour
{
    public float damageAmount; //������

 
 
    void OnTriggerEnter(Collider other)
    {
        PlayerHP playerHP = other.gameObject.GetComponent<PlayerHP>();
        if (playerHP != null)
        {
            playerHP.hp -= damageAmount; //��� ü�� ����
        }
        
    }   
}
