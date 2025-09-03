using UnityEngine;

public class ClickSpawner : MonoBehaviour
{
    public Camera mainCamera; // ← 할당 안돼도 자동으로 잡게 수정
    public GameObject skullPrefab;
    public GameObject ghostPrefab;
    public float spawnHeight = 0.5f;

    void Start()
    {
        // ✅ 자동으로 Main Camera 찾기 (할당 안되었을 때만)
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 클릭
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                // ✅ "Ground" 태그를 가진 오브젝트만 허용
                if (hit.collider.CompareTag("ground"))
                {
                    Vector3 spawnPos = hit.point + Vector3.up * spawnHeight;
                    GameObject prefabToSpawn = Random.value < 0.5f ? skullPrefab : ghostPrefab;
                    Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                }
            }
        }
    }
}
