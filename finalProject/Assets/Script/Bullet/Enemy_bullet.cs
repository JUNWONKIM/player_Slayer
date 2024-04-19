using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체가 플레이어인 경우
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어와 충돌하면 총알을 파괴함
            Destroy(gameObject);
            // 여기에 추가적인 플레이어에 대한 처리를 할 수 있음
            // 예를 들어, 플레이어의 체력을 감소시키는 등의 작업을 수행할 수 있음
        }
    }
}
