using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // EventSystem 사용

public class CreatureSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // 플레이어 프리팹
    public GameObject[] creaturePrefabs; // 적 오브젝트 프리팹 배열
    public float spawnRange = 30f; // 플레이어와의 최소 소환 범위

    private const string GroundTag = "ground";
    private const int LeftMouseButton = 0;
    public int selectedCreature = 1;

    private LineRenderer lineRenderer;

    public UI_selectCreature[] uiButtons; // UI 버튼 스크립트 배열
    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;

    // UI_Setting 스크립트에 대한 참조 추가
    public UI_Setting uiSetting;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 360;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = false; // 로컬 좌표계 사용
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    public bool IsWithinSpawnRange(Vector3 position)
    {
        Vector3 playerPosition = playerPrefab.transform.position;
        return Vector3.Distance(playerPosition, position) <= spawnRange;
    }

    void Update()
    {
        HandleCreatureSelection();

        // 설정창이 활성화된 경우 소환 방지
        if (uiSetting != null && !uiSetting.IsSettingsPanelActive())
        {
            // UI 클릭이 아니라면 마우스 클릭 처리
            if (Input.GetMouseButtonDown(LeftMouseButton) && !IsPointerOverUIElement())
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

        DrawSpawnRange();
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

    void DrawSpawnRange()
    {
        Vector3 playerPosition = playerPrefab.transform.position;
        Vector3 center = new Vector3(playerPosition.x, 1f, playerPosition.z);

        for (int i = 0; i < 360; i++)
        {
            float rad = Mathf.Deg2Rad * i;
            float x = center.x + Mathf.Cos(rad) * spawnRange;
            float z = center.z + Mathf.Sin(rad) * spawnRange;
            lineRenderer.SetPosition(i, new Vector3(x, center.y, z));
        }
    }

    void SpawnSelectedCreature(int index)
    {
        if (index < creaturePrefabs.Length)
        {
            if (!uiButtons[index].IsOnCooldown())
            {
                GameObject creatureToSpawn = creaturePrefabs[index];
                SpawnCreature(creatureToSpawn);

                // 버튼의 쿨타임 시작
                TriggerButtonCooldown(index);
            }
        }
    }

    void SpawnRandomCreature()
    {
        int randomIndex = Random.Range(3, creaturePrefabs.Length);
        if (!uiButtons[randomIndex].IsOnCooldown())
        {
            GameObject creatureToSpawn = creaturePrefabs[randomIndex];
            SpawnCreature(creatureToSpawn);

            // 버튼의 쿨타임 시작
            TriggerButtonCooldown(randomIndex);
        }
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

    void TriggerButtonCooldown(int index)
    {
        if (index < uiButtons.Length)
        {
            uiButtons[index].StartCooldown();
        }
    }

    // UI 위에서 클릭이 발생했는지 확인하는 함수
    bool IsPointerOverUIElement()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }
}
