using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 적 오브젝트 프리팹을 할당할 변수

    void Update()
    {
        // 마우스 우클릭을 감지하는 부분
        if (Input.GetMouseButtonDown(0)) // 1은 마우스 우클릭을 의미합니다.
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // 마우스 커서 위치를 기준으로 레이를 쏴서 충돌하는 지점을 찾음
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // 충돌 지점에 적을 소환
            Instantiate(enemyPrefab, hit.point, Quaternion.identity);
        }
    }
}
