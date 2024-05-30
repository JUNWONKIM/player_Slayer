using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Atk_2 : MonoBehaviour
{
    public GameObject objectToSpawn; // 소환할 오브젝트 프리팹
    public float lifetime = 1f; // 오브젝트가 사라지기까지의 시간
    public float fixedYPosition = 2.5f; // 고정된 Y 축 위치

    private void Start()
    {
        // 오브젝트를 lifetime 시간 후에 사라지게 하고 새로운 오브젝트를 소환
        StartCoroutine(DestroyAndSpawn());
    }

    private IEnumerator DestroyAndSpawn()
    {
        // lifetime 시간 동안 대기
        yield return new WaitForSeconds(lifetime);

        // 현재 위치에서 Y축을 고정한 상태로 새로운 오브젝트 소환
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = fixedYPosition;

        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

        // 현재 오브젝트를 파괴
        Destroy(gameObject);
    }
}
