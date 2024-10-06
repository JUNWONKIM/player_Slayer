using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class CreatureSpawner : MonoBehaviour
{
    public GameObject playerPrefab; // 용사 프리팹
    public GameObject[] creaturePrefabs; // 크리쳐 프리팹
    public float spawnRange = 30f; //최소 소환 범위
    public int selectedCreature = 1; // 선택된 크리쳐, 처음에 1번으로 설정
    public UI_selectCreature[] uiButtons; // UI 버튼 
    public GraphicRaycaster graphicRaycaster; //캔버스 레이캐스터
    public EventSystem eventSystem; 
    public UI_Setting uiSetting;  // UI_Setting 

    private const string GroundTag = "ground";
    private const int LeftMouseButton = 0;
    private LineRenderer lineRenderer;

    private PlayerHP playerHP; // 플레이어 체력 스크립트

    void Start()
    {
        // 소환 가능 범위 표시
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 360;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        // 플레이어의 체력 스크립트 참조
        playerHP = playerPrefab.GetComponent<PlayerHP>();

        // 선택된 크리처를 1번으로 강제 설정
        selectedCreature = 1;
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

    public bool IsWithinSpawnRange(Vector3 position) //스폰 가능 범위 확인
    {
        Vector3 playerPosition = playerPrefab.transform.position;
        return Vector3.Distance(playerPosition, position) <= spawnRange;
    }

    void HandleCreatureSelection() // 소환할 크리쳐 선택
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
            // 플레이어 체력이 70% 미만일 때만 3번 크리쳐 선택 가능
            if (playerHP.hp <= playerHP.max_hp * 0.7f)
            {
                selectedCreature = 3;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // 플레이어 체력이 50% 미만일 때만 4번 크리쳐 선택 가능
            if (playerHP.hp <= playerHP.max_hp * 0.5f)
            {
                selectedCreature = 4;
            }
        }
    }

    void DrawSpawnRange() //스폰 범위 표시
    {
        Vector3 playerPosition = playerPrefab.transform.position;
        Vector3 center = new Vector3(playerPosition.x, 1f, playerPosition.z);

        for (int i = 0; i < 360; i++) //LinRenderer 사용
        {
            float rad = Mathf.Deg2Rad * i;
            float x = center.x + Mathf.Cos(rad) * spawnRange;
            float z = center.z + Mathf.Sin(rad) * spawnRange;
            lineRenderer.SetPosition(i, new Vector3(x, center.y, z));
        }
    }

    void SpawnSelectedCreature(int index) //선택된 크리쳐 소환
    {
        if (index < creaturePrefabs.Length)
        {
            if (!uiButtons[index].IsOnCooldown())
            {
                GameObject creatureToSpawn = creaturePrefabs[index];
                SpawnCreature(creatureToSpawn);
                TriggerButtonCooldown(index); //버튼 쿨타임 시작
            }
        }
    }

    void SpawnRandomCreature() //마녀 랜덤 소환
    {
        int randomIndex = Random.Range(3, creaturePrefabs.Length);
        if (!uiButtons[randomIndex].IsOnCooldown())
        {
            GameObject creatureToSpawn = creaturePrefabs[randomIndex];
            SpawnCreature(creatureToSpawn);
            TriggerButtonCooldown(randomIndex);//버튼 쿨타임 시작
        }
    }

    void SpawnCreature(GameObject creaturePrefab) //크리쳐 소환
    {
        Vector3 playerPosition = playerPrefab.transform.position;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(GroundTag)) //클릭된 오브젝트가 Ground일 경우
        {
            Vector3 spawnPosition = hit.point;

            if (Vector3.Distance(new Vector3(playerPosition.x, 0f, playerPosition.z), new Vector3(spawnPosition.x, 0f, spawnPosition.z)) > spawnRange) //스폰 범위일 경우
            {
                if (creaturePrefab != null)
                {
                    Instantiate(creaturePrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }

    void TriggerButtonCooldown(int index) //UI 쿨다운 시작
    {
        if (index < uiButtons.Length)
        {
            uiButtons[index].StartCooldown();
        }
    }

    bool IsPointerOverUIElement() //설정창이 클릭될 경우 소환 x
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        return results.Count > 0;
    }
}
