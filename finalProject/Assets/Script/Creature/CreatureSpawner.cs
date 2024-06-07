using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // 플레이어 프리팹
    public GameObject[] creaturePrefabs; // 적 오브젝트 프리팹 배열
    public float spawnRange = 5f; // 플레이어와의 최소 소환 범위

    private const string GroundTag = "ground";
    private const int LeftMouseButton = 0;
    public int selectedCreature = 1;

    void Update()
    {
        HandleCreatureSelection();

        if (Input.GetMouseButtonDown(LeftMouseButton))
        {
            if (selectedCreature == 4)
            {
                SpawnRandomCreature();
            }
            else
            {
                SpawnSelectedCreature(selectedCreature - 1);
            }
        }
    }

    void HandleCreatureSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedCreature = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedCreature = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedCreature = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedCreature = 4;
        }
    }

    void SpawnSelectedCreature(int index)
    {
        if (index < creaturePrefabs.Length)
        {
            GameObject creatureToSpawn = creaturePrefabs[index];
            SpawnCreature(creatureToSpawn);
        }
    }

    void SpawnRandomCreature()
    {
        int randomIndex = Random.Range(3, creaturePrefabs.Length);
        GameObject creatureToSpawn = creaturePrefabs[randomIndex];
        SpawnCreature(creatureToSpawn);
    }

    void SpawnCreature(GameObject creaturePrefab)
    {
        Vector3 playerPosition = playerPrefab.transform.position;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(GroundTag))
        {
            Vector3 spawnPosition = hit.point;

            if (Vector3.Distance(new Vector3(playerPosition.x, 0f, playerPosition.z), new Vector3(spawnPosition.x, 0f, spawnPosition.z)) > spawnRange)
            {
                if (creaturePrefab != null)
                {
                    Instantiate(creaturePrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }
}
