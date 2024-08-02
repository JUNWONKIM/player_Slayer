using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public float hp = 1000f;
    public float max_hp = 1000f;

    // Start is called before the first frame update
    void Start()
    {
        hp = max_hp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log("Player HP: " + hp);
    }
}
