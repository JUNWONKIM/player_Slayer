using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject playerPrefab; // 플레이어 프리팹
    public GameObject enemyPrefab1; // 1번 적 오브젝트 프리팹을 할당할 변수
    public GameObject enemyPrefab2; // 2번 적 오브젝트 프리팹을 할당할 변수
    public float spawnRange = 5f; // 플레이어와의 최소 소환 범위

    private int selectedEnemy = 1; // 현재 선택된 적의 번호

    void Update()
    {
        // 키보드 숫자 1을 누르면 1번 적 선택
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedEnemy = 1;
            Debug.Log("Selected enemy: " + selectedEnemy);
        }
        // 키보드 숫자 2를 누르면 2번 적 선택
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedEnemy = 2;
            Debug.Log("Selected enemy: " + selectedEnemy);
        }

        // 마우스 좌클릭을 감지하는 부분
        if (Input.GetMouseButtonDown(0)) // 0은 마우스 좌클릭을 의미합니다.
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // 플레이어 프리팹의 위치를 가져옵니다.
        Vector3 playerPosition = playerPrefab.transform.position;

        // 마우스 커서 위치를 기준으로 레이를 쏴서 충돌하는 지점을 찾습니다.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // 충돌 지점의 태그가 "ground"일 때만 적을 소환합니다.
            if (hit.collider.CompareTag("ground"))
            {
                Vector3 spawnPosition = hit.point; // 소환 위치 가져오기

                // 플레이어와의 거리를 계산하여 특정 범위 밖이면 적을 소환합니다.
                if (Vector3.Distance(new Vector3(playerPosition.x, 0f, playerPosition.z), new Vector3(spawnPosition.x, 0f, spawnPosition.z)) > spawnRange)
                {
                    // 현재 선택된 적에 따라 해당 적을 소환합니다.
                    if (selectedEnemy == 1)
                    {
                        Instantiate(enemyPrefab1, hit.point, Quaternion.identity);
                    }
                    else if (selectedEnemy == 2)
                    {
                        Instantiate(enemyPrefab2, hit.point, Quaternion.identity);
                    }
                }
            }
        }
    }
}
